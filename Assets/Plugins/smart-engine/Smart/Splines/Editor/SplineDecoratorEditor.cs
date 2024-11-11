using Smart.Splines.Components;
using UnityEditor;
using UnityEngine;

namespace Smart.Splines
{
    [CustomEditor(typeof(SplineDecorator))]
    public class SplineDecoratorEditor : Editor
    {
        private const int ButtonHeight = 35;
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var buttonHeight = GUILayout.Height(ButtonHeight);
            
            GUILayout.Space(10);
            
            var decorator = (SplineDecorator) target;
            
            if (GUILayout.Button("Redraw", buttonHeight))
            {
                Undo.RecordObject(decorator, "Redraw Spline Decorator");
                decorator.Clear();
                decorator.Redraw();
                EditorUtility.SetDirty(decorator);
            }
        }

        private void OnSceneGUI()
        {
            var decorator = (SplineDecorator) target;
            
            if (decorator.RealtimeMode && !Application.isPlaying)
            {
                decorator.Redraw();
            }
        }
    }
}