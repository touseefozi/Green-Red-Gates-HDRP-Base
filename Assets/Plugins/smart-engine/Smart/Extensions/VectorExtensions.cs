using UnityEngine;

namespace Smart.Basics.Extensions
{
	public static class VectorExtensions
	{
		public static bool Approximately(this Vector3 vector, Vector3 otherVector)
		{
			return Mathf.Approximately(vector.x, otherVector.x) && 
			       Mathf.Approximately(vector.y, otherVector.y) && 
			       Mathf.Approximately(vector.z, otherVector.z);
		}
		
		public static float GetMagnitude(this Vector2 vector)
		{
			if (vector.x == 0)
			{
				return vector.y == 0 ? 0f : Mathf.Abs(vector.y);
			}
			else if (vector.y == 0)
			{
				return vector.x == 0 ? 0f : Mathf.Abs(vector.x);
			}
			else
			{
				return vector.magnitude;
			}
		}
        
        public static Vector3 Invert(this Vector3 vector)
        {
        	return vector * -1f;
        }
        
        public static Vector3 ToScaleVector(this float value)
        {
        	return new Vector3(value, value, value);
        }
        
        public static Vector3 SetXY(this Vector3 vector, float x, float y)
        {
            vector.x = x;
            vector.y = y;
            return vector;
        }
	
        public static Vector3 SetX(this Vector3 vector, float x)
        {
            vector.x = x;
            return vector;
        }
	
        public static Vector3 SetY(this Vector3 vector, float y)
        {
            vector.y = y;
            return vector;
        }
	
        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            vector.z = z;
            return vector;
        }
	
        public static Vector3 AddX(this Vector3 vector, float x)
        {
            vector.x += x;
            return vector;
        }
	
        public static Vector3 AddY(this Vector3 vector, float y)
        {
            vector.y += y;
            return vector;
        }
	
        public static Vector3 AddZ(this Vector3 vector, float z)
        {
            vector.z += z;
            return vector;
        }
	
        public static Vector3 SubstractX(this Vector3 vector, float x)
        {
            vector.x -= x;
            return vector;
        }
	
        public static Vector3 SubstractY(this Vector3 vector, float y)
        {
            vector.y -= y;
            return vector;
        }
	
        public static Vector3 SubstractZ(this Vector3 vector, float z)
        {
            vector.z -= z;
            return vector;
        }
        
        public static Vector3 RotateTo(this Vector3 vector, float x, float y, float z)
        {
            return Quaternion.Euler(x, y, z) * vector;
        }
        
        public static Vector3 RotateToX(this Vector3 vector, float x)
        {
            return Quaternion.Euler(x, 0f, 0f) * vector;
        }
        
        public static Vector3 RotateToY(this Vector3 vector, float y)
        {
            return Quaternion.Euler(0f, y, 0f) * vector;
        }
        
        public static Vector3 RotateToZ(this Vector3 vector, float z)
        {
            return Quaternion.Euler(0f, 0f, z) * vector;
        }
	}
}