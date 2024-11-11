using System;
using System.Collections;
using UnityEngine;

namespace Smart.Animation.Base
{
	public abstract class Tween : MonoBehaviour
	{
		private const float	MinFrameDuration = 1f / 60f;
		
		internal float GroupDelay;
		
		public abstract void UpdateTime(float time);
		public abstract float GetDuration();
		public abstract GameObject GetGameObject();

		internal virtual void OnStartPlaying()
		{
		}

		public void RewindToStart()
		{
			UpdateTime(0f);
		}

		public void RewindToEnd()
		{
			var duration = GetDuration();
			UpdateTime(duration);
		}

		public IEnumerator AnimationCoroutine(Action onComplete = null)
		{
			OnStartPlaying();
			
			var time = 0f;
			var duration = GetDuration();
			
			RewindToStart();

			while (true)
			{
				if (time > duration)
				{
					time = duration;
				}

				UpdateTime(time);

				if (time == duration)
				{
					onComplete?.Invoke();
					yield break;
				}
				
				var delta = Time.unscaledDeltaTime;
#if UNITY_EDITOR
				if (!Application.isPlaying && delta > MinFrameDuration)
				{
					delta = MinFrameDuration;
				}
#endif
				time += delta;
				yield return null;
			}
		}

		public IEnumerator ReversedAnimationCoroutine(Action onComplete = null)
		{
			OnStartPlaying();
			
			var duration = GetDuration();
			var time = duration;
			
			RewindToEnd();

			while (true)
			{
				if (time < 0f)
				{
					time = 0f;
				}

				UpdateTime(time);

				if (time == 0f)
				{
					onComplete?.Invoke();
					yield break;
				}
				
				var delta = Time.unscaledDeltaTime;
#if UNITY_EDITOR
				if (!Application.isPlaying && delta > MinFrameDuration)
				{
					delta = MinFrameDuration;
				}
#endif
				time -= delta;
				yield return null;
			}
		}
	}
}