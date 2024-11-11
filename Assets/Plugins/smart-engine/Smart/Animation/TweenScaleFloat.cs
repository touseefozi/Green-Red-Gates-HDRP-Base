using Smart.Animation.Base;
using Smart.Attributes;
using UnityEngine;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenScaleFloat : TweenFloatBase
	{
		[SerializeField] protected RectTransform _transform;
		
		protected override void UpdateValue(float ratio)
		{
			var scale = GetValue(ratio);
			_transform.localScale = new Vector3(scale, scale, scale);
		}

		public override GameObject GetGameObject()
		{
			return _transform.gameObject;
		}
	}
}