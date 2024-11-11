using UnityEngine;

namespace Smart.Extensions
{
    public static class UnityExtensions
    {
        public static bool IsNull(this Object @object)
        {
        	return ReferenceEquals(@object, null);
        }
        
        public static bool IsNotNull(this Object @object)
        {
        	return !ReferenceEquals(@object, null);
        }
    }
}