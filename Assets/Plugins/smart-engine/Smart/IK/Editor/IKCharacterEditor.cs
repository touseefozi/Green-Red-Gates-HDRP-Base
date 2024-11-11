#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Smart.Basics.Extensions;
using Smart.Utils;
using SmartEditor.Utils;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Smart.IK
{
	[CustomEditor(typeof(IKCharacter))]
	public class IKCharacterEditor : Editor
	{
		private enum PivotMode { Universal, Moving, Rotation }
		
		private const string MoveActionName = "Move IK Pivot";
		private const string RotateActionName = "Rotate IK Limb";
		private const string SelectActionName = "Select IK Pivot";
		private const string PoseActionName = "Change IK Pose";
		private const string ResetSelectionActionName = "Reset IK Selection";
		private const string RecordKeyframeActionName = "Record ID Animation Keyframe";
		
		private const float ButtonHeight = 36f;
		private const float Space = 10f;
		private const float RotationPrecision = 100f;
		private const float SmallFloat =  0.00001f;
		private const float DoubleClickMaxDelay = 0.25f;
		private const float NotificationHideDelay = 0.75f;
		
		private IKCharacter _character;
		private IKTransformMode _ikTransformMode;
		
		[SerializeField] private long _lastClickTicks;
		[SerializeField] private IKPivot _lastSelectedPivot;
		[SerializeField] private IKPivot[] _posePivots;
		[SerializeField] private Vector3 _posePelvisPosition;
		[SerializeField] private Quaternion[] _poseRotations;
		[SerializeField] private Tool _lastUsedTool;
		[SerializeField] private Quaternion _initRotation;
		[SerializeField] private bool _isRotationMode;
		[SerializeField] private int _lastFrame;
		[SerializeField] private string _lastClipName;

		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			
			_character = (IKCharacter) target;
			
			var armLeftChain = _character.ArmLeftChain;
			var armRightChain = _character.ArmRightChain;
			var legLeftChain = _character.LegLeftChain;
			var legRightChain = _character.LegRightChain;
			
			GUILayout.Space(Space);
			EditorGUI.BeginChangeCheck();
			var armLeftConstraint = EditorGUILayout.Toggle("Left Arm Constraint", armLeftChain.ConstraintEnabled);
			var armRightConstraint = EditorGUILayout.Toggle("Right Arm Constraint", armRightChain.ConstraintEnabled);
			var legLeftConstraint = EditorGUILayout.Toggle("Left Leg Constraint", legLeftChain.ConstraintEnabled);
			var legRightConstraint = EditorGUILayout.Toggle("Right Leg Constraint", legRightChain.ConstraintEnabled);
			GUILayout.Space(Space);

			if (EditorGUI.EndChangeCheck())
			{
				armLeftChain.ConstraintEnabled = armLeftConstraint; 
				armRightChain.ConstraintEnabled = armRightConstraint; 
				legLeftChain.ConstraintEnabled = legLeftConstraint; 
				legRightChain.ConstraintEnabled = legRightConstraint; 
				EditorUtility.SetDirty(_character);
			}

			if (IsSingleSelection())
			{
				var bone = _character.GetPivotTransform(_character.SelectedPivots.First());

				if (bone != null)
				{
					EditorGUI.BeginChangeCheck();
				
					var roundedRotation = SmartMath.RoundTo(bone.localEulerAngles, RotationPrecision);
					var rotation = EditorGUILayout.Vector3Field("Bone Rotation", roundedRotation);
				
					if (EditorGUI.EndChangeCheck())
					{
						Undo.RecordObject(bone, RotateActionName);
						bone.localRotation = Quaternion.Euler(rotation);
						EditorUtility.SetDirty(bone);
					}
				}
			}
			
			GUILayout.Space(Space);
			
			if (GUILayout.Button("Initialize", GUILayout.Height(ButtonHeight)))
			{
				_character.Initialize();
				EditorUtility.SetDirty(_character);
				Repaint();
			}
			
			if (_character.IsValid())
			{
				if (GUILayout.Button("Zero Pose", GUILayout.Height(ButtonHeight)))
				{
					RecordFullHierarchy(PoseActionName);
					_character.ApplyZeroPose();
					EditorUtility.SetDirty(_character);
					Repaint();
				}
			
				if (AnimationWindowHelper.IsRecordingMode() && GUILayout.Button("Record Keyframe", GUILayout.Height(ButtonHeight)))
				{
					RecordKeyframe();
				}
			}
		}

		private void OnSceneGUI()
		{
			_character = (IKCharacter) target;
			
			if (_character.IsValid() && Selection.objects.Length <= 1)
			{
				UpdatePivots();
				ProcessKeyboardShortcuts();
				UpdateSelection();
				DrawBones();
				DrawPivots();
			}
		}

		private void UpdatePivots()
		{
			var clip = AnimationWindowHelper.GetAnimationWindowCurrentClip();

			if (clip != null)
			{
				var currentFrame = AnimationWindowHelper.GetCurrentFrame();

				if (currentFrame != _lastFrame || _lastClipName != clip.name)
				{
					_character.ResetPivots();
					_lastClipName = clip.name;
					_lastFrame = currentFrame;
				}
			}
		}

		private void ProcessKeyboardShortcuts()
		{
			var currentEvent = Event.current;
				
			if (currentEvent.type == EventType.KeyDown)
			{
				var keyCode = currentEvent.keyCode;
				
				if (currentEvent.alt)
				{
					switch (keyCode)
					{
						case KeyCode.M: SwitchMirroring();     break;
						case KeyCode.L: SwitchSelectionLock(); break;
					}
				}
				else if (currentEvent.command || currentEvent.control)
				{
					switch (keyCode)
					{
						case KeyCode.C: CopyPose();  break;
						case KeyCode.V: PastePose(); break;
					}
				}
				else
				{
					switch (keyCode)
					{
						case KeyCode.M: SwitchMirroring();                           break;
						case KeyCode.F: FocusOnCurrentPivot();                       break;
						case KeyCode.E: _ikTransformMode = IKTransformMode.Rotation; break;
						case KeyCode.W: _ikTransformMode = IKTransformMode.Moving;   break;
					}
				}
			}
		}

		private void SwitchMirroring()
		{
			_character.SwitchMirroring();
			EditorUtility.SetDirty(_character);
			ShowNotification($"Mirroring Enabled: {_character.EnableMirroring}");
		}

		private void SwitchSelectionLock()
		{
			_character.SwitchSelectionLock();
			EditorUtility.SetDirty(_character);
			ShowNotification($"Selection Lock: {_character.LockSelection}");
		}

		private async void CopyPose()
		{
			if (!IsCharacterNotSelected())
			{
				var count = _character.SelectedPivots.Count;
				
				_posePivots = new IKPivot[count];
				_poseRotations = new Quaternion[count];

				for (int i = 0; i < count; i++)
				{
					var pivot = _character.SelectedPivots[i];
					_posePivots[i] = pivot;
					_poseRotations[i] = _character.GetPivotTransformRotation(pivot);

					if (pivot == IKPivot.Pelvis)
					{
						_posePelvisPosition = _character.GetPivotPosition(IKPivot.Pelvis);
					}
				}
				
				await Task.Delay(1);
				EditorGUIUtility.systemCopyBuffer = null;
				ShowNotification("Pose copied");
			}
		}

		private void PastePose()
		{
			if (!IsCharacterNotSelected())
			{
				var dictionary = new Dictionary<IKPivot, Quaternion>();

				for (int i = 0; i < _posePivots.Length; i++)
				{
					var pivot = _posePivots[i];
					var rotation = _poseRotations[i];
					dictionary.Add(pivot, rotation);
				}
				
				var count = _character.SelectedPivots.Count;
				var isDirty = false;
				
				for (int i = 0; i < count; i++)
				{
					var pivot = _character.SelectedPivots[i];

					if (dictionary.ContainsKey(pivot))
					{
						if (!isDirty)
						{
							RecordFullHierarchy(PoseActionName);
							isDirty = true;
						}

						if (pivot == IKPivot.Pelvis)
						{
							_character.SetPelvisPosition(_posePelvisPosition);
						}
						
						var bone = _character.GetPivotTransform(pivot);
						var rotation = dictionary[pivot];
						Undo.RecordObject(bone, PoseActionName);
						_character.SetPivotLocalRotation(pivot, rotation);
						EditorUtility.SetDirty(bone);
					}
				}

				if (isDirty)
				{
					EditorUtility.SetDirty(_character.gameObject);
				}
			}
		}

		private void DrawBones()
		{
			DrawPoleLine(_character.SpineChain);
			DrawPoleLine(_character.HeadChain);
			DrawPoleLine(_character.ArmLeftChain);
			DrawPoleLine(_character.ArmRightChain);
			DrawPoleLine(_character.LegLeftChain);
			DrawPoleLine(_character.LegRightChain);
				
			DrawBoneLine(IKPivot.Pelvis, IKPivot.Stomach, IKPivot.Chest, IKPivot.Neck, IKPivot.Head, IKPivot.HeadEnd);
			DrawBoneLine(IKPivot.ClavicleLeft, IKPivot.ArmLeft, IKPivot.ForearmLeft, IKPivot.HandLeft);
			DrawBoneLine(IKPivot.ClavicleRight, IKPivot.ArmRight, IKPivot.ForearmRight, IKPivot.HandRight);
			DrawBoneLine(IKPivot.Pelvis, IKPivot.ThighLeft, IKPivot.CalfLeft, IKPivot.FootLeft, IKPivot.ToeLeft);
			DrawBoneLine(IKPivot.Pelvis, IKPivot.ThighRight, IKPivot.CalfRight, IKPivot.FootRight, IKPivot.ToeRight);
			
			DrawBoneLine(IKPivot.HandLeft, IKPivot.FingersLeft_1, IKPivot.FingersLeft_2, IKPivot.FingersLeft_3);
			DrawBoneLine(IKPivot.HandLeft, IKPivot.ThumbLeft_1, IKPivot.ThumbLeft_2);
			DrawBoneLine(IKPivot.ThumbLeft_1, IKPivot.FingersLeft_1);
			
			DrawBoneLine(IKPivot.HandRight, IKPivot.FingersRight_1, IKPivot.FingersRight_2, IKPivot.FingersRight_3);
			DrawBoneLine(IKPivot.HandRight, IKPivot.ThumbRight_1, IKPivot.ThumbRight_2);
			DrawBoneLine(IKPivot.ThumbRight_1, IKPivot.FingersRight_1);
		}

		private void DrawPivots()
		{
			var targetSize = _character.TargetSize;
			var poleSize = _character.PoleSize;
			var rotatorSize = _character.RotatorSize;
				
			DrawPivot(IKPivot.HeadEnd, targetSize, PivotMode.Moving);
			DrawPivot(IKPivot.Pelvis, targetSize, PivotMode.Universal);
			DrawPivot(IKPivot.Neck, targetSize, PivotMode.Universal);
			DrawPivot(IKPivot.HandLeft, targetSize, PivotMode.Universal);
			DrawPivot(IKPivot.HandRight, targetSize, PivotMode.Universal);
			DrawPivot(IKPivot.FootLeft, targetSize, PivotMode.Universal);
			DrawPivot(IKPivot.FootRight, targetSize, PivotMode.Universal);
			
			DrawPivot(IKPivot.HeadPole, poleSize, PivotMode.Moving);
			DrawPivot(IKPivot.ChestPole, poleSize, PivotMode.Moving);
			DrawPivot(IKPivot.ArmPoleLeft, poleSize, PivotMode.Moving);
			DrawPivot(IKPivot.ArmPoleRight, poleSize, PivotMode.Moving);
			DrawPivot(IKPivot.LegPoleLeft, poleSize, PivotMode.Moving);
			DrawPivot(IKPivot.LegPoleRight, poleSize, PivotMode.Moving);
			
			DrawPivot(IKPivot.Stomach, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.Chest, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.Head, rotatorSize, PivotMode.Rotation);
				
			DrawPivot(IKPivot.ClavicleLeft, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ArmLeft, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ForearmLeft, rotatorSize, PivotMode.Rotation);
				
			DrawPivot(IKPivot.ClavicleRight, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ArmRight, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ForearmRight, rotatorSize, PivotMode.Rotation);
				
			DrawPivot(IKPivot.ThighLeft, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.CalfLeft, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ToeLeft, rotatorSize, PivotMode.Rotation);
				
			DrawPivot(IKPivot.ThighRight, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.CalfRight, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ToeRight, rotatorSize, PivotMode.Rotation);
			
			DrawPivot(IKPivot.FingersLeft_1, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.FingersLeft_2, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.FingersLeft_3, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ThumbLeft_1, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ThumbLeft_2, rotatorSize, PivotMode.Rotation);
			
			DrawPivot(IKPivot.FingersRight_1, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.FingersRight_2, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.FingersRight_3, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ThumbRight_1, rotatorSize, PivotMode.Rotation);
			DrawPivot(IKPivot.ThumbRight_2, rotatorSize, PivotMode.Rotation);
		}

		private async void FocusOnCurrentPivot()
		{
			if (_character.SelectedPivots.IsNotNullOrEmpty())
			{
				var pivot = _character.SelectedPivots.First();
				var position = IsMultiplySelection() ? _character.GetPivotsCenter() : _character.GetPivotTransformPosition(pivot);

				if (position != Vector3.zero)
				{
					await Task.Delay(1);
					SceneView.lastActiveSceneView.LookAt(position);
				}
			}
		}

		private void DrawPoleLine(IKChain chain)
		{
			var transform = chain.RootTransform;
			var pointA = chain.GetPositionRootSpace(chain.MiddleBone.position);
			var pointB = chain.PolePosition;
				
			Handles.color = _character.PoleLineColor;
			Handles.DrawLine(transform.TransformPoint(pointA), transform.TransformPoint(pointB), _character.BoneThickness);
		}

		private void DrawBoneLine(params IKPivot[] pivots)
		{
			Handles.color = _character.BoneColor;
			
			for (int i = 0; i < pivots.Length - 1; i++)
			{
				var pivotA = pivots[i];
				var pivotB = pivots[i + 1];

				if (_character.HasPivot(pivotA) && _character.HasPivot(pivotB))
				{
					var pointA = _character.GetPivotTransformPosition(pivotA);
					var pointB = _character.GetPivotTransformPosition(pivotB);
		
					Handles.DrawLine(pointA, pointB, _character.BoneThickness);
				}
			}
		}

		private void DrawPivot(IKPivot pivot, float pivotSize, PivotMode pivotMode)
		{
			if (_character.HasPivot(pivot))
			{
				var position = GetPivotPosition(pivot, pivotMode);
				var isSelected = IsPivotSelected(pivot);
			
				Handles.color = isSelected ? _character.PivotSelectedColor : _character.PivotNormalColor;
			
				if (Handles.Button(position, Quaternion.identity, pivotSize, pivotSize / 2f, Handles.SphereHandleCap))
				{
					SelectPivot(pivot);
				}
			
				if (isSelected && pivot == _lastSelectedPivot)
				{
					TransformPivot(pivot, pivotMode);
				}
			}
		}

		private Vector3 GetPivotPosition(IKPivot pivot, PivotMode pivotMode)
		{
			if (pivotMode == PivotMode.Rotation)
			{
				var transform = _character.GetPivotTransform(pivot);
				return transform.TransformPoint(Vector3.zero);
			}
			else
			{
				var transform = _character.GetPivotRootTransform(pivot);
				var position = _character.GetPivotPosition(pivot);
				return transform.TransformPoint(position);
			}
		}

		private void TransformPivot(IKPivot pivot, PivotMode pivotMode)
		{
			if (Tools.current != Tool.None)
			{
				_lastUsedTool = Tools.current;
				Tools.current = Tool.None;
			}
			
			switch (pivotMode)
			{
				case PivotMode.Universal:
				{
					switch (_ikTransformMode)
					{
						case IKTransformMode.Moving:	 MovePivot(pivot);	 break;
						case IKTransformMode.Rotation: RotatePivot(pivot); break;
					}
					break;
				}
				case PivotMode.Rotation: RotatePivot(pivot); break;
				case PivotMode.Moving:	 MovePivot(pivot);	 break;
			}
		}
		
		private void MovePivot(IKPivot pivot)
		{
			if (Event.current.type == EventType.MouseDown)
			{
				ProcessDoubleClick();
			}
			
			var transform = _character.GetPivotRootTransform(pivot);
			var point = _character.GetPivotPosition(pivot);
			point = transform.TransformPoint(point);
			
			EditorGUI.BeginChangeCheck();
			point = Handles.DoPositionHandle(point, Quaternion.identity);
            
			if (EditorGUI.EndChangeCheck())
			{
				RecordFullHierarchy(MoveActionName);
				point = transform.InverseTransformPoint(point);
				_character.SetPivotPosition(pivot, point);
				
				if (_character.EnableMirroring)
				{
					MirrorRotateChildren(pivot);
				}

				if (pivot == IKPivot.Pelvis)
				{
					var pelvis = _character.Skeleton.Pelvis;
					Undo.RecordObject(pelvis, MoveActionName);
					pelvis.position = pelvis.position;
					EditorUtility.SetDirty(pelvis);
				}
				
				RecordBones(pivot, IKTransformMode.Moving, MoveActionName);
				EditorUtility.SetDirty(_character.gameObject);
				Repaint();
			}
		}

		private void RotatePivot(IKPivot pivot)
		{
			var transform = _character.GetPivotTransform(pivot);
			
			EditorGUI.BeginChangeCheck();
			var isGlobalMode = Tools.pivotRotation == PivotRotation.Global;
			var position = transform.TransformPoint(Vector3.zero);
			var rotation = Handles.RotationHandle(isGlobalMode ? Quaternion.identity : transform.rotation, position);

			var type = Event.current.type;

			if (!_isRotationMode && type == EventType.Used)
			{
				_isRotationMode = true;
				_initRotation = transform.rotation;
			}
			else if (_isRotationMode && type == EventType.MouseMove)
			{
				_isRotationMode = false;
			}
			
			if (EditorGUI.EndChangeCheck())
			{
				RecordFullHierarchy(RotateActionName);
				Undo.RecordObject(transform, RotateActionName);

				if (isGlobalMode)
				{
					var newRotation = rotation.eulerAngles;
					newRotation.y *= -1f;
					rotation = Quaternion.Euler(_initRotation.eulerAngles - newRotation);
					_character.SetPivotRotation(pivot, rotation);
				}
				else
				{
					_character.SetPivotRotation(pivot, rotation);
				}

				if (_character.EnableMirroring)
				{
					MirrorRotatePoint(pivot);
				}

				RecordBones(pivot, IKTransformMode.Rotation, RotateActionName);
				EditorUtility.SetDirty(_character.gameObject);
				EditorUtility.SetDirty(transform);
				Repaint();
			}
		}

		private void MirrorRotateChildren(IKPivot pivot)
		{
			var children = IKPivotConstants.GetLimbChildren(pivot);

			if (children != null)
			{
				foreach (var childPivot in children)
				{
					MirrorRotatePoint(childPivot);
				}
			}
		}
		
		private void MirrorRotatePoint(IKPivot pivot)
		{
			var mirroredPivot = IKPivotConstants.GetMirroredPivot(pivot);

			if (mirroredPivot != IKPivot.Unknown)
			{
				var transform = _character.GetPivotTransform(mirroredPivot);
				var localRotation = _character.GetPivotTransform(pivot).localRotation;
				var euler = localRotation.normalized.eulerAngles;
				localRotation = Quaternion.Euler(euler.x, 360f - euler.y, -euler.z);
		
				Undo.RecordObject(transform, RotateActionName);
				_character.SetPivotLocalRotation(mirroredPivot, localRotation);
				EditorUtility.SetDirty(transform);
			}
		}

		private void SelectPivot(IKPivot pivot)
		{
			var isShiftPressed = Event.current.shift;
			
			if (IsPivotSelected(pivot))
			{
				if (isShiftPressed)
				{
					_character.SelectedPivots.SafeRemove(pivot);
					_lastSelectedPivot = _character.SelectedPivots.FirstOrDefault();
				}
				else
				{
					_lastSelectedPivot = pivot;
					ProcessDoubleClick();
				}
			}
			else
			{
				if (!isShiftPressed)
				{
					_lastSelectedPivot = IKPivot.Unknown;
					_character.SelectedPivots.Clear();
				}
				
				Undo.RecordObject(_character, SelectActionName);
				_character.SelectedPivots.Add(pivot);
				_lastSelectedPivot = pivot;
				_lastClickTicks = DateTime.UtcNow.Ticks;
				EditorUtility.SetDirty(_character);
				Repaint();
			}
		}
		
		private void ProcessDoubleClick()
		{
			if (_lastSelectedPivot != IKPivot.Unknown)
			{
				var ticks = DateTime.UtcNow.Ticks;
				var delta = (ticks - _lastClickTicks) / (float) TimeSpan.TicksPerSecond;
				
				if (delta <= DoubleClickMaxDelay)
				{
					Undo.RecordObject(_character, SelectActionName);
					_character.SelectWithChildren(_lastSelectedPivot);
					EditorUtility.SetDirty(_character);
					Repaint();
				}
				else
				{
					_lastClickTicks = ticks;
				}
			}
		}

		private void UpdateSelection()
		{
			if (_character.LockSelection)
			{
				if (IsCharacterNotSelected())
				{
					ResetSelection();
					Tools.current = _lastUsedTool;
				}
					
				FocusOnCharacter();
			}
			else if (IsCharacterNotSelected() && IsSelectionNotEmpty())
			{
				ResetSelection();
				FocusOnCharacter();
				Tools.current = _lastUsedTool;
			}

			if (IsSelectionNotEmpty() && _lastSelectedPivot == IKPivot.Unknown)
			{
				_lastSelectedPivot = _character.SelectedPivots.First();
			}
		}

		private void FocusOnCharacter()
		{
			Selection.objects = new Object[] {_character.gameObject};
		}

		private void ResetSelection()
		{
			_lastSelectedPivot = IKPivot.Unknown;
			Undo.RecordObject(_character, ResetSelectionActionName);
			_character.SelectedPivots.Clear();
			EditorUtility.SetDirty(_character);
			Repaint();
		}

		private bool IsPivotSelected(IKPivot pivot)
		{
			return _character.SelectedPivots.Contains(pivot);
		}
		
		private bool IsCharacterNotSelected()
		{
			var objects = Selection.objects;
			return objects.Length == 0 || objects[0] != _character.gameObject;
		}
		
		private bool IsSelectionNotEmpty()
		{
			return _character.SelectedPivots.Count > 0;
		}

		private bool IsMultiplySelection()
		{
			return _character.SelectedPivots.Count > 1;
		}

		private bool IsSingleSelection()
		{
			return _character.SelectedPivots.Count == 1;
		}

		private void RecordBones(IKPivot pivot, IKTransformMode mode, string actionName)
		{
			var bones = _character.GetAffectedBones(pivot, mode);

			foreach (var bone in bones)
			{
				Undo.RecordObject(bone, actionName);
				bone.rotation = bone.rotation;
				EditorUtility.SetDirty(bone);
			}
		}

		private void RecordKeyframe()
		{
			var bones = _character.GetAllBones();
			var pelvis = _character.GetPelvisTransform();
			
			var positionDelta = new Vector3(0f, 0f, SmallFloat);
			pelvis.position -= positionDelta;
			
			foreach (var bone in bones)
			{
				if (bone != null)
				{
					var rotation = bone.rotation;
					rotation.w -= SmallFloat;
					bone.rotation = rotation;
				}
			}
			
			foreach (var bone in bones)
			{
				if (bone != null)
				{
					Undo.RecordObject(bone, RecordKeyframeActionName);
					var rotation = bone.rotation;
					rotation.w += SmallFloat;
					bone.rotation = rotation;

					if (bone == pelvis)
					{
						pelvis.position += positionDelta;
					}
					EditorUtility.SetDirty(bone);
				}
			}
		}
		
		public void RecordFullHierarchy(string actionName)
		{
			Undo.RegisterFullObjectHierarchyUndo(_character.gameObject, actionName);
		}
		
		private void ShowNotification(string message)
		{
			var sceneView = SceneView.lastActiveSceneView;
			sceneView.RemoveNotification();
			sceneView.ShowNotification(new GUIContent(message), NotificationHideDelay);
		}
	}
}
#endif