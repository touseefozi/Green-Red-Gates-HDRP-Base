using Smart.Animation.Base;
using Smart.Attributes;
using UnityEngine;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenSize : TweenRectTransformBase
	{
		protected override void UpdateValue(float ratio)
		{
			_transform.sizeDelta = GetValue(ratio);
		}

		public override GameObject GetGameObject()
		{
			return _transform.gameObject;
		}
	}
}