using System.Text.RegularExpressions;
using Smart.Extensions;
using Smart.Utils;
using UnityEditor;
using UnityEngine;

namespace Smart.Attributes
{
	public class RegexStringAttribute : PropertyAttribute
	{
		public readonly Regex Regex;
		public readonly RegexStringMode AttributeMode;

		public RegexStringAttribute(string regex, RegexStringMode mode = RegexStringMode.Match, RegexOptions options = RegexOptions.None)
		{
			Regex = new Regex(regex, options);
			AttributeMode = mode;
		}
	}

	public enum RegexStringMode
	{
		Match,
		Replace,
		WarningIfMatch,
		WarningIfNotMatch
	}
}

#if UNITY_EDITOR
namespace Smart.Attributes.Internal
{
	[CustomPropertyDrawer(typeof(RegexStringAttribute))]
	public class RegexStringAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				SmartGUI.DrawColouredRect(position, SmartGUI.Colors.Red);
				EditorGUI.LabelField(position, new GUIContent("", "[RegexStringAttribute] used with non-string property"));
			}
			else
			{
				var regex = (RegexStringAttribute)attribute;
				var mode = regex.AttributeMode;
				bool ifMatch = mode == RegexStringMode.WarningIfMatch;
				bool ifNotMatch = mode == RegexStringMode.WarningIfNotMatch;
				bool anyMatching = regex.Regex.IsMatch(property.stringValue);
				bool warn = (ifMatch && anyMatching) || (ifNotMatch && !anyMatching);
				var originalPosition = position;

				DrawWarning();
				position.width -= 20;
				EditorGUI.PropertyField(position, property, label, true);
				DrawTooltip();

				if (GUI.changed)
				{
					if (mode == RegexStringMode.Replace) OnReplace();
					if (mode == RegexStringMode.Match) OnKeepMatching();
				}

				if (warn)
				{
					GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);
					position = originalPosition;
					position.y += EditorGUIUtility.singleLineHeight;
					DrawWarning();
					position.x += EditorGUIUtility.labelWidth;
					var warningContent = new GUIContent("Regex rule violated!");
					EditorGUI.LabelField(position, warningContent, EditorStyles.miniBoldLabel);
				}

				property.serializedObject.ApplyModifiedProperties();


				void OnReplace() => property.stringValue = regex.Regex.Replace(property.stringValue, "");
				void OnKeepMatching() => property.stringValue = regex.Regex.KeepMatching(property.stringValue);

				void DrawWarning()
				{
					if (!ifMatch && !ifNotMatch) return;
					SmartGUI.DrawColouredRect(position, warn ? SmartGUI.Colors.Yellow : Color.clear);
				}

				void DrawTooltip()
				{
					string tooltip = "Regex field: ";
					if (mode == RegexStringMode.Match || mode == RegexStringMode.WarningIfNotMatch) tooltip += "match expression";
					else tooltip += "remove expression";
					tooltip += $"\n[{regex.Regex}]";

					position.x += position.width + 2;
					position.width = 18;
					var tooltipContent = new GUIContent(EditorGUIUtility.IconContent("_Help"));
					tooltipContent.tooltip = tooltip;
					EditorGUI.LabelField(position, tooltipContent, EditorStyles.label);
				}
			}
		}
	}
}
#endif