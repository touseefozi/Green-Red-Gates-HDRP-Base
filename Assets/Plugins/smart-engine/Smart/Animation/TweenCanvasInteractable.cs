using Smart.Animation.Base;
using Smart.Attributes;
using UnityEngine;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenCanvasInteractable : Tween
	{
		[SerializeField] private float _duration;
		[SerializeField] private bool _targetValue;
		[SerializeField] private CanvasGroup _canvasGroup;
		
		protected float _normalizedTime = -1f;
		
		public override void UpdateTime(float time)
		{
			var normalizedTime = time < _duration ? time / _duration : 1f;

			if (normalizedTime != _normalizedTime)
			{
				_normalizedTime = normalizedTime;
				var value = _normalizedTime == 1f ? _targetValue : !_targetValue;
				_canvasGroup.interactable = value;
				_canvasGroup.blocksRaycasts = value;
			}
		}

		public override float GetDuration()
		{
			return _duration;
		}

		public override GameObject GetGameObject()
		{
			return _canvasGroup.gameObject;
		}
		
	}
}