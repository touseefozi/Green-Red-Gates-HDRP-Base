using Smart.DataTypes;
using SmartEditor.Utils;
using UnityEditor;
using UnityEngine;

namespace SmartEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(IntRange))]
	public class IntRangePropertyDrawer : MinMaxPropertyDrawer {}

	[CustomPropertyDrawer(typeof(FloatRange))]
	public class FloatRangePropertyDrawer : MinMaxPropertyDrawer {}

	public class MinMaxPropertyDrawer : PropertyDrawer
	{
		private const string MinProperty = "Min";
		private const string MaxProperty = "Max";
		
		public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
		{
			SmartEditorGUI.DrawTwoPropertiesInline(pos, property, label, MinProperty, MaxProperty);
		}
	}
}