using System;
using Smart.Extensions;
using UnityEngine;
using SystemRandom = System.Random;
using UnityRandom = UnityEngine.Random;

namespace Smart.DataTypes
{
    [Serializable]
    public struct IntRange
    {
        public int Min;
        public int Max;
        
        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }
        
        public int Clamp(int value)
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
        
        public bool Contains(int value)
        {
            return value >= Min && value <= Max;
        }
        
        public float InverseLerpClamped(int value)
        {
            return (Clamp(value) - Min) / (float) (Max - Min);
        }
        
        public float InverseLerp(int value)
        {
            return (value - Min) / (float) (Max - Min);
        }

        public int Evaluate(float time)
        {
            return Mathf.RoundToInt(Min + (Max - Min) * time);
        }
        
        public float EvaluateClamped(float time)
        {
            return Min + (Max - Min) * Mathf.Clamp01(time);
        }

        public int GetRandomValue(SystemRandom randomizer)
        {
            var time = randomizer.NextFloat();
            return Evaluate(time);
        }

        public int GetRandomValue(int seed)
        {
            UnityRandom.InitState(seed);
            return Evaluate(UnityRandom.value);
        }

        public int GetRandomValue()
        {
            return Evaluate(UnityRandom.value);
        }

        [Obsolete("Use GetRandomValue()")]
        public int GetRandomIntValue(SystemRandom randomizer)
        {
            return GetRandomValue(randomizer);
        }
    }
}