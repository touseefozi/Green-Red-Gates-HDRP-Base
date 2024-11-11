using System.Collections.Generic;

namespace Smart.Extensions
{
	public static class HashSetExtensions
	{
		public static void SafeAdd<T>(this HashSet<T> hashSet, T item)
		{
			if (!hashSet.Contains(item))
			{
				hashSet.Add(item);
			}
		}
		
		public static void SafeRemove<T>(this HashSet<T> hashSet, T item)
		{
			if (hashSet.Contains(item))
			{
				hashSet.Remove(item);
			}
		}
	}
}