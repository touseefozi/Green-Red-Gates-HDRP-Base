using Smart.Animation.Base;

namespace Smart.Animation
{
	public static class TweenExtensions
	{
		public static void SafeRewindToStart(this Tween tween)
		{
			if (tween != null)
			{
				tween.UpdateTime(0f);
			}
		}
		
		public static void SafeRewindToEnd(this Tween tween)
		{
			if (tween != null)
			{
				var duration = tween.GetDuration();
				tween.UpdateTime(duration);
			}
		}
	}
}