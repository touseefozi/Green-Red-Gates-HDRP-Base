using System.Collections.Generic;
using System.Linq;
using Smart.Animation.Base;
using UnityEngine;

namespace Smart.Animation
{
	public class TweenGroup : Tween
	{
		[SerializeField] private bool _autoUpdateChildren;
		[SerializeField] private float _timeScale = 1f;
		[SerializeField] private List<Tween> _tweens;
		[SerializeField] protected float _groupItemDelay;

		public bool AutoUpdateChildren => _autoUpdateChildren;
		public List<Tween> Tweens => _tweens;
		
		private float _normalizedTime = -1f;
		private float _duration;

		public void UpdateTweensFromChildren()
		{
			_tweens = GetComponentsInChildren<Tween>().Where(tween => tween != this).ToList();
		}
		
		internal override void OnStartPlaying()
		{
			var totalTweens = _tweens.Count;
			
			for (int i = 0; i < totalTweens; i++)
			{
				var tween =  _tweens[i];
				tween.GroupDelay = i * _groupItemDelay;
				tween.OnStartPlaying();
			}
			
			_duration = GetDuration();
		}

		public override void UpdateTime(float time)
		{
			var normalizedTime = 0f;
			
			if (time >= GroupDelay)
			{
				if (time < _duration)
				{
					normalizedTime = (time - GroupDelay) / (_duration - GroupDelay);
				}
				else
				{
					normalizedTime = 1f;
				}
			}

			if (normalizedTime != _normalizedTime)
			{
				time = normalizedTime * (_duration - GroupDelay) * _timeScale; 
				
				for (int i = 0; i < _tweens.Count; i++)
				{
					_tweens[i].UpdateTime(time);
				}
				
				_normalizedTime = normalizedTime;
			}
		}

		public override float GetDuration()
		{
			var totalTweens = _tweens.Count;
			var result = 0f;
			
			for (int i = 0; i < totalTweens; i++)
			{
				var tween = _tweens[i];
				var duration = tween.GetDuration();
				
				if (duration > result)
				{
					result = duration;
				}
			}
			
			return result / _timeScale + GroupDelay;
		}

		public override GameObject GetGameObject()
		{
			return gameObject;
		}
	}
}