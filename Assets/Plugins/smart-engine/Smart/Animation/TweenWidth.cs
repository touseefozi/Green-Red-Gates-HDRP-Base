using Smart.Animation.Base;
using Smart.Attributes;
using UnityEngine;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenWidth : TweenFloatBase
	{
		[SerializeField] protected RectTransform _transform;

		protected override void UpdateValue(float ratio)
		{
			var size = _transform.sizeDelta;
			size.x = GetValue(ratio);
			_transform.sizeDelta = size;
		}

		public override GameObject GetGameObject()
		{
			return _transform.gameObject;
		}
	}
}