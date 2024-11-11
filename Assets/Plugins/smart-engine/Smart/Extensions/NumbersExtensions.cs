using UnityEngine;

namespace Smart.Extensions
{
	public static class NumbersExtensions
	{
		public static int RoundToInt(this float value)
		{
			return Mathf.RoundToInt(value);
		}
	}
}