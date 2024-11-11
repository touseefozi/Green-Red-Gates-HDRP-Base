using Smart.Animation.Base;
using UnityEngine;

namespace Smart.Animation
{
	public class TweenShakeRotationZ : TweenSingleBase
	{
		[SerializeField] private float _amplitude = 10;
		[SerializeField, Range(0, 10)] private int _shakeCycles = 4;
		[SerializeField] protected Transform _transform;
		
		protected override void UpdateValue(float ratio)
		{
			var rotation = _transform.localRotation.eulerAngles;
			rotation.z = _amplitude * Mathf.Sin(Mathf.PI * _shakeCycles * ratio) * (1f - ratio);
		
			_transform.localRotation = Quaternion.Euler(rotation);
		}

		public override GameObject GetGameObject()
		{
			return _transform.gameObject;
		}
	}
}