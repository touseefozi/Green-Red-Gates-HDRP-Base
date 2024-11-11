using Smart.Animation;
using UnityEditor;

namespace SmartEditor.Animation
{
	[CustomEditor(typeof(TweenShakeRotationZ))]
	public class TweenShakeRotationZEditor : Editor
	{
		private const string EasingProperty = "_easing";
		private const string ScriptProperty = "m_Script";

		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUI.BeginChangeCheck();
			DrawPropertiesExcluding(serializedObject, ScriptProperty, EasingProperty);
			
			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}