using System;
using System.Collections.Generic;
using System.Linq;

namespace Smart.Extensions
{
    public static class EnumExtensions
    {
        public static List<T> GetEnumList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }
        
        public static T NextEnum<T>(this T source) where T : struct
        {
            var type = source.GetType();
            
            if (type.IsEnum)
            {
                var values = Enum.GetValues(type).Cast<T>().ToList();
                var index = values.IndexOf(source) + 1;
                return index == values.Count ? values[0] : values[index];     
            }
            else
            {
                throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
            }
        }
        
        public static T PrevEnum<T>(this T source) where T : struct
        {
            var type = source.GetType();
            
            if (type.IsEnum)
            {
                var values = Enum.GetValues(type).Cast<T>().ToList();
                var index = values.IndexOf(source) - 1;
                return index < 0 ? values[values.Count - 1] : values[index];     
            }
            else
            {
                throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));
            }
        }
    }
}