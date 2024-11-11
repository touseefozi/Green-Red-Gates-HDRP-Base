using Smart.Animation.DataTypes;
using UnityEngine;

namespace Smart.Animation.Base
{
	public abstract class TweenFloatBase : TweenSingleBase
	{
		[SerializeField] protected TweenFloatValue _value;
		
		public float GetValue(float ratio)
		{
			return (_value.End - _value.Start) * ratio + _value.Start;
		}
	}
}