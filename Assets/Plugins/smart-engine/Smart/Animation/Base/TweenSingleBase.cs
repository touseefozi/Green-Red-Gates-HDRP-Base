using System;
using Smart.Animation.DataTypes;
using Smart.Attributes;
using UnityEngine;

namespace Smart.Animation.Base
{
	public abstract class TweenSingleBase : Tween
	{
		[HideInInspector] [SerializeField] private bool _isPlaybackAvailable;
		[SerializeField] protected EasingType _easing;
		
		[ConditionalField(nameof(_easing), false, EasingType.Curve)]
		[SerializeField] protected AnimationCurve _curve;
		
		[ConditionalField(nameof(_easing), false, EasingType.Curve)]
		[SerializeField] [Range(1, 25)] protected int _cycles = 1;
		
		[SerializeField] protected TweenTimingsValue _timings;
		
		public bool IsPlaybackAvailable => _isPlaybackAvailable;
		
		protected Func<float, float> _easingMethod;
		
		protected float _normalizedTime = -1f;

		protected void InitializeEasing()
		{
			_easingMethod = _easing == EasingType.Curve ? _curve.Evaluate : Easing.GetEasingMethod(_easing);
		}

		public override void UpdateTime(float time)
		{
			var totalDuration = GetDuration();
			var normalizedTime = 0f;
			
#if UNITY_EDITOR
			InitializeEasing();
#else
			if (_easingMethod == null)
			{
				InitializeEasing();
			}
#endif
			
			if (time >= _timings.Delay + GroupDelay)
			{
				if (time < totalDuration)
				{
					normalizedTime = (time - (_timings.Delay + GroupDelay)) / _timings.Duration % 1f;
				}
				else
				{
					normalizedTime = 1f;
				}
			}

			if (normalizedTime != _normalizedTime)
			{
				var ratio = _easingMethod(normalizedTime);
				UpdateValue(ratio);
				_normalizedTime = normalizedTime;
			}
		}

		public override float GetDuration()
		{
			return _timings.Delay + GroupDelay + _timings.Duration * _cycles;
		}
		
		[ContextMenu("Set Playback Enabled")]
		private void SetPlaybackEnabled()
		{
			_isPlaybackAvailable = true;
		}
		
		[ContextMenu("Set Playback Disabled")]
		private void SetPlaybackDisabled()
		{
			_isPlaybackAvailable = false;
		}

		protected abstract void UpdateValue(float ratio);
	}
}