using System;

namespace Smart.DataTypes
{
    [Serializable]
    public class FloatValue
    {
        public float Value;
        
        public FloatValue(float value = 0f)
        {
            Value = value;
        }

        public void SetValue(float value)
        {
            Value = value;
        }
    }
}