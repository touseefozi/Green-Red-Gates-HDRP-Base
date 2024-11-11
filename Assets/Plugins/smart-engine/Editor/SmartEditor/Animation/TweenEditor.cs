using System.Collections;
using Smart.Animation;
using Smart.Animation.Base;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace SmartEditor.Animation
{
	[CustomEditor(typeof(TweenSingleBase), true)]
	public class TweenEditor : Editor
	{
		protected const string PlayAnimationLabel = "Play Animation";
		protected const string RewindAnimationLabel = "Rewind Animation";
		protected const string StopAnimationLabel = "Stop Animation";
		protected const string ScriptProperty = "m_Script";
		
		protected const float ButtonHeight = 40f;
		protected const float Space = 10f;
		
		private readonly GUILayoutOption _buttonLayout = GUILayout.Height(ButtonHeight);
		
		protected EditorCoroutine _coroutine;
		
		public override void OnInspectorGUI()
		{
			if (target is TweenSingleBase tween)
			{
				DrawProperties();
				GUILayout.Space(Space);

				if (tween.IsPlaybackAvailable)
				{
					if (_coroutine == null)
					{
						if (GUILayout.Button(PlayAnimationLabel, _buttonLayout))
						{
							StartAnimation();
						}
						if (GUILayout.Button(RewindAnimationLabel, _buttonLayout))
						{
							RewindAnimation();
						}
					}
					else if (GUILayout.Button(StopAnimationLabel, _buttonLayout))
					{
						StopAnimation();
					}
				}
			}
		}

		protected void DrawProperties()
		{
			serializedObject.Update();
			DrawPropertiesExcluding(serializedObject, ScriptProperty);
			serializedObject.ApplyModifiedProperties();
		}

		protected void StartAnimation()
		{
			_coroutine = EditorCoroutineUtility.StartCoroutine(AnimationCoroutine(), this);
		}
		
		protected void RewindAnimation()
		{
			var tween = (Tween) target;
			tween.SafeRewindToStart();
		}

		protected void StopAnimation()
		{
			var tween = (Tween) target;
			EditorCoroutineUtility.StopCoroutine(_coroutine);
			_coroutine = null;
			tween.SafeRewindToEnd();
			SetObjectsDirty();
		}

		protected IEnumerator AnimationCoroutine()
		{
			var tween = (Tween) target;
			yield return tween.AnimationCoroutine();
			SetObjectsDirty();
			_coroutine = null;
		}

		protected virtual void SetObjectsDirty()
		{
			var tween = (Tween) target;
			
			var gameObject = tween.GetGameObject();
			EditorUtility.SetDirty(gameObject);
		}

		protected void UpdateTweensFromChildren()
		{
			var group = (TweenGroup) target;
			group.UpdateTweensFromChildren();
			EditorUtility.SetDirty(group);
		}
	}
	
	[CustomEditor(typeof(TweenGroup))]
	public class TweenGroupEditor : TweenEditor
	{
		public override void OnInspectorGUI()
		{
			var buttonLayout = GUILayout.Height(ButtonHeight);
			var group = (TweenGroup) target;
			
			DrawProperties();
			GUILayout.Space(Space);
			
			if (_coroutine == null)
			{
				if (GUILayout.Button(PlayAnimationLabel, buttonLayout))
				{
					if (group.AutoUpdateChildren)
					{
						UpdateTweensFromChildren();
					}

					StartAnimation();
				}
				
				if (GUILayout.Button(RewindAnimationLabel, buttonLayout))
				{
					if (group.AutoUpdateChildren)
					{
						UpdateTweensFromChildren();
					}
					
					RewindAnimation();
				}
			}
			else if (GUILayout.Button(StopAnimationLabel, buttonLayout))
			{
				StopAnimation();
			}
		}
		
		protected override void SetObjectsDirty()
		{
			var group = (TweenGroup) target;
			
			foreach (var tween in group.Tweens)
			{
				var gameObject = tween.GetGameObject();
				EditorUtility.SetDirty(gameObject);
			}
		}
	}
}