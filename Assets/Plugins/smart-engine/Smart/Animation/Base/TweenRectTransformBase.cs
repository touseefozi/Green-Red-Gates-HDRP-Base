using UnityEngine;

namespace Smart.Animation.Base
{
	public abstract class TweenRectTransformBase : TweenSingleBase
	{
		[SerializeField] protected Vector2 _startValue;
		[SerializeField] protected Vector2 _endValue;
		[SerializeField] protected RectTransform _transform;
		
		public Vector2 GetValue(float ratio)
		{
			return Vector2.LerpUnclamped(_startValue, _endValue, ratio);
		}

		public override GameObject GetGameObject()
		{
			return _transform.gameObject;
		}
	}
}