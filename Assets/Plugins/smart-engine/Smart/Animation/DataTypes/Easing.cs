using System;
using UnityEngine;

namespace Smart.Animation.DataTypes
{
	public class Easing
	{
		private const float HALF_PI = (float) (Math.PI / 2f);
	
		public static float Interpolate(float startValue, float endValue, float currentTime, float totalTime, Func<float, float> easing)
		{
			return (endValue - startValue) * easing(currentTime / totalTime) + startValue;
		}
		
		public static Func<float, float> GetEasingMethod(EasingType type)
		{
			switch (type)
			{
				case EasingType.Linear:       return Linear;
				case EasingType.QuadIn:       return QuadIn;
				case EasingType.QuadOut:      return QuadOut;
				case EasingType.QuadInOut:    return QuadInOut;
				case EasingType.CubicIn:      return CubicIn;
				case EasingType.CubicOut:     return CubicOut;
				case EasingType.CubicInOut:   return CubicInOut;
				case EasingType.QuartIn:      return QuartIn;
				case EasingType.QuartOut:     return QuartOut;
				case EasingType.QuartInOut:   return QuartInOut;
				case EasingType.QuintIn:      return QuintIn;
				case EasingType.QuintOut:     return QuintOut;
				case EasingType.QuintInOut:   return QuintInOut;
				case EasingType.SineIn:       return SineIn;
				case EasingType.SineOut:      return SineOut;
				case EasingType.SineInOut:    return SineInOut;
				case EasingType.CircIn:       return CircIn;
				case EasingType.CircOut:      return CircOut;
				case EasingType.CircInOut:    return CircInOut;
				case EasingType.ExpoIn:       return ExpoIn;
				case EasingType.ExpoOut:      return ExpoOut;
				case EasingType.ExpoInOut:    return ExpoInOut;
				case EasingType.ElasticIn:    return ElasticIn;
				case EasingType.ElasticOut:   return ElasticOut;
				case EasingType.ElasticInOut: return ElasticInOut;
				case EasingType.BackIn:       return BackIn;
				case EasingType.BackOut:      return BackOut;
				case EasingType.SlowBackIn:   return SlowBackIn;
				case EasingType.SlowBackOut:  return SlowBackOut;
				case EasingType.BackInOut:    return BackInOut;
				case EasingType.BounceIn:     return BounceIn;
				case EasingType.BounceOut:    return BounceOut;
				case EasingType.BounceInOut:  return BounceInOut;
			}
			
			return Linear;
		}
	
		public static float Linear(float p)
		{
			return p;
		}
	
		public static float QuadIn(float p)
		{
			return p * p;
		}
	
		public static float QuadOut(float p)
		{
			return -(p * (p - 2));
		}
	
		public static float QuadInOut(float p)
		{
			if (p < 0.5f)
			{
				return 2 * p * p;
			}
			else
			{
				return (-2 * p * p) + (4 * p) - 1;
			}
		}
	
		public static float CubicIn(float p)
		{
			return p * p * p;
		}
	
		public static float CubicOut(float p)
		{
			float f = (p - 1);
			return f * f * f + 1;
		}
	
		public static float CubicInOut(float p)
		{
			if (p < 0.5f)
			{
				return 4 * p * p * p;
			}
			else
			{
				float f = ((2 * p) - 2);
				return 0.5f * f * f * f + 1;
			}
		}
	
		public static float QuartIn(float p)
		{
			return p * p * p * p;
		}
	
		public static float QuartOut(float p)
		{
			float f = (p - 1);
			return f * f * f * (1 - p) + 1;
		}
	
		public static float QuartInOut(float p) 
		{
			if (p < 0.5f)
			{
				return 8 * p * p * p * p;
			}
			else
			{
				float f = (p - 1);
				return -8 * f * f * f * f + 1;
			}
		}
	
		public static float QuintIn(float p) 
		{
			return p * p * p * p * p;
		}
	
		public static float QuintOut(float p) 
		{
			float f = (p - 1);
			return f * f * f * f * f + 1;
		}
	
		public static float QuintInOut(float p) 
		{
			if (p < 0.5f)
			{
				return 16 * p * p * p * p * p;
			}
			else
			{
				float f = ((2 * p) - 2);
				return 0.5f * f * f * f * f * f + 1;
			}
		}
	
