using UnityEngine;

namespace Smart.Extensions
{
	public static class GameObjectExtension
	{
		public static void Show(this GameObject gameObject)
		{
			gameObject.SetActive(true);
		}
		
		public static void Hide(this GameObject gameObject)
		{
			gameObject.SetActive(false);
		}
		
		public static GameObject CreateChild(this GameObject gameObject, string name)
		{
			var child = new GameObject(name);
			child.transform.SetParent(gameObject.transform, false);
			return child;
		}
		
		public static GameObject GetChild(this GameObject gameObject, string name)
		{
			var transform = gameObject.transform.Find(name);
			return transform != null ? transform.gameObject : null;
		}
	
		public static T GetChildComponent<T>(this GameObject gameObject, string name) where T : Component
		{
			var transform = gameObject.transform.Find(name);
			return transform != null ? transform.gameObject.GetComponent<T>() : null;
		}
		
		public static void Clear(this GameObject gameObject)
		{
            for (int i = gameObject.transform.childCount - 1; i >= 0; i--)
            {
			    var child = gameObject.transform.GetChild(i);
                Object.DestroyImmediate(child.gameObject);
            }
		}
        
        public static void SafeDestroy(this GameObject gameObject)
        {
            if (gameObject != null)
            {
                Object.Destroy(gameObject);
            }
        }
        
        public static void SafeDestroyImmediate(this GameObject gameObject)
        {
            if (gameObject != null)
            {
                Object.DestroyImmediate(gameObject);
            }
        }
	}
}