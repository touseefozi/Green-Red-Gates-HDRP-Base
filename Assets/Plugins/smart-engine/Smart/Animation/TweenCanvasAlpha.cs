using Smart.Animation.Base;
using Smart.Attributes;
using UnityEngine;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenCanvasAlpha : TweenFloatBase
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		
		protected override void UpdateValue(float ratio)
		{
			_canvasGroup.alpha = GetValue(ratio);
		}

		public override GameObject GetGameObject()
		{
			return _canvasGroup.gameObject;
		}
	}
}