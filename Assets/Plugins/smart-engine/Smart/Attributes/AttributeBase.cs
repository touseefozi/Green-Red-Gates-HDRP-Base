using System;
using UnityEngine;

namespace Smart.Attributes.Internal
{
	[AttributeUsage(AttributeTargets.Field)]
	public abstract class AttributeBase : PropertyAttribute
	{
#if UNITY_EDITOR
		public virtual void ValidateProperty(UnityEditor.SerializedProperty property)
		{
		}
		
		public virtual void OnBeforeGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
		{
		}

		public virtual bool OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
		{
			return false;
		}

		public virtual void OnAfterGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
		{
		}

		public virtual float? OverrideHeight()
		{
			return null;
		}
#endif
	}
}