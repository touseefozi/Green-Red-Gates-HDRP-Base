using Smart.Animation.Base;
using Smart.Animation.DataTypes;
using Smart.Attributes;
using TMPro;
using UnityEngine;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenNumericTextInt : TweenSingleBase
	{
		[SerializeField] private TweenIntValue _value;
		[SerializeField] private TMP_Text _text;
		
		private long _prevPrintedValue = -1;
		
		protected override void UpdateValue(float ratio)
		{
			var value = Mathf.CeilToInt(Mathf.Lerp(_value.Start, _value.End, ratio));

			if (value != _prevPrintedValue)
			{
				_text.text = value.ToString();
				_prevPrintedValue = value;
			}
		}
		
		public void SetValue(int start, int end)
		{
			_value.Start = start;
			_value.End = end;
		}

		public override GameObject GetGameObject()
		{
			return _text.gameObject;
		}
	}
}