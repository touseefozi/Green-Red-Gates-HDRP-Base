using UnityEditor;
using UnityEngine;

namespace Smart.Attributes
{
	public class ReadOnlyAttribute : ConditionalFieldAttribute
	{
		public ReadOnlyAttribute() : base("")
		{
		}

		public ReadOnlyAttribute(string fieldToCheck, bool inverse = false, params object[] compareValues)
			: base(fieldToCheck, inverse, compareValues)
		{
		}
	}
}

#if UNITY_EDITOR
namespace Smart.Attributes.Internal
{
	[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
	public class ReadOnlyAttributeDrawer : PropertyDrawer
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return EditorGUI.GetPropertyHeight(property, label, true);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (!(attribute is ReadOnlyAttribute conditional)) return;

			bool enabled = false;

			if (!string.IsNullOrEmpty(conditional.FieldToCheck))
			{
				var propertyToCheck = ConditionalFieldUtility.FindRelativeProperty(property, conditional.FieldToCheck);
				enabled = !ConditionalFieldUtility.PropertyIsVisible(propertyToCheck, conditional.Inverse, conditional.CompareValues);
			}

			GUI.enabled = enabled;
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}
}
#endif