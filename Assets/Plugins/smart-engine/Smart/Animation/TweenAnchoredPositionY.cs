using Smart.Animation.Base;
using Smart.Attributes;
using UnityEngine;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenAnchoredPositionY : TweenFloatBase
	{
		[SerializeField] protected RectTransform _transform;
		
		protected override void UpdateValue(float ratio)
		{
			var position = _transform.anchoredPosition;
			position.y = GetValue(ratio);
			_transform.anchoredPosition = position;
		}

		public override GameObject GetGameObject()
		{
			return _transform.gameObject;
		}
	}
}