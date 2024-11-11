using Smart.Animation.Base;
using Smart.Attributes;

namespace Smart.Animation
{
	[HideScriptField]
	public class TweenAnchoredPosition : TweenRectTransformBase
	{
		protected override void UpdateValue(float ratio)
		{
			_transform.anchoredPosition = GetValue(ratio);
		}
	}
}