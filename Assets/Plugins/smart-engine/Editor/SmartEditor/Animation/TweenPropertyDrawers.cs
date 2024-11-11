using Smart.Animation.DataTypes;
using SmartEditor.Utils;
using UnityEditor;
using UnityEngine;

namespace SmartEditor.Animation
{
	[CustomPropertyDrawer(typeof(TweenTimingsValue))]
	internal class TweenTimeValuePropertyDrawer : PropertyDrawer
	{
		private const string DurationProperty = "Duration";
		private const string DelayProperty = "Delay";
    
		public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
		{
			SmartEditorGUI.DrawTwoPropertiesInline(pos, property, label, DurationProperty, DelayProperty);
		}
	}
	
	[CustomPropertyDrawer(typeof(TweenFloatValue))]
	internal class TweenFloatValuePropertyDrawer : StartEndPropertyDrawer {}

	[CustomPropertyDrawer(typeof(TweenIntValue))]
	internal class TweenIntValuePropertyDrawer : StartEndPropertyDrawer {}

	[CustomPropertyDrawer(typeof(TweenLongValue))]
	internal class TweenLongValuePropertyDrawer : StartEndPropertyDrawer {}

	internal class StartEndPropertyDrawer : PropertyDrawer
	{
		private const string StartProperty = "Start";
		private const string EndProperty = "End";
    
		public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
		{
			SmartEditorGUI.DrawTwoPropertiesInline(pos, property, label, StartProperty, EndProperty);
		}
	}
}