using System.Collections.Generic;
using System.Linq;
using Smart.Extensions;
using UnityEngine;

namespace Smart.IK
{
	[SelectionBase]
	public class IKCharacter : MonoBehaviour
	{
		private static readonly HashSet<Transform> _temporaryHashSet = new HashSet<Transform>();
		
		private readonly List<Transform> EmptyList = new List<Transform>();
		
		[HideInInspector] [SerializeField] private List<IKPivot> _selectedPivots;
		[SerializeField] private IKSkeleton _skeleton;
		[Space]
		[Header("Handlers Settings:")]
		[SerializeField] private float _targetSize = 0.03f;
		[SerializeField] private float _poleSize = 0.03f;
		[SerializeField] private float _rotatorSize = 0.015f;
		[SerializeField] private float _boneThickness = 2.0f;
		[Space]
		[Header("Color Settings:")]
		[SerializeField] private Color _boneColor = Color.black;
		[SerializeField] private Color _poleLineColor = new Color(0f, 0f, 0f, 0.5f);
		[SerializeField] private Color _pivotNormalColor = Color.black;
		[SerializeField] private Color _pivotSelectedColor = Color.yellow;
		[Space]
		[Header("IK Solver Settings:")]
		[SerializeField] private bool _lockSelection;
		[SerializeField] private bool _enableMirroring;
		[SerializeField] private float _delta = 0.001f;
		[SerializeField] [Range(1, 30)] private int _iterations = 10;
		[SerializeField] [Range(0, 1)]  private float _snapBackStrength = 1f;
		
		[HideInInspector] [SerializeField] private IKPoseData _zeroPose;
		[HideInInspector] [SerializeField] private IKChain _spineChain;
		[HideInInspector] [SerializeField] private IKChain _headChain;
		[HideInInspector] [SerializeField] private IKChain _armLeftChain;
		[HideInInspector] [SerializeField] private IKChain _armRightChain;
		[HideInInspector] [SerializeField] private IKChain _legLeftChain;
		[HideInInspector] [SerializeField] private IKChain _legRightChain;
		
		public List<IKPivot> SelectedPivots  => _selectedPivots ??= new List<IKPivot>();
		
		public IKSkeleton Skeleton => _skeleton;
		public IKChain SpineChain => _spineChain;
		public IKChain HeadChain => _headChain;
		public IKChain ArmLeftChain => _armLeftChain;
		public IKChain ArmRightChain => _armRightChain;
		public IKChain LegLeftChain => _legLeftChain;
		public IKChain LegRightChain => _legRightChain;
			
		public float TargetSize => _targetSize;
		public float PoleSize => _poleSize;
		public float RotatorSize => _rotatorSize;
		public float BoneThickness => _boneThickness;
		
		public Color BoneColor => _boneColor;
		public Color PoleLineColor => _poleLineColor;
		public Color PivotNormalColor => _pivotNormalColor;
		public Color PivotSelectedColor => _pivotSelectedColor;
		
		public bool LockSelection => _lockSelection;
		public bool EnableMirroring => _enableMirroring;
		public int Iterations => _iterations;
		public float Delta => _delta;
		public float SnapBackStrength => _snapBackStrength;

		public bool IsValid()
		{
			return _skeleton.IsValid() &&
			       _spineChain.IsValid &&
			       _headChain.IsValid &&
			       _armLeftChain.IsValid &&
			       _armRightChain.IsValid &&
			       _legLeftChain.IsValid &&
			       _legRightChain.IsValid;
		}

		public void Initialize()
		{
			if (_skeleton.IsValid())
			{
				_spineChain = new IKChain(this, _skeleton.Pelvis, _skeleton.Stomach, _skeleton.Neck);
				_headChain = new IKChain(this, _skeleton.Chest, _skeleton.Neck, _skeleton.HeadEnd);
				_armLeftChain = new IKChain(this, _skeleton.ClavicleLeft, _skeleton.ArmLeft, _skeleton.HandLeft);
				_armRightChain = new IKChain(this, _skeleton.ClavicleRight, _skeleton.ArmRight, _skeleton.HandRight);
				_legLeftChain = new IKChain(this, _skeleton.Pelvis, _skeleton.ThighLeft, _skeleton.FootLeft, reversePole: true);
				_legRightChain = new IKChain(this, _skeleton.Pelvis, _skeleton.ThighRight, _skeleton.FootRight, reversePole: true);
				
				_legLeftChain.ConstraintEnabled = true;
				_legRightChain.ConstraintEnabled = true;
				
				_zeroPose = new IKPoseData(_skeleton.Pelvis.localPosition, _skeleton.GetRotations());
			}
			else
			{
				Debug.LogError("Skeleton should contains all bones!");
			}
		}
		
