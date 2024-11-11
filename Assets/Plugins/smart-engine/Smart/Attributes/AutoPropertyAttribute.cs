using System;
using System.Reflection;
using Smart.Extensions;
using Smart.Utils;
using UnityEngine;

namespace Smart.Attributes
{
	[AttributeUsage(AttributeTargets.Field)]
	public class AutoPropertyAttribute : PropertyAttribute
	{
		public readonly AutoPropertyMode Mode;

		public AutoPropertyAttribute(AutoPropertyMode mode = AutoPropertyMode.Children) => Mode = mode;
	}

	public enum AutoPropertyMode
	{
		Children = 0,
		Parent = 1,
		Scene = 2,
		Asset = 3,
		Any = 4
	}
}

#if UNITY_EDITOR
namespace Smart.Attributes.Internal
{
	using UnityEditor;
	using System.Collections.Generic;
	using System.Linq;
	using Object = UnityEngine.Object;
	
#if UNITY_2020_3
	using UnityEditor.Experimental.SceneManagement;
#else
	using UnityEditor.SceneManagement;
#endif

	[CustomPropertyDrawer(typeof(AutoPropertyAttribute))]
	public class AutoPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			GUI.enabled = false;
			EditorGUI.PropertyField(position, property, label);
			GUI.enabled = true;
		}
	}

	[InitializeOnLoad]
	public static class AutoPropertyHandler
	{
		private struct ObjectField
		{
			public readonly FieldInfo Field;
			public readonly Object Context;

			public ObjectField(FieldInfo field, Object context)
			{
				Field = field;
				Context = context;
			}
		}

		private static readonly Dictionary<AutoPropertyMode, Func<ObjectField, Object[]>> MultipleObjectsGetters
			= new Dictionary<AutoPropertyMode, Func<ObjectField, Object[]>>
			{
				[AutoPropertyMode.Children] = property => (property.Context as Component)
					?.GetComponentsInChildren(property.Field.FieldType.GetElementType(), true),
				[AutoPropertyMode.Parent] = property => (property.Context as Component)
					?.GetComponentsInParent(property.Field.FieldType.GetElementType(), true),
				[AutoPropertyMode.Scene] = property => GetAllComponentsInSceneOf(property.Context,
						property.Field.FieldType.GetElementType()).ToArray(),
				[AutoPropertyMode.Asset] = property => Resources
					.FindObjectsOfTypeAll(property.Field.FieldType.GetElementType())
					.Where(AssetDatabase.Contains).ToArray(),
				[AutoPropertyMode.Any] = property => Resources
					.FindObjectsOfTypeAll(property.Field.FieldType.GetElementType())
			};

		private static readonly Dictionary<AutoPropertyMode, Func<ObjectField, Object>> SingularObjectGetters
			= new Dictionary<AutoPropertyMode, Func<ObjectField, Object>>
			{
				[AutoPropertyMode.Children] = property => (property.Context as Component)
					?.GetComponentInChildren(property.Field.FieldType, true),
				[AutoPropertyMode.Parent] = property => (property.Context as Component)
					?.GetComponentsInParent(property.Field.FieldType, true)
					.FirstOrDefault(),
				[AutoPropertyMode.Scene] = property => GetAllComponentsInSceneOf(property.Context, property.Field.FieldType)
					.FirstOrDefault(),
				[AutoPropertyMode.Asset] = property => Resources
					.FindObjectsOfTypeAll(property.Field.FieldType)
					.FirstOrDefault(AssetDatabase.Contains),
				[AutoPropertyMode.Any] = property => Resources
					.FindObjectsOfTypeAll(property.Field.FieldType)
					.FirstOrDefault()
			};

		static AutoPropertyHandler()
		{
			// this event is for GameObjects in the project.
			SmartEditorEvents.OnSave += CheckAssets;
			// this event is for prefabs saved in edit mode.
			PrefabStage.prefabSaved += CheckComponentsInPrefab;
			PrefabStage.prefabStageOpened += stage => CheckComponentsInPrefab(stage.prefabContentsRoot);
		}

		private static void CheckAssets()
		{
			var toFill = GetFieldsWithAttributeFromAll<AutoPropertyAttribute>();
			toFill.ForEach(FillProperty);
		}

		private static void CheckComponentsInPrefab(GameObject prefab) => GetFieldsWithAttribute<AutoPropertyAttribute>(prefab)
			.ForEach(FillProperty);

		private static void FillProperty(ObjectField property)
		{
			var apAttribute = property.Field
				.GetCustomAttributes(typeof(AutoPropertyAttribute), true)
				.FirstOrDefault() as AutoPropertyAttribute;
			if (apAttribute == null) return;

			if (property.Field.FieldType.IsArray)
			{
				var objects = MultipleObjectsGetters[apAttribute.Mode].Invoke(property);
				if (objects != null && objects.Length > 0)
				{
					var serializedObject = new SerializedObject(property.Context);
					var serializedProperty = serializedObject.FindProperty(property.Field.Name);
					serializedProperty.ReplaceArray(objects);
					serializedObject.ApplyModifiedProperties();
					return;
				}
			}
			else
			{
				var obj = SingularObjectGetters[apAttribute.Mode].Invoke(property);
				if (obj != null)
				{
					var serializedObject = new SerializedObject(property.Context);
					var serializedProperty = serializedObject.FindProperty(property.Field.Name);
					serializedProperty.objectReferenceValue = obj;
					serializedObject.ApplyModifiedProperties();
					return;
				}
			}

			Debug.LogError($"{property.Context.name} caused: {property.Field.Name} is failed to Auto Assign property. No match",
				property.Context);
		}

		private static IEnumerable<Component> GetAllComponentsInSceneOf(Object obj, Type type)
		{
			GameObject contextGO;
			if (obj is Component comp) contextGO = comp.gameObject;
			else if (obj is GameObject go) contextGO = go;
			else return Array.Empty<Component>();
			if (contextGO.scene.isLoaded)
				return contextGO.scene.GetRootGameObjects()
					.SelectMany(rgo => rgo.GetComponentsInChildren(type, true));
			return Array.Empty<Component>();
		}

		private static void LoadAllAssetsOfType(Type type)
		{
			var guids = AssetDatabase.FindAssets($"t:{type.FullName}");

			foreach (var guid in guids)
			{
				AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), type);
			}
		}

		public static void LoadAllAssetsOfType(string typeName)
		{
			var guids = AssetDatabase.FindAssets($"t:{typeName}");

			foreach (var guid in guids)
			{
				AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(UnityEngine.Object));
			}
		}

		private static Object[] GetAllUnityObjects()
		{
			LoadAllAssetsOfType(typeof(ScriptableObject));
			LoadAllAssetsOfType("Prefab");
			return Resources.FindObjectsOfTypeAll(typeof(Object));
		}
		
		private static List<ObjectField> GetFieldsWithAttributeFromAll<T>() where T : Attribute
		{
			var allObjects = GetAllUnityObjects();

			return GetFieldsWithAttribute<T>(allObjects);
		}
		
		private static List<ObjectField> GetFieldsWithAttribute<T>(GameObject root) where T : Attribute
		{
			var allObjects = root.GetComponentsInChildren<MonoBehaviour>();
			return GetFieldsWithAttribute<T>(allObjects);
		}
		
		private static List<ObjectField> GetFieldsWithAttribute<T>(Object[] objects) where T : Attribute
		{
			var desiredAttribute = typeof(T);
			var result = new List<ObjectField>();
			
			foreach (var o in objects)
			{
				if (o == null) continue;
				
				var fields = o.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
				
				foreach (var field in fields)
				{
					if (!field.IsDefined(desiredAttribute, false)) continue;
					result.Add(new ObjectField(field, o));
				}
			}

			return result;
		}
	}
}
#endif