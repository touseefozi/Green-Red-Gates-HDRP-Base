using UnityEditor;
using UnityEngine;

namespace Smart.Attributes
{
	public class OverrideLabelAttribute : PropertyAttribute
	{
		public readonly string NewLabel;

		public OverrideLabelAttribute(string newLabel) => NewLabel = newLabel;
	}
}

#if UNITY_EDITOR
namespace Smart.Attributes.Internal
{
	[CustomPropertyDrawer(typeof(OverrideLabelAttribute))]
	public class OverrideLabelDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			label.text = ((OverrideLabelAttribute) attribute).NewLabel;
			EditorGUI.PropertyField(position, property, label);
		}
	}
}
#endif