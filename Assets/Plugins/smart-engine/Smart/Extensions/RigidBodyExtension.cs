using UnityEngine;

namespace Smart.Extensions
{
    public static class RigidBodyExtension
    {
        // Set Position
	
        public static void SetXY(this Rigidbody rigidbody, float x, float y)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(x, y, pos.z);
        }
	
        public static void SetXZ(this Rigidbody rigidbody, float x, float z)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(x, pos.y, z);
        }
	
        public static void SetX(this Rigidbody rigidbody, float x)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(x, pos.y, pos.z);
        }
	
        public static void SetY(this Rigidbody rigidbody, float y)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(pos.x, y, pos.z);
        }
	
        public static void SetZ(this Rigidbody rigidbody, float z)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(pos.x, pos.y, z);
        }
        
        // Add Position
	
        public static void AddXY(this Rigidbody rigidbody, float x, float y)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(pos.x + x, pos.y + y, pos.z);
        }
	
        public static void AddX(this Rigidbody rigidbody, float x)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(pos.x + x, pos.y, pos.z);
        }
	
        public static void AddY(this Rigidbody rigidbody, float y)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(pos.x, pos.y + y, pos.z);
        }
	
        public static void AddZ(this Rigidbody rigidbody, float z)
        {
            var pos = rigidbody.position;
            rigidbody.position = new Vector3(pos.x, pos.y, pos.z + z);
        }
        
        // Add Rotation
	
        public static void AddRotationX(this Rigidbody rigidbody, float x)
        {
            var rotation = rigidbody.rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Euler(rotation.x + x, rotation.y, rotation.z);
        }
	
        public static void AddRotationY(this Rigidbody rigidbody, float y)
        {
            var rotation = rigidbody.rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Euler(rotation.x, rotation.y + y, rotation.z);
        }
	
        public static void AddRotationZ(this Rigidbody rigidbody, float z)
        {
            var rotation = rigidbody.rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z + z);
        }
	
        // Set Rotation
	
        public static void SetRotation(this Rigidbody rigidbody, Vector3 rotation)
        {
            rigidbody.rotation = Quaternion.Euler(rotation);
        }
	
        public static void SetRotation(this Rigidbody rigidbody, float x, float y, float z)
        {
            rigidbody.rotation = Quaternion.Euler(x, y, z);
        }
	
        public static void SetRotationXY(this Rigidbody rigidbody, float x, float y)
        {
            var rotation = rigidbody.rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Euler(x, y, rotation.z);
        }
	
        public static void SetRotationX(this Rigidbody rigidbody, float x)
        {
            var rotation = rigidbody.rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Euler(x, rotation.y, rotation.z);
        }
	
        public static void SetRotationY(this Rigidbody rigidbody, float y)
        {
            var rotation = rigidbody.rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Euler(rotation.x, y, rotation.z);
        }
	
        public static void SetRotationZ(this Rigidbody rigidbody, float z)
        {
            var rotation = rigidbody.rotation.eulerAngles;
            rigidbody.rotation = Quaternion.Euler(rotation.x, rotation.y, z);
        }
	
        // Get Rotation
        
        public static Vector3 GetRotation(this Rigidbody rigidbody)
        {
            return rigidbody.rotation.eulerAngles;
        }
	
        public static float GetRotationX(this Rigidbody rigidbody)
        {
            return rigidbody.rotation.eulerAngles.x;
        }
	
        public static float GetRotationY(this Rigidbody rigidbody)
        {
            return rigidbody.rotation.eulerAngles.y;
        }
	
        public static float GetRotationZ(this Rigidbody rigidbody)
        {
            return rigidbody.rotation.eulerAngles.z;
        }
    }
}