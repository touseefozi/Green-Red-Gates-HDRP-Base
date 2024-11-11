using Smart.Utils;
using UnityEngine;

namespace Smart.Attributes
{
	public class CharactersRangeAttribute : PropertyAttribute
	{
		public readonly string Characters;
		public readonly CharacterRangeMode Mode;
		public readonly bool IgnoreCase;

		public CharactersRangeAttribute(string characters, CharacterRangeMode mode = CharacterRangeMode.Allow,
			bool ignoreCase = true)
		{
			Characters = characters;
			Mode = mode;
			IgnoreCase = ignoreCase;
		}
	}

	public enum CharacterRangeMode
	{
		Allow,
		Disallow,
		WarningIfAny,
		WarningIfNotMatch
	}
}

#if UNITY_EDITOR
namespace Smart.Attributes.Internal
{
	using UnityEditor;
	using System.Linq;

	[CustomPropertyDrawer(typeof(CharactersRangeAttribute))]
	public class CharacterRangeAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			if (property.propertyType != SerializedPropertyType.String)
			{
				SmartGUI.DrawColouredRect(position, SmartGUI.Colors.Red);
				EditorGUI.LabelField(position, new GUIContent("", "[CharacterRangeAttribute] used with non-string property"));
			}
			else
			{
				var charactersRange = (CharactersRangeAttribute)attribute;
				var mode = charactersRange.Mode;
				var ignoreCase = charactersRange.IgnoreCase;
				var filter = charactersRange.Characters;
				var allow = mode == CharacterRangeMode.Allow || mode == CharacterRangeMode.WarningIfNotMatch;
				var warning = mode == CharacterRangeMode.WarningIfAny || mode == CharacterRangeMode.WarningIfNotMatch;

				if (ignoreCase) filter = filter.ToUpper();
				var filteredCharacters = property.stringValue.Distinct()
					.Where(c =>
					{
						if (ignoreCase) c = char.ToUpper(c);
						return filter.Contains(c) ^ allow;
					});
				bool ifMatch = mode == CharacterRangeMode.WarningIfAny;
				bool ifNotMatch = mode == CharacterRangeMode.WarningIfNotMatch;
				bool anyFiltered = filteredCharacters.Any();
				bool warn = (ifMatch && anyFiltered || ifNotMatch && anyFiltered);
				var originalPosition = position;

				DrawWarning();
				position.width -= 20;

				property.stringValue = EditorGUI.TextField(position, label, property.stringValue);
				DrawTooltip();

				if (!warning)
				{
					property.stringValue = filteredCharacters.Aggregate(
						property.stringValue,
						(p, c) => p.Replace(c.ToString(), ""));
				}

				if (warn)
				{
					GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.objectField);
					position = originalPosition;
					position.y += EditorGUIUtility.singleLineHeight;
					DrawWarning();
					position.x += EditorGUIUtility.labelWidth;
					var warningContent = new GUIContent("Containing disallowed characters!");
					EditorGUI.LabelField(position, warningContent, EditorStyles.miniBoldLabel);
				}

				property.serializedObject.ApplyModifiedProperties();


				void DrawWarning()
				{
					if (!warning) return;
					if (property.stringValue.Length == 0) warn = false;
					SmartGUI.DrawColouredRect(position, warn ? SmartGUI.Colors.Yellow : Color.clear);
				}

				void DrawTooltip()
				{
					string tooltip = "Characters range ";
					if (mode == CharacterRangeMode.Allow || mode == CharacterRangeMode.WarningIfNotMatch) tooltip += "is allowed:";
					else tooltip += "not allowed:";
					tooltip += $"\n[{filter}]";

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