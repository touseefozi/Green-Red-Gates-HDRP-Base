using Smart.Animation.Base;
using TMPro;
using UnityEngine;

namespace Smart
{
	public class TweenTextAlpha : TweenFloatBase
	{
		[SerializeField] private TMP_Text _text;
		
		protected override void UpdateValue(float ratio)
		{
			var color = _text.color;
			color.a = GetValue(ratio);
			_text.color = color;
		}

		public override GameObject GetGameObject()
		{
			return _text.gameObject;
		}
	}
}