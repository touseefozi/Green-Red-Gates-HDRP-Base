using Smart.Animation.Base;
using Smart.Attributes;
using UnityEngine;
using UnityEngine.UI;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenImageAlpha : TweenFloatBase
	{
		[SerializeField] private Image _image;
		
		protected override void UpdateValue(float ratio)
		{
			var color = _image.color;
			color.a = GetValue(ratio);
			_image.color = color;
		}

		public override GameObject GetGameObject()
		{
			return _image.gameObject;
		}
	}
}