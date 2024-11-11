using TMPro;

namespace Smart.Extensions
{
	public static class TextExtensions
	{
		public static void SetAlpha(this TMP_Text text, float value)
		{
			var color = text.color;
			color.a = value;
			text.color = color;
		}
	}
}