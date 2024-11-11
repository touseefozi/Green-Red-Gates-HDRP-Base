using Smart.DataTypes;
using SmartEditor.Utils;
using UnityEditor;
using UnityEngine;

namespace SmartEditor.PropertyDrawers
{
	[CustomPropertyDrawer(typeof(SizeInt))]
	public class SizeIntPropertyDrawer : SizePropertyDrawer
	{
	}

	[CustomPropertyDrawer(typeof(Size))]
	public class SizePropertyDrawer : PropertyDrawer
	{
		private const string WidthProperty = "Width";
		private const string HeightProperty = "Height";
		
		public override void OnGUI(Rect pos, SerializedProperty property, GUIContent label)
		{
			label = EditorGUI.BeginProperty(pos, label, property);
			
			var contentRect = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
			
			var labels = new[]
			{
				new GUIContent(WidthProperty),
				new GUIContent(HeightProperty)
			};
			
			var properties = new[]
			{
				property.FindPropertyRelative(WidthProperty),
				property.FindPropertyRelative(HeightProperty),
			};
			
			SmartEditorGUI.DrawMultiplePropertyFields(contentRect, labels, properties);
			EditorGUI.EndProperty();
		}
	}
}