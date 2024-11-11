using System.Collections.Generic;

namespace Smart.Extensions
{
    public static class EnumeratorExt
    {
        public static T GetNextValue<T>(this IEnumerator<T> enumerator)
        {
            return enumerator.MoveNext() ? enumerator.Current : default;
        }
        
        public static T GetNextValueLooped<T>(this IEnumerator<T> enumerator)
        {
        	if (!enumerator.MoveNext())
            {
                enumerator.Reset();
                enumerator.MoveNext();
            }
            
            return enumerator.Current;
        }
    }
}