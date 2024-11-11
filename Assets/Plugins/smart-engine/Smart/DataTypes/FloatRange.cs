using System;
using Smart.Extensions;
using UnityEngine;
using SystemRandom = System.Random;
using UnityRandom = UnityEngine.Random;

namespace Smart.DataTypes
{
    [Serializable]
    public struct FloatRange
    {
        public float Min;
        public float Max;
        
        public FloatRange(float min, float max)
        {
            Min = min;
            Max = max;
        }
        
        public float Clamp(float value)
        {
            if (value < Min)
            {
                return Min;
            }
            else if (value > Max)
            {
                return Max;
            }
            
            return value;
        }
        
        public bool Contains(float value)
        {
        	return value >= Min && value <= Max;
        }
        
        public float InverseLerpClamped(float value)
        {
            return (Clamp(value) - Min) / (Max - Min);
        }
       
        public float InverseLerp(float value)
        {
            return (value - Min) / (Max - Min);
        }

        public float Evaluate(float time)
        {
            return Min + (Max - Min) * time;
        }

        public float EvaluateClamped(float time)
        {
            return Min + (Max - Min) * Mathf.Clamp01(time);
        }

        public float GetRandomValue(SystemRandom randomizer)
        {
            var time = randomizer.NextFloat();
            return Evaluate(time);
        }

        public float GetRandomValue(int seed)
        {
            UnityRandom.InitState(seed);
            return UnityRandom.Range(Min, Max);
        }

        public float GetRandomValue()
        {
            return UnityRandom.Range(Min, Max);
        }
        
        // *** OBSOLETE *** //
        
        [Obsolete("Use InverseLerpClamped() method.")]
        public float GetClampedPosition(float value)
        {
            return InverseLerpClamped(value);
        }
        
        [Obsolete("Use InverseLerp() method.")]
        public float GetPosition(float value)
        {
            return InverseLerp(value);
        }
        
        [Obsolete("Use Evaluate() method.")]
        public float GetValueByPosition(float time)
        {
            return Evaluate(time);
        }
        
        [Obsolete("Use EvaluateClamped() method.")]
        public float Lerp(float time)
        {
            return EvaluateClamped(time);
        }
    }
}