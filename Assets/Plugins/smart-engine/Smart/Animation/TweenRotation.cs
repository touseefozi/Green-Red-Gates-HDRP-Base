using Smart.Animation.Base;
using Smart.Attributes;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenRotation : TweenTransformBase
	{
		protected override void UpdateValue(float ratio)
		{
			_transform.localRotation = GetQuaternionValue(ratio);
		}
	}
}