		public void ApplyZeroPose()
		{
			ApplyPose(_zeroPose);
		}

		private void ApplyPose(IKPoseData pose)
		{
			var bones = _skeleton.GetAllBones();
			_skeleton.Pelvis.localPosition = pose.PelvisLocalPosition;

			for (var i = 0; i < pose.Rotations.Length && i < bones.Count; i++)
			{
				bones[i].localRotation = pose.Rotations[i];
			}

			ResetPivots();
		}

		public void ResetPivots()
		{
			_spineChain.ResetPivots();
			_headChain.ResetPivots();
			_armLeftChain.ResetPivots();
			_armRightChain.ResetPivots();
			_legLeftChain.ResetPivots();
			_legRightChain.ResetPivots();
		}

		public Vector3 GetPivotsCenter()
		{
			var minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			var maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

			foreach (var pivot in SelectedPivots)
			{
				var position = GetPivotTransformPosition(pivot);

				if (position != Vector3.zero)
				{
					if (position.x < minBounds.x) minBounds.x = position.x;
					if (position.y < minBounds.y) minBounds.y = position.y;
					if (position.z < minBounds.z) minBounds.z = position.z;
					if (position.x > maxBounds.x) maxBounds.x = position.x;
					if (position.y > maxBounds.y) maxBounds.y = position.y;
					if (position.z > maxBounds.z) maxBounds.z = position.z;
				}
			}
			
			return minBounds + (maxBounds - minBounds) / 2f;
		}
		
		public void SetPelvisPosition(Vector3 position)
		{
			_skeleton.Pelvis.localPosition = position;
		}
		
		public void SetPelvisPositionWithConstraints(Vector3 position)
		{
			CacheTargetsConstraints();
			_skeleton.Pelvis.localPosition = position;
			RestoreTargetsConstraints(IKPivot.Pelvis);
		}

		private void CacheTargetsConstraints()
		{
			_armLeftChain.CacheConstraintTargetPosition();
			_armRightChain.CacheConstraintTargetPosition();
			_legLeftChain.CacheConstraintTargetPosition();
			_legRightChain.CacheConstraintTargetPosition();
		}

		private void RestoreTargetsConstraints(IKPivot pivot)
		{
			if (pivot == IKPivot.Pelvis)
			{
				if (_legLeftChain.ConstraintEnabled)  _legLeftChain.ApplyConstraintTargetPosition();
				if (_legRightChain.ConstraintEnabled) _legRightChain.ApplyConstraintTargetPosition();
			}
			
			if (IsArmsConstraintsAffected(pivot))
			{
				if (_armLeftChain.ConstraintEnabled)  _armLeftChain.ApplyConstraintTargetPosition();
				if (_armRightChain.ConstraintEnabled) _armRightChain.ApplyConstraintTargetPosition();
			}
		}

		private bool IsArmsConstraintsAffected(IKPivot pivot)
		{
			switch (pivot)
			{
				case IKPivot.ClavicleLeft:
				case IKPivot.ClavicleRight:
				case IKPivot.Pelvis:
				case IKPivot.Stomach:
				case IKPivot.Chest:	return true;
			}
			
			return false;
		}

		public void SetPivotRotation(IKPivot pivot, Quaternion rotation)
		{
			var bone = GetPivotTransform(pivot);

			if (bone != null)
			{
				CacheTargetsConstraints();
				bone.rotation = rotation;
				RestoreTargetsConstraints(pivot);
				ResetChainPivots(pivot);
			}
		}
		
		public void SetPivotLocalRotation(IKPivot pivot, Quaternion localRotation)
		{
			var bone = GetPivotTransform(pivot);

			if (bone != null)
			{
				CacheTargetsConstraints();
				bone.localRotation = localRotation;
				RestoreTargetsConstraints(pivot);
				ResetChainPivots(pivot);
			}
		}

