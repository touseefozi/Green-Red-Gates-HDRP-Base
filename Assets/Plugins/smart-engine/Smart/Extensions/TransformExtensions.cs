using UnityEngine;

namespace Smart.Extensions
{
	public static class TransformExtensions
	{
		public static void RemoveChildren(this Transform transform)
		{
			var isEditor = Application.isEditor;
			
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				var child = transform.GetChild(i);
				
				if (isEditor)
				{
					Object.DestroyImmediate(child.gameObject);
				}
				else
				{
					Object.Destroy(child.gameObject);
				}
			}
		}
		
        // Set Scale
	
		public static void SetScaleXY(this Transform transform, float scaleX, float scaleY)
		{
			var scale = transform.localScale;
			transform.localScale = new Vector3(scaleX, scaleY, scale.z);
		}
	
		public static void SetScale(this Transform transform, float value)
		{
			transform.localScale = new Vector3(value, value, value);
		}
        
		public static void SetScale(this Transform transform, float x, float y, float z)
		{
			transform.localScale = new Vector3(x, y, z);
		}
	
		public static void SetScaleX(this Transform transform, float scaleX)
		{
			var scale = transform.localScale;
			transform.localScale = new Vector3(scaleX, scale.y, scale.z);
		}
	
		public static void SetScaleY(this Transform transform, float scaleY)
		{
			var scale = transform.localScale;
			transform.localScale = new Vector3(scale.x, scaleY, scale.z);
		}
	
		public static void SetScaleZ(this Transform transform, float scaleZ)
		{
			var scale = transform.localScale;
			transform.localScale = new Vector3(scale.x, scale.y, scaleZ);
		}
        
		public static float GetScale(this Transform transform)
		{
			return transform.localScale.x;
		}
        
        // Set Position
	
        public static void SetXY(this Transform transform, float x, float y)
        {
            var pos = transform.position;
            transform.position = new Vector3(x, y, pos.z);
        }
	
        public static void SetXZ(this Transform transform, float x, float z)
        {
            var pos = transform.position;
            transform.position = new Vector3(x, pos.y, z);
        }
	
        public static void SetX(this Transform transform, float x)
        {
            var pos = transform.position;
            transform.position = new Vector3(x, pos.y, pos.z);
        }
	
        public static void SetY(this Transform transform, float y)
        {
            var pos = transform.position;
            transform.position = new Vector3(pos.x, y, pos.z);
        }
	
