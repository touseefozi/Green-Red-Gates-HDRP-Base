using UnityEngine;

namespace Smart.Extensions
{
	public static class RectTransformExtensions
	{
		public static void SetSize(this RectTransform transform, float width, float height)
		{
			transform.sizeDelta = new Vector3(width, height);
		}
		
		public static void SetWidth(this RectTransform transform, float value)
		{
			var size = transform.sizeDelta;
			transform.sizeDelta = new Vector3(value, size.y);
		}
		
		public static void SetHeight(this RectTransform transform, float value)
		{
			var size = transform.sizeDelta;
			transform.sizeDelta = new Vector3(size.x, value);
		}
		
		public static float GetWidth(this RectTransform transform)
		{
			return transform.sizeDelta.x;
		}
		
		public static float GetHeight(this RectTransform transform)
		{
			return transform.sizeDelta.y;
		}
	}
}