using Smart.Utils;
using UnityEngine;

namespace Smart.Animation.Base
{
	public abstract class TweenTransformBase : TweenSingleBase
	{
		[SerializeField] protected Vector3 _startValue;
		[SerializeField] protected Vector3 _endValue;
		[SerializeField] protected Transform _transform;
		
		public Vector3 GetVectorValue(float ratio)
		{
			return Vector3.LerpUnclamped(_startValue, _endValue, ratio);
		}
		
		public Quaternion GetQuaternionValue(float ratio)
		{
			var rotation = new Vector3
			{
				x = SmartMath.LerpAngleUnclamped(_startValue.x, _endValue.x, ratio),
				y = SmartMath.LerpAngleUnclamped(_startValue.y, _endValue.y, ratio),
				z = SmartMath.LerpAngleUnclamped(_startValue.z, _endValue.z, ratio),
			};
			
			return Quaternion.Euler(rotation);
		}

		public override GameObject GetGameObject()
		{
			return _transform.gameObject;
		}
	}
}