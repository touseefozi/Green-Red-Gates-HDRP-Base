using System.Collections.Generic;

namespace Smart.Extensions
{
    public static class DictionaryExtensions
    {
        public static void SafeAdd<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
        	if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
        }
        
        public static void SafeRemove<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
        	if (dictionary.ContainsKey(key))
            {
                dictionary.Remove(key);
            }
        }
    }
}