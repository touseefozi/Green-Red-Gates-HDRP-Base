using Smart.Interfaces;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Smart.Attributes
{
	public class AutoRefreshAttribute : PropertyAttribute
	{
	}
}

#if UNITY_EDITOR

namespace Smart.Attributes.Internal
{
	[CustomPropertyDrawer(typeof(AutoRefreshAttribute))]
	public class AutoRefreshAttributeDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var serializedObject = property.serializedObject;
			serializedObject.UpdateIfRequiredOrScript();
		
			var target = serializedObject.targetObject as IAutoRefreshable;

			if (target == null)
			{
				EditorGUI.HelpBox(position, "Script should implement IAutoRefreshable interface", MessageType.Error);
			}
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(property, new GUIContent(property.displayName), true);
			serializedObject.ApplyModifiedProperties();

			if (EditorGUI.EndChangeCheck())
			{
				if (target != null)
				{
					target.AutoRefresh();
				}
				
				EditorUtility.SetDirty(serializedObject.targetObject);
			}
		}
	}
}
#endif