        public static void SetZ(this Transform transform, float z)
        {
            var pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, z);
        }
	
		public static void SetLocalXY(this Transform transform, float x, float y)
		{
			var pos = transform.localPosition;
			transform.localPosition = new Vector3(x, y, pos.z);
		}
	
		public static void SetLocalXZ(this Transform transform, float x, float z)
		{
			var pos = transform.localPosition;
			transform.localPosition = new Vector3(x, pos.y, z);
		}
	
		public static void SetLocalX(this Transform transform, float x)
		{
			var pos = transform.localPosition;
			transform.localPosition = new Vector3(x, pos.y, pos.z);
		}
	
		public static void SetLocalY(this Transform transform, float y)
		{
			var pos = transform.localPosition;
			transform.localPosition = new Vector3(pos.x, y, pos.z);
		}
	
		public static void SetLocalZ(this Transform transform, float z)
		{
			var pos = transform.localPosition;
			transform.localPosition = new Vector3(pos.x, pos.y, z);
		}
		
		// Anchored Position

		public static void SetAnchoredXY(this RectTransform transform, float x, float y)
		{
			transform.anchoredPosition = new Vector2(x, y);
		}

		public static void SetAnchoredX(this RectTransform transform, float x)
		{
			var pos = transform.anchoredPosition;
			transform.anchoredPosition = new Vector2(x, pos.y);
		}
	
		public static void SetAnchoredY(this RectTransform transform, float y)
		{
			var pos = transform.anchoredPosition;
			transform.anchoredPosition = new Vector2(pos.x, y);
		}
        
        // Add Position
	
        public static void AddXY(this Transform transform, float x, float y)
        {
            var pos = transform.position;
            transform.position = new Vector3(pos.x + x, pos.y + y, pos.z);
        }
	
        public static void AddX(this Transform transform, float x)
        {
            var pos = transform.position;
            transform.position = new Vector3(pos.x + x, pos.y, pos.z);
        }
	
        public static void AddY(this Transform transform, float y)
        {
            var pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y + y, pos.z);
        }
	
        public static void AddZ(this Transform transform, float z)
        {
            var pos = transform.position;
            transform.position = new Vector3(pos.x, pos.y, pos.z + z);
        }
	
		public static void AddLocalXY(this Transform transform, float x, float y)
		{
			var pos = transform.localPosition;
			transform.localPosition = new Vector3(pos.x + x, pos.y + y, pos.z);
		}
	
		public static void AddLocalX(this Transform transform, float x)
		{
			var pos = transform.localPosition;
			transform.localPosition = new Vector3(pos.x + x, pos.y, pos.z);
		}
	
		public static void AddLocalY(this Transform transform, float y)
		{
			var pos = transform.localPosition;
			transform.localPosition = new Vector3(pos.x, pos.y + y, pos.z);
		}
	
        public static void AddLocalZ(this Transform transform, float z)
        {
            var pos = transform.localPosition;
            transform.localPosition = new Vector3(pos.x, pos.y, pos.z + z);
        }
        
        // Add Rotation
	
        public static void AddRotationX(this Transform transform, float x)
        {
            var rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x + x, rotation.y, rotation.z);
        }
	
        public static void AddRotationY(this Transform transform, float y)
        {
            var rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y + y, rotation.z);
        }
	
        public static void AddRotationZ(this Transform transform, float z)
        {
            var rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z + z);
        }
	
        public static void AddLocalRotation(this Transform transform, Vector3 rotation)
        {
            transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles + rotation);
        }
	
        public static void AddLocalRotationX(this Transform transform, float x)
        {
            var rotation = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(rotation.x + x, rotation.y, rotation.z);
        }
	
        public static void AddLocalRotationY(this Transform transform, float y)
        {
            var rotation = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(rotation.x, rotation.y + y, rotation.z);
        }
	
        public static void AddLocalRotationZ(this Transform transform, float z)
        {
            var rotation = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z + z);
        }
	
        // Set Rotation
	
        public static void SetRotation(this Transform transform, Vector3 rotation)
        {
            transform.rotation = Quaternion.Euler(rotation);
        }
	
        public static void SetRotation(this Transform transform, float x, float y, float z)
        {
            transform.rotation = Quaternion.Euler(x, y, z);
        }
	
        public static void SetRotationXY(this Transform transform, float x, float y)
        {
            var rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(x, y, rotation.z);
        }
	
        public static void SetRotationX(this Transform transform, float x)
        {
            var rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(x, rotation.y, rotation.z);
        }
	
        public static void SetRotationY(this Transform transform, float y)
        {
            var rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x, y, rotation.z);
        }
	
        public static void SetRotationZ(this Transform transform, float z)
        {
            var rotation = transform.rotation.eulerAngles;
            transform.rotation = Quaternion.Euler(rotation.x, rotation.y, z);
        }
	
        public static void SetLocalRotation(this Transform transform, Vector3 rotation)
        {
            transform.localRotation = Quaternion.Euler(rotation);
        }
	
        public static void SetLocalRotation(this Transform transform, float x, float y, float z)
        {
            transform.localRotation = Quaternion.Euler(x, y, z);
        }
	
        public static void SetLocalRotationXY(this Transform transform, float x, float y)
        {
            var rotation = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(x, y, rotation.z);
        }
	
        public static void SetLocalRotationX(this Transform transform, float x)
        {
            var rotation = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(x, rotation.y, rotation.z);
        }
	
        public static void SetLocalRotationY(this Transform transform, float y)
        {
            var rotation = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(rotation.x, y, rotation.z);
        }
	
        public static void SetLocalRotationZ(this Transform transform, float z)
        {
            var rotation = transform.localRotation.eulerAngles;
            transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, z);
        }
	
        // Get Rotation
        
        public static Vector3 GetRotation(this Transform transform)
        {
            return transform.rotation.eulerAngles;
        }
	
        public static float GetRotationX(this Transform transform)
        {
            return transform.rotation.eulerAngles.x;
        }
	
        public static float GetRotationY(this Transform transform)
        {
            return transform.rotation.eulerAngles.y;
        }
	
        public static float GetRotationZ(this Transform transform)
        {
            return transform.rotation.eulerAngles.z;
        }
	
        public static Vector3 GetLocalRotation(this Transform transform)
        {
            return transform.localRotation.eulerAngles;
        }
	
        public static float GetLocalRotationX(this Transform transform)
        {
            return transform.localRotation.eulerAngles.x;
        }
	
        public static float GetLocalRotationY(this Transform transform)
        {
            return transform.localRotation.eulerAngles.y;
        }
	
        public static float GetLocalRotationZ(this Transform transform)
        {
            return transform.localRotation.eulerAngles.z;
        }
	}
}