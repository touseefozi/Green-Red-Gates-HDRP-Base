#if UNITY_EDITOR
using System.Linq;
using Smart.Basics.Extensions;
using UnityEditor;
using UnityEngine;

namespace Smart.Attributes.Internal
{
	[CustomPropertyDrawer(typeof(AttributeBase), true)]
	public class AttributeBaseDrawer : PropertyDrawer
	{
		private AttributeBase[] _cachedAttributes;

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			CacheAttributes();

			for (var i = _cachedAttributes.Length - 1; i >= 0; i--)
			{
				var overriden = _cachedAttributes[i].OverrideHeight();
				if (overriden != null) return overriden.Value;
			}
			
			return base.GetPropertyHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			CacheAttributes();

			var drawn = false;
			
			for (var i = _cachedAttributes.Length - 1; i >= 0; i--)
			{
				var ab = _cachedAttributes[i];
				ab.ValidateProperty(property);
				
				ab.OnBeforeGUI(position, property, label);

				if (!drawn)
				{
					if (ab.OnGUI(position, property, label)) drawn = true;
				}
				
				ab.OnAfterGUI(position, property, label);
			}

			if (!drawn) EditorGUI.PropertyField(position, property, label);
		}

		private void CacheAttributes()
		{
			if (_cachedAttributes.IsNullOrEmpty())
			{
				_cachedAttributes = fieldInfo
					.GetCustomAttributes(typeof(AttributeBase), false)
					.OrderBy(s => ((PropertyAttribute) s).order)
					.Select(a => a as AttributeBase).ToArray();
			}
		}
	}
}
#endif