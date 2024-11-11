using Smart.Animation.Base;
using Smart.Attributes;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenScale : TweenTransformBase
	{
		protected override void UpdateValue(float ratio)
		{
			_transform.localScale = GetVectorValue(ratio);
		}
	}
}