		public static float SineIn(float p)
		{
			return Mathf.Sin((p - 1) * HALF_PI) + 1;
		}
	
		public static float SineOut(float p)
		{
			return Mathf.Sin(p * HALF_PI);
		}
	
		public static float SineInOut(float p)
		{
			return 0.5f * (1 - Mathf.Cos(p * Mathf.PI));
		}
	
		public static float CircIn(float p)
		{
			return 1 - Mathf.Sqrt(1 - (p * p));
		}
	
		public static float CircOut(float p)
		{
			return Mathf.Sqrt((2 - p) * p);
		}
	
		public static float CircInOut(float p)
		{
			if (p < 0.5f)
			{
				return 0.5f * (1 - Mathf.Sqrt(1 - 4 * (p * p)));
			}
			else
			{
				return 0.5f * (Mathf.Sqrt(-((2 * p) - 3) * ((2 * p) - 1)) + 1);
			}
		}
	
		public static float ExpoIn(float p)
		{
			return (p == 0.0f) ? p : Mathf.Pow(2, 10 * (p - 1));
		}
	
		public static float ExpoOut(float p)
		{
			return (p == 1.0f) ? p : 1 - Mathf.Pow(2, -10 * p);
		}
	
		public static float ExpoInOut(float p)
		{
			if (p == 0.0 || p == 1.0) return p;
			
			if (p < 0.5f)
			{
				return 0.5f * Mathf.Pow(2, (20 * p) - 10);
			}
			else
			{
				return -0.5f * Mathf.Pow(2, (-20 * p) + 10) + 1;
			}
		}
	
		public static float ElasticIn(float p)
		{
			return Mathf.Sin(13 * HALF_PI * p) * Mathf.Pow(2, 10 * (p - 1));
		}
	
		public static float ElasticOut(float p)
		{
			return Mathf.Sin(-13 * HALF_PI * (p + 1)) * Mathf.Pow(2, -10 * p) + 1;
		}
	
		public static float ElasticInOut(float p)
		{
			if (p < 0.5f)
			{
				return 0.5f * Mathf.Sin(13 * HALF_PI * (2 * p)) * Mathf.Pow(2, 10 * ((2 * p) - 1));
			}
			else
			{
				return 0.5f * (Mathf.Sin(-13 * HALF_PI * ((2 * p - 1) + 1)) * Mathf.Pow(2, -10 * (2 * p - 1)) + 2);
			}
		}
	
		public static float BackIn(float p)
		{
			return p * p * p - p * Mathf.Sin(p * Mathf.PI);
		}
	
		public static float BackOut(float p)
		{
			float f = 1 - p;
			return 1 - (f * f * f - f * Mathf.Sin(f * Mathf.PI));
		}
	
		public static float SlowBackIn(float p)
		{
			return p * p - p * Mathf.Sin(p * Mathf.PI);
		}
	
		public static float SlowBackOut(float p)
		{
			float f = 1 - p;
			return 1 - (f * f - f * Mathf.Sin(f * Mathf.PI));
		}
	
		public static float BackInOut(float p)
		{
			if (p < 0.5f)
			{
				float f = 2 * p;
				return 0.5f * (f * f * f - f * Mathf.Sin(f * Mathf.PI));
			}
			else
			{
				float f = (1 - (2*p - 1));
				return 0.5f * (1 - (f * f * f - f * Mathf.Sin(f * Mathf.PI))) + 0.5f;
			}
		}
	
		public static float BounceIn(float p)
		{
			return 1 - BounceOut(1 - p);
		}
	
		public static float BounceOut(float p)
		{
			if (p < 4/11.0f)
			{
				return (121 * p * p)/16.0f;
			}
			else if (p < 8/11.0f)
			{
				return (363/40.0f * p * p) - (99/10.0f * p) + 17/5.0f;
			}
			else if (p < 9/10.0f)
			{
				return (4356/361.0f * p * p) - (35442/1805.0f * p) + 16061/1805.0f;
			}
			else
			{
				return (54/5.0f * p * p) - (513/25.0f * p) + 268/25.0f;
			}
		}
	
		public static float BounceInOut(float p)
		{
			if (p < 0.5f)
			{
				return 0.5f * BounceIn(p*2);
			}
			else
			{
				return 0.5f * BounceOut(p * 2 - 1) + 0.5f;
			}
		}
	}
}