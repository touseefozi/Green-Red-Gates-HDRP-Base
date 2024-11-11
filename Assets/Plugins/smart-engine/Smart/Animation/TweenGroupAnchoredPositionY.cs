using Smart.Animation.Base;
using UnityEngine;

namespace Smart.Animation
{
	public class TweenGroupAnchoredPositionY : TweenFloatBase
	{
		[SerializeField] protected float _groupItemDelay;
		[SerializeField] protected RectTransform[] _transforms;
		
		public override void UpdateTime(float time)
		{
			if (_easingMethod == null)
			{
				InitializeEasing();
			}

			for (int i = 0; i < _transforms.Length; i++)
			{
				var delay = _timings.Delay + i * _groupItemDelay;
				var duration = _timings.Delay + _timings.Duration + delay;
				
				if (time >= delay)
				{
					if (time < duration)
					{
						var ratio = _easingMethod((time - delay) / _timings.Duration);
						UpdateValue(i, ratio);
					}
					else
					{
						var ratio = _easingMethod(1f);
						UpdateValue(i, ratio);
					}
				}
				else
				{
					var ratio = _easingMethod(0f);
					UpdateValue(i, ratio);
				}
			}
		}
		
		protected void UpdateValue(int index, float ratio)
		{
			var transform = _transforms[index];
			var position = transform.anchoredPosition;
			position.y = GetValue(ratio);
			transform.anchoredPosition = position;
		}
		
		protected override void UpdateValue(float ratio)
		{
		}

		public override GameObject GetGameObject()
		{
			return gameObject;
		}

		public override float GetDuration()
		{
			return _timings.Delay + _timings.Duration + _groupItemDelay * (_transforms.Length - 1);
		}
	}
}