using System;

namespace Smart.DataTypes
{
    public class SmartException : Exception
    {
        public SmartException(object source, string message) : base($"[{source.GetType().Name}] {message}")
        {
        }
    }
}