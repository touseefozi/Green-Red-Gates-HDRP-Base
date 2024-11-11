using Smart.Animation.Base;
using Smart.Animation.DataTypes;
using Smart.Attributes;
using TMPro;
using UnityEngine;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenNumericTextLong : TweenSingleBase
	{
		[SerializeField] private TweenLongValue _value;
		[SerializeField] private TMP_Text _text;
		
		private long _prevPrintedValue = -1;
		
		protected override void UpdateValue(float ratio)
		{
			var value = (long) Mathf.Ceil(Mathf.Lerp(_value.Start, _value.End, ratio));

			if (value != _prevPrintedValue)
			{
				_text.text = value.ToString();
				_prevPrintedValue = value;
			}
		}
		
		public void SetValue(long start, long end)
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