		private void ResetChainPivots(IKPivot pivot)
		{
			var chain = GetRotatorAffectedChain(pivot);
			chain?.ResetPivots();
		}

		private IKChain GetRotatorAffectedChain(IKPivot pivot)
		{
			switch (pivot)
			{
				case IKPivot.Stomach:
				case IKPivot.Chest:			 return _spineChain;
				
				case IKPivot.Neck:
				case IKPivot.Head:			 return _headChain;
				
				case IKPivot.ClavicleLeft:
				case IKPivot.ArmLeft:
				case IKPivot.ForearmLeft:	 return _armLeftChain;
				
				case IKPivot.ClavicleRight:
				case IKPivot.ArmRight:
				case IKPivot.ForearmRight:	 return _armRightChain;
				
				case IKPivot.ThighLeft:
				case IKPivot.CalfLeft:	     return _legLeftChain;
				
				case IKPivot.ThighRight:
				case IKPivot.CalfRight:	     return _legRightChain;
				
				default: return null;
			}
		}

		public Vector3 GetPivotTransformPosition(IKPivot pivot)
		{
			var bone = GetPivotTransform(pivot);
			return bone != null ? bone.position : Vector3.zero;
		}

		public Quaternion GetPivotTransformRotation(IKPivot pivot)
		{
			var bone = GetPivotTransform(pivot);
			return bone != null ? bone.localRotation : Quaternion.identity;
		}

		public Transform GetPivotTransform(IKPivot pivot)
		{
			return _skeleton.GetBoneTransform(pivot);
		}

		public void SetPivotPosition(IKPivot pivot, Vector3 point)
		{
			switch (pivot)
			{
				case IKPivot.Pelvis:		SetPelvisPositionWithConstraints(point);                 break;
				case IKPivot.HeadEnd:	    _headChain.SetTargetPosition(point);	  break;
				case IKPivot.HeadPole:		_headChain.SetPolePosition(point);	      break;
				case IKPivot.Neck:          _spineChain.SetTargetPosition(point);	  break;
				case IKPivot.ChestPole:		_spineChain.SetPolePosition(point);       break;
				case IKPivot.HandLeft:	    _armLeftChain.SetTargetPosition(point);   break;
				case IKPivot.ArmPoleLeft:	_armLeftChain.SetPolePosition(point);	  break;
				case IKPivot.HandRight:     _armRightChain.SetTargetPosition(point);  break;
				case IKPivot.ArmPoleRight:	_armRightChain.SetPolePosition(point);	  break;
				case IKPivot.FootLeft:	    _legLeftChain.SetTargetPosition(point);   break;
				case IKPivot.LegPoleLeft:	_legLeftChain.SetPolePosition(point);	  break;
				case IKPivot.FootRight:     _legRightChain.SetTargetPosition(point);  break;
				case IKPivot.LegPoleRight:	_legRightChain.SetPolePosition(point);	  break;
			}
		}

		public Vector3 GetPivotPosition(IKPivot pivot)
		{
			switch (pivot)
			{
				case IKPivot.Pelvis:	   return _skeleton.Pelvis.localPosition;
				case IKPivot.HeadEnd:	   return _headChain.TargetPosition;
				case IKPivot.HeadPole:	   return _headChain.PolePosition;
				case IKPivot.Neck:         return _spineChain.TargetPosition;
				case IKPivot.ChestPole:	   return _spineChain.PolePosition;
				case IKPivot.HandLeft:	   return _armLeftChain.TargetPosition;
				case IKPivot.ArmPoleLeft:  return _armLeftChain.PolePosition;
				case IKPivot.HandRight:    return _armRightChain.TargetPosition;
				case IKPivot.ArmPoleRight: return _armRightChain.PolePosition;
				case IKPivot.FootLeft:	   return _legLeftChain.TargetPosition;
				case IKPivot.LegPoleLeft:  return _legLeftChain.PolePosition;
				case IKPivot.FootRight:    return _legRightChain.TargetPosition;
				case IKPivot.LegPoleRight: return _legRightChain.PolePosition;
				
				default: return Vector3.zero;
			}
		}

		public Transform GetPivotRootTransform(IKPivot pivot)
		{
			switch (pivot)
			{
				case IKPivot.Pelvis:		 return _skeleton.Pelvis.parent;
				case IKPivot.HeadEnd:
				case IKPivot.HeadPole:		 return _headChain.RootTransform;
				case IKPivot.Neck:
				case IKPivot.ChestPole:		 return _spineChain.RootTransform;
				case IKPivot.HandLeft:
				case IKPivot.ArmPoleLeft:	 return _armLeftChain.RootTransform;
				case IKPivot.HandRight:
				case IKPivot.ArmPoleRight:	 return _armRightChain.RootTransform;
				case IKPivot.FootLeft:
				case IKPivot.LegPoleLeft:	 return _legLeftChain.RootTransform;
				case IKPivot.FootRight:
				case IKPivot.LegPoleRight:	 return _legRightChain.RootTransform;
				
				default: return null;
			}
		}

		public List<Transform> GetAffectedBones(IKPivot pivot, IKTransformMode mode)
		{
			if (mode == IKTransformMode.Moving)
			{
				switch (pivot)
				{
					case IKPivot.Pelvis:		 return GetPelvisAffectedBones();
					case IKPivot.HeadEnd:
					case IKPivot.HeadPole:		 return _headChain.Bones;
					case IKPivot.Neck:
					case IKPivot.ChestPole:		 return _spineChain.Bones;
					case IKPivot.HandLeft:
					case IKPivot.ArmPoleLeft:	 return _armLeftChain.Bones;
					case IKPivot.HandRight:
					case IKPivot.ArmPoleRight:	 return _armRightChain.Bones;
					case IKPivot.FootLeft:
					case IKPivot.LegPoleLeft:	 return _legLeftChain.Bones;
					case IKPivot.FootRight:
					case IKPivot.LegPoleRight:	 return _legRightChain.Bones;
				}
			}
			else
			{
				switch (pivot)
				{
					case IKPivot.Pelvis:		 return GetPelvisAffectedBones();
					
					case IKPivot.Stomach:
					case IKPivot.Chest:			 return GetSpineAffectedBones();

					case IKPivot.ClavicleLeft:
					{
						if (_armLeftChain.ConstraintEnabled)
						{
							return _armLeftChain.Bones;
						}
						break;
					}

					case IKPivot.ClavicleRight:
					{
						if (_armRightChain.ConstraintEnabled)
						{
							return _armRightChain.Bones;
						}
						break;
					}
				}
			}
			
			return EmptyList;
		}

		private List<Transform> GetPelvisAffectedBones()
		{
			_temporaryHashSet.Clear();
			_temporaryHashSet.Add(_skeleton.Pelvis);
			
			if (_armLeftChain.ConstraintEnabled)  AddBonesToHashset(_armLeftChain.Bones);
			if (_armRightChain.ConstraintEnabled) AddBonesToHashset(_armRightChain.Bones);
			if (_legLeftChain.ConstraintEnabled)  AddBonesToHashset(_legLeftChain.Bones);
			if (_legRightChain.ConstraintEnabled) AddBonesToHashset(_legRightChain.Bones);

			return _temporaryHashSet.ToList();
		}

		private List<Transform> GetSpineAffectedBones()
		{
			_temporaryHashSet.Clear();
			_temporaryHashSet.Add(_skeleton.Pelvis);
			
			if (_armLeftChain.ConstraintEnabled)  AddBonesToHashset(_armLeftChain.Bones);
			if (_armRightChain.ConstraintEnabled) AddBonesToHashset(_armRightChain.Bones);

			return _temporaryHashSet.ToList();
		}

		private void AddBonesToHashset(List<Transform> bones)
		{
			foreach (var bone in bones)
			{
				_temporaryHashSet.SafeAdd(bone);
			}
		}

		public List<Transform> GetAllBones()
		{
			return _skeleton.GetAllBones();
		}

		public Transform GetPelvisTransform()
		{
			return _skeleton.Pelvis;
		}
		
		public void SelectWithChildren(IKPivot pivot)
		{
			var children = IKPivotConstants.GetPivotChildren(pivot);

			if (children != null)
			{
				_selectedPivots.AddRange(children);
			}
		}

		public void SwitchMirroring()
		{
			_enableMirroring = !_enableMirroring;
		}

		public void SwitchSelectionLock()
		{
			_lockSelection = !_lockSelection;
		}

		public bool HasPivot(IKPivot pivot)
		{
			var transform = _skeleton.GetBoneTransform(pivot);
			return transform != null;
		}
	}
}