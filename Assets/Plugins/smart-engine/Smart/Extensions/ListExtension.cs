using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Smart.Basics.Extensions
{
	public static class ListExtensions
	{
		private const string CommaSeparator = ", ";
        
        public static bool ContainsIndex<T>(this List<T> list, int index)
        {
            return list != null && index >= 0 && index < list.Count;
        }
        
        public static List<T> Shuffle<T>(this List<T> list, int? seed = null)  
        {  
            var random = seed != null ? new System.Random(seed.Value) : new System.Random();
            var count = list.Count; 
            
            while (count-- > 1) 
            {  
                var index = random.Next(count + 1);  
                var value = list[index];  
                list[index] = list[count];  
                list[count] = value;  
            }
            
            return list;
        }
        
		public static T SafeGet<T>(this List<T> list, int index, T defaultValue)
		{
			if (list == null)
			{
				return defaultValue;
			}
			
			return index >= 0 && index < list.Count ? list[index] : defaultValue;
		}
		
		public static bool SafeRemove<T>(this List<T> list, T value)
		{
			if (list == null || !list.Contains(value))
			{
				return false;
			}
			
			return list.Remove(value);
		}
		
        public static T SafeGetClamped<T>(this List<T> list, int index, T defaultValue)
        {
            return list.IsNullOrEmpty() ? defaultValue : list.GetClamped(index);
        }
		
        public static T GetClamped<T>(this List<T> list, int index)
        {
            index = Mathf.Clamp(index, 0, list.Count - 1);
            
            return list[index];
        }
		
        public static T GetRandom<T>(this List<T> list, int seed = 0)
        {
            var random = seed == 0 ? new System.Random() : new System.Random(seed);
            var index = random.Next(0, list.Count);
            return list[index];
        }
		
        public static T SafeGetRandom<T>(this List<T> list, T defaultValue = default, int seed = 0)
        {
            return list.IsNullOrEmpty() ? defaultValue : list.GetRandom(seed);
        }
		
        public static T GetLooped<T>(this List<T> list, int index)
        {
            var count = list.Count;
            
            if (count == 0)
            {
                return default;
            }
            else if (count == 1)
            {
                return list[0];
            }
            else
            {
                return list[index < 0 ? count - -index % count : index % count];
            } 
        }
		
		public static void AddFirst<T>(this List<T> list, T item)
		{
			list.Insert(0, item);
		}
		
		public static T Pop<T>(this List<T> list)
		{
			if (list.Count > 0)
			{
				var index = list.Count - 1;
				var value = list[index];
				list.RemoveAt(index);
				return value;
			}
			
			return default;
		}
		
		public static T Shift<T>(this List<T> list)
		{
			if (list.Count > 0)
			{
				var value = list[0];
				list.RemoveAt(0);
				return value;
			}
			
			return default;
		}
		
		public static void DisposeAndClear<T>(this List<T> list) where T : IDisposable
		{
			foreach (var item in list)
			{
				item.Dispose();
			}
			
			list.Clear();
		}
		
		public static void SafeDisposeAndClear<T>(this List<T> list) where T : IDisposable
		{
			var items = list.ToArray();
			list.Clear();
			
			foreach (var item in items)
			{
				item.Dispose();
			}
		}
		
		public static string ElementsToString<T>(this List<T> list)
		{
			return $"List<{typeof(T).Name}> : {string.Join(CommaSeparator, list)}";
		}
		
		public static T Extract<T>(this List<T> list, int index)
		{
			Assert.IsTrue(index < list.Count);
			var item = list[index];
			list.RemoveAt(index);
			return item;
		}
	
		public static bool IsNotNullOrEmpty<T>(this List<T> list)
		{
			return list != null && list.Count > 0;
		}
	
		public static bool IsNullOrEmpty<T>(this List<T> list)
		{
			return list == null || list.Count == 0;
		}
		
        public static int GetLastIndex<T>(this List<T> list)
        {
            return list?.Count - 1 ?? -1;
        }
	}
}