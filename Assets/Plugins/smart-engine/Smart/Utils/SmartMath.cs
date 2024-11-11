using Smart.Basics.Extensions;
using UnityEngine;

namespace Smart.Utils
{
	public static class SmartMath
	{
		private const float MaxAngle = 360f;
		
		public static float LerpAngleUnclamped(float startValue, float endValue, float ratio)
		{
			var delta = endValue - startValue;
			var value = delta - Mathf.Floor(delta / MaxAngle) * MaxAngle;

			if (value < 0f)
			{
				value = 0f;
			}
			else if (value > MaxAngle)
			{
				value = MaxAngle;
			}
			else if (value > MaxAngle / 2f)
			{
				value -= MaxAngle;
			}
			
			return startValue + value * ratio;
		}
		
		public static Vector3 GetVelocityToTarget(Vector3 origin, Vector3 target, float height, float gravity)
		{
            var deltaY = target.y - origin.y;
            var deltaXZ = (target - origin).SetY(0);
            var velocity = deltaXZ / (Mathf.Sqrt(-2f * height / gravity) + Mathf.Sqrt(2f * (deltaY - height) / gravity));
            velocity.y = Mathf.Sqrt(-2f * gravity * height);
            
            return velocity;
		}
        
        public static Vector3 GetIntersection(Vector3 originA, Vector3 directionA, Vector3 originB, Vector3 directionB)
        {
            float k = (directionA.x * (originA.y - originB.y) + directionA.y * (originB.x - originA.x))
                      / (directionB.y * directionA.x - directionA.y * directionB.x);
 
            if (float.IsNaN(k) || float.IsInfinity(k))
            {
                return Vector3.zero;
            }
            else
            {
                return new Vector3(originB.x + directionB.x * k, originB.y + directionB.y * k, originB.z + directionB.z * k);
            }
        }
        
        public static float FastSquareRoot(float value, float precision = 0.1f)
        {
	        var delta = 1f;
	        var prev = 0f;
	        var mid = value;
		
	        while (delta > precision)
	        {
		        prev = mid;
		        mid = (mid + value / mid) / 2;
		        delta = mid - prev;

		        if (delta < 0)
		        {
			        delta = -delta;
		        }
	        }
	        return mid;
        }

        public static float GetAngle(float x, float y)
        {
	        var vector = new Vector2(x, y).normalized;
	        return GetAngleFromNormal(vector.normalized);
        }

        public static float GetAngle(Vector2 vector)
        {
	        return GetAngleFromNormal(vector.normalized);
        }

        private static float GetAngleFromNormal(Vector2 normal)
        {
	        if (normal.x == 0f && normal.y == 0f)
	        {
		        return 0f;
	        }
	        
	        var angle = Mathf.Atan(normal.y / normal.x) * 180f / Mathf.PI;

	        if (normal.x < 0f)
	        {
		        angle += normal.y > 0f ? 180f : -180f;
	        }
	        
	        return angle;
        }
        
        public static Vector3 GetNormalByAngleY(float angle)
        {
	        angle *= Mathf.Deg2Rad;
	        
	        return new Vector3
	        {
		        x = Mathf.Cos(angle),
		        z = Mathf.Sin(angle),
	        };
        }
		
        public static float RandomNegative(float value = 1f)
        {
	        return RandomBoolean() ? value : -value;
        }
		
        public static bool RandomBoolean()
        {
	        return Random.value >= 0.5f;
        }

        public static float RoundTo(float value, float precision)
        {
	        return Mathf.Round(value * precision) / precision;
        }

        public static Vector3 RoundTo(Vector3 vector, float precision)
        {
	        vector.x = Mathf.Round(vector.x * precision) / precision;
	        vector.y = Mathf.Round(vector.y * precision) / precision;
	        vector.z = Mathf.Round(vector.z * precision) / precision;
	        
	        return vector;
        }
	}
}