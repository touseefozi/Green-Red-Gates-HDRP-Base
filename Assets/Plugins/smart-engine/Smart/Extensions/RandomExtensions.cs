using UnityEngine;
using Random = System.Random;

namespace Smart.Extensions
{
    public static class RandomExtensions
    {
        public static float NextFloat(this Random random)
        {
        	return (float) random.NextDouble();
        }
        
        public static int NextInt(this Random random, int minInclusive, int maxInclusive)
        {
            var value = random.NextFloat(minInclusive, maxInclusive);
            return Mathf.RoundToInt(value);
        }
        
        public static float NextFloat(this Random random, float minInclusive, float maxInclusive)
        {
            return (float) random.NextDouble() * (maxInclusive - minInclusive) + minInclusive;
        }
    }
}