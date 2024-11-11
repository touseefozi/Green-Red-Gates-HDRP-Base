using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Smart.Attributes
{
	public class SpriteLayerAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR
namespace Smart.Attributes.Internal
{
	[CustomPropertyDrawer(typeof(SpriteLayerAttribute))]
	public class SpriteLayerAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.Integer)
			{
				if (!_checkedType) PropertyTypeWarning(property);
				EditorGUI.PropertyField(position, property, label);
				return;
			}

			var spriteLayerNames = GetSpriteLayerNames();
			HandleSpriteLayerSelectionUI(position, property, label, spriteLayerNames);
		}

		private bool _checkedType;
		private void PropertyTypeWarning(SerializedProperty property)
		{
			Debug.LogWarning(string.Format("Property <color=brown>{0}</color> in object <color=brown>{1}</color> is of wrong type. Expected: Int",
				property.name, property.serializedObject.targetObject));
			_checkedType = true;
		}
		
		private void HandleSpriteLayerSelectionUI(Rect position, SerializedProperty property, GUIContent label, string[] spriteLayerNames)
		{
			EditorGUI.BeginProperty(position, label, property);

			bool layerFound = TryGetSpriteLayerIndexFromProperty(out var currentSpriteLayerIndex, spriteLayerNames, property);

			if (!layerFound)
			{
				Debug.Log(string.Format(
					"Property <color=brown>{0}</color> in object <color=brown>{1}</color> is set to the default layer. Reason: previously selected layer was removed.",
					property.name, property.serializedObject.targetObject));
				property.intValue = 0;
				currentSpriteLayerIndex = 0;
			}

			int selectedSpriteLayerIndex = EditorGUI.Popup(position, label.text, currentSpriteLayerIndex, spriteLayerNames);

			if (selectedSpriteLayerIndex != currentSpriteLayerIndex)
			{
				property.intValue = SortingLayer.NameToID(spriteLayerNames[selectedSpriteLayerIndex]);
			}

			EditorGUI.EndProperty();
		}

		private bool TryGetSpriteLayerIndexFromProperty(out int index, string[] spriteLayerNames, SerializedProperty property)
		{
			var layerName = SortingLayer.IDToName(property.intValue);

			for (int i = 0; i < spriteLayerNames.Length; ++i)
			{
				if (spriteLayerNames[i].Equals(layerName))
				{
					index = i;
					return true;
				}
			}

			index = -1;
			return false;
		}

		private string[] GetSpriteLayerNames()
		{
			var result = new string[SortingLayer.layers.Length];

			for (int i = 0; i < result.Length; ++i)
			{
				result[i] = SortingLayer.layers[i].name;
			}

			return result;
		}
	}
}
#endif