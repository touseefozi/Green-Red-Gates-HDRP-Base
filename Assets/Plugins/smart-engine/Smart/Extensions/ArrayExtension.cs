using System;
using UnityEngine;

namespace Smart.Basics.Extensions
{
	public static class ArrayExtension
	{
		private const string CommaSeparator = ", ";
		
		public static T[] Append<T>(this T[] array, T item)
		{
			var length = array.Length;
			var result = new T[length + 1];
			array.CopyTo(result, 0);
			result[length] = item;
			return result;
		}

		public static int FindLastIndex<T>(this T[] array, Func<T, bool> predicate)
		{
			if (array == null) throw new ArgumentNullException(nameof(array));
			if (predicate == null) throw new ArgumentNullException(nameof(predicate));

			var index = -1;
			
			for (int i = 0; i < array.Length; i++)
			{
				var item = array[i];

				if (predicate(item))
				{
					index = i;
				}
			}

			return index;
		}

		public static T[] Concat<T>(this T[] arrayA, T[] arrayB)
		{
			if (arrayA == null) throw new ArgumentNullException(nameof(arrayA));
			if (arrayB == null) throw new ArgumentNullException(nameof(arrayB));
			int oldLength = arrayA.Length;
			Array.Resize(ref arrayA, arrayA.Length + arrayB.Length);
			Array.Copy(arrayB, 0, arrayA, oldLength, arrayB.Length);
			return arrayA;
		}

		public static string ElementsToString<T>(this T[] array)
		{
			return $"{typeof(T).Name}[] : {string.Join(CommaSeparator, array)}";
		}

		public static bool ContainsIndex<T>(this T[] array, int index)
		{
			return array != null && index >= 0 && index < array.Length;
		}

		public static T GetRandom<T>(this T[] array, int seed = 0)
		{
			var random = new System.Random(seed);
			var index = random.Next(0, array.Length);
			return array[index];
		}

		public static T SafeGetRandom<T>(this T[] array, T defaultValue = default, int seed = 0)
		{
			if (array.IsNullOrEmpty())
			{
				return defaultValue;
			}
			else if (array.Length == 1)
			{
				return array[0];
			}
			else
			{
				return array.GetRandom(seed);
			}
		}

		public static T SafeGet<T>(this T[] array, int index, T defaultValue)
		{
			if (array.IsNullOrEmpty())
			{
				return defaultValue;
			}

			return index >= 0 && index < array.Length ? array[index] : defaultValue;
		}

		public static T SafeGetClamped<T>(this T[] array, int index, T defaultValue)
		{
			return array.IsNullOrEmpty() ? defaultValue : array.GetClamped(index);
		}

		public static T GetClamped<T>(this T[] array, int index)
		{
			index = Mathf.Clamp(index, 0, array.Length - 1);
			return array[index];
		}

		public static T GetLooped<T>(this T[] array, int index)
		{
			var length = array.Length;

			if (length == 0)
			{
				return default;
			}
			else if (length == 1)
			{
				return array[0];
			}
			else
			{
				return array[index < 0 ? length - -index % length : index % length];
			}
		}

		public static bool IsNullOrEmpty<T>(this T[] array)
		{
			return array == null || array.Length == 0;
		}

		public static bool IsNotNullOrEmpty<T>(this T[] array)
		{
			return array != null && array.Length > 0;
		}

		public static int GetLastIndex<T>(this T[] array)
		{
			return array?.Length - 1 ?? -1;
		}

		public static T[] InsertAt<T>(this T[] array, int index)
		{
			if (index < 0)
			{
				Debug.LogError("Index is less than zero. Array is not modified");
				return array;
			}

			if (index > array.Length)
			{
				Debug.LogError("Index exceeds array length. Array is not modified");
				return array;
			}

			T[] newArray = new T[array.Length + 1];
			int index1 = 0;
			for (int index2 = 0; index2 < newArray.Length; ++index2)
			{
				if (index2 == index) continue;

				newArray[index2] = array[index1];
				++index1;
			}

			return newArray;
		}
	}
}