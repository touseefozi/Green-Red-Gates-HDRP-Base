using Smart.Animation.Base;
using Smart.Attributes;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenPosition : TweenTransformBase
	{
		protected override void UpdateValue(float ratio)
		{
			_transform.localPosition = GetVectorValue(ratio);
		}
	}
}