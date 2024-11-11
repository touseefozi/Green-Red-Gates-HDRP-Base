using Smart.Attributes;

namespace SmartEditor
{
	using UnityEditor;
	using UnityEngine;

	[CustomEditor(typeof(MonoBehaviour), true)]
	public class DefaultMonoBehaviourEditor : Editor
	{
		private const string ScriptProperty = "m_Script";
		
		private bool hideScriptField;

		private void OnEnable()
		{
			hideScriptField = target.GetType().GetCustomAttributes(typeof(HideScriptFieldAttribute), false).Length > 0;
		}

		public override void OnInspectorGUI()
		{
			if (hideScriptField)
			{
				serializedObject.Update();
				EditorGUI.BeginChangeCheck();
				DrawPropertiesExcluding(serializedObject, ScriptProperty);
				
				if (EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();
				}
			}
			else
			{
				base.OnInspectorGUI();
			}
		}
	}
}