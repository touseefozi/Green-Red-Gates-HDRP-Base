using System;
using Smart.Utils;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Smart.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class MustBeAssignedAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace Smart.Attributes.Internal
{
	using System.Reflection;
	using UnityEditor;
	
#if UNITY_2020_3
	using UnityEditor.Experimental.SceneManagement;
#else
	using UnityEditor.SceneManagement;
#endif

	[InitializeOnLoad]
	public class MustBeAssignedAttributeChecker
	{
		public static Func<FieldInfo, Object, bool> ExcludeFieldFilter;

		static MustBeAssignedAttributeChecker()
		{
			SmartEditorEvents.OnSave += AssertComponentsInScene;
			PrefabStage.prefabSaved += AssertComponentsInPrefab;
		}

		private static void AssertComponentsInScene()
		{
			var behaviours = Object.FindObjectsOfType<MonoBehaviour>(true);
			AssertComponents(behaviours);

			var scriptableObjects = LoadAssets<ScriptableObject>();
			AssertComponents(scriptableObjects);
		}
		
		private static T[] LoadAssets<T>() where T : ScriptableObject
		{
			var guids = AssetDatabase.FindAssets("t:" + typeof(T).Name);
			var a = new T[guids.Length];
			
			for (var i = 0; i < guids.Length; i++)
			{
				var path = AssetDatabase.GUIDToAssetPath(guids[i]);
				a[i] = AssetDatabase.LoadAssetAtPath<T>(path);
			}

			return a;
		}

		private static void AssertComponentsInPrefab(GameObject prefab)
		{
			MonoBehaviour[] components = prefab.GetComponentsInChildren<MonoBehaviour>();
			AssertComponents(components);
		}

		private static void AssertComponents(Object[] objects)
		{
			var mustBeAssignedType = typeof(MustBeAssignedAttribute);
			foreach (var obj in objects)
			{
				if (obj == null) continue;
				
				Type typeOfScript = obj.GetType();
				var typeFields = typeOfScript.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

				foreach (FieldInfo field in typeFields)
				{
					if (!field.IsDefined(mustBeAssignedType, false)) continue;
					
					if (FieldIsExcluded(field, obj)) continue;

					AssertField(obj, typeOfScript, field);
				}
			}
		}
		
		private static void AssertField(Object targetObject, Type targetType, FieldInfo field)
		{
			object fieldValue = field.GetValue(targetObject);
			
			bool valueTypeWithDefaultValue = field.FieldType.IsValueType && Activator.CreateInstance(field.FieldType).Equals(fieldValue);
			if (valueTypeWithDefaultValue)
			{
				Debug.LogError($"{targetType.Name} caused: {field.Name} is Value Type with default value", targetObject);
				return;
			}

					
			bool nullReferenceType = fieldValue == null || fieldValue.Equals(null);
			if (nullReferenceType)
			{
				Debug.LogError($"{targetType.Name} caused: {field.Name} is not assigned (null value)", targetObject);
				return;
			}


			bool emptyString = field.FieldType == typeof(string) && (string) fieldValue == string.Empty;
			if (emptyString)
			{
				Debug.LogError($"{targetType.Name} caused: {field.Name} is not assigned (empty string)", targetObject);
				return;
			}


			if (fieldValue is Array arr && arr.Length == 0)
			{
				Debug.LogError($"{targetType.Name} caused: {field.Name} is not assigned (empty array)", targetObject);
			}
		}
		
		private static bool FieldIsExcluded(FieldInfo field, Object behaviour)
		{
			if (ExcludeFieldFilter == null) return false;

			foreach (var filterDelegate in ExcludeFieldFilter.GetInvocationList())
			{
				var filter = filterDelegate as Func<FieldInfo, Object, bool>;
				if (filter != null && filter(field, behaviour)) return true;
			}

			return false;
		}
	}
}
#endif