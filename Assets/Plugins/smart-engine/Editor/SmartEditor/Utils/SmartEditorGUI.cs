using UnityEditor;
using UnityEngine;

namespace SmartEditor.Utils
{
	public static class SmartEditorGUI
	{
		private const float SubLabelSpacing = 4;
    
		public static void DrawTwoPropertiesInline(Rect pos, SerializedProperty property, GUIContent label, params string[] propertyNames)
		{
			label = EditorGUI.BeginProperty(pos, label, property);

			var firstProperty = propertyNames[0];
			var secondProperty = propertyNames[1];
			var contentRect = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
        
			var labels = new[]
			{
				new GUIContent(firstProperty),
				new GUIContent(secondProperty)
			};
        
			var properties = new[]
			{
				property.FindPropertyRelative(firstProperty),
				property.FindPropertyRelative(secondProperty),
			};
        
			DrawMultiplePropertyFields(contentRect, labels, properties);
		}
		
		public static void DrawMultiplePropertyFields(Rect pos, GUIContent[] subLabels, SerializedProperty[] props)
		{
			var indent = EditorGUI.indentLevel;
			var labelWidth = EditorGUIUtility.labelWidth;
			var propsCount = props.Length;
			var width = (pos.width - (propsCount - 1) * SubLabelSpacing) / propsCount;
			var contentPos = new Rect(pos.x, pos.y, width, pos.height);
        
			EditorGUI.indentLevel = 0;
        
			for (var i = 0; i < propsCount; i++)
			{
				EditorGUIUtility.labelWidth = EditorStyles.label.CalcSize(subLabels[i]).x;
				EditorGUI.PropertyField(contentPos, props[i], subLabels[i]);
				contentPos.x += width + SubLabelSpacing;
			}

			EditorGUIUtility.labelWidth = labelWidth;
			EditorGUI.indentLevel = indent;
		}
    
		public static void DrawTwoPropertiesInlineNoLabels(Rect pos, SerializedProperty property, GUIContent label, params string[] propertyNames)
		{
			label = EditorGUI.BeginProperty(pos, label, property);

			var contentRect = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);

			var properties = new[]
			{
				property.FindPropertyRelative(propertyNames[0]),
				property.FindPropertyRelative(propertyNames[1]),
			};
        
			DrawMultiplePropertyFieldsNoLabels(contentRect, properties);
		}
		
		public static void DrawMultiplePropertyFieldsNoLabels(Rect pos, SerializedProperty[] props)
		{
			var indent = EditorGUI.indentLevel;
			var propsCount = props.Length;
			var width = (pos.width - (propsCount - 1) * SubLabelSpacing) / propsCount;
			var contentPos = new Rect(pos.x, pos.y, width, pos.height);
        
			EditorGUI.indentLevel = 0;
        
			for (var i = 0; i < propsCount; i++)
			{
				EditorGUIUtility.labelWidth = 0f;
				EditorGUI.PropertyField(contentPos, props[i], new GUIContent(""));
				contentPos.x += width + SubLabelSpacing;
			}

			EditorGUI.indentLevel = indent;
		}
	}
}