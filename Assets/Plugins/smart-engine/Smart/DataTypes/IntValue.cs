using System;

namespace Smart.DataTypes
{
    [Serializable]
    public class IntValue
    {
        public int Value;
        
        public IntValue(int value = 0)
        {
            Value = value;
        }
    }
}