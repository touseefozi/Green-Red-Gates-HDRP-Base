using Smart.Splines.Components;
using UnityEditor;
using UnityEngine;

namespace Smart.Splines
{
    [CustomEditor(typeof(CatmullRomSplineComponent))]
    public class CatmullRomSplineEditor : Editor
    {
        private const float HandleSize = 0.06f;
        private const float PickSize = 0.08f;
        
        private CatmullRomSplineComponent _spline;
        private Transform _handleTransform;
        private Quaternion _handleRotation;
        private int _selectedIndex;
        
        private void OnSceneGUI()
        {
            _spline = (CatmullRomSplineComponent) target;
            _handleTransform = _spline.transform;
            _handleRotation = Tools.pivotRotation == PivotRotation.Local ? _handleTransform.rotation : Quaternion.identity;

            for (int i = 0; i < _spline.ControlPointsCount; i++)
            {
                ShowPoint(i);
            }
            
            _spline.Recalculate();
            _spline.DrawSpline();
        }
        
        private void ShowPoint(int index)
        {
            var point = _handleTransform.TransformPoint(_spline.GetControlPoint(index));
            var size = HandleUtility.GetHandleSize(point);
            
            Handles.color = Color.white;
            
            if (Handles.Button(point, _handleRotation, size * HandleSize, size * PickSize, Handles.DotHandleCap))
            {
                _selectedIndex = index;
                Repaint();
            }

            if (_selectedIndex == index)
            {
                EditorGUI.BeginChangeCheck();
                point = Handles.DoPositionHandle(point, _handleRotation);
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_spline, "Move Point");
                    _spline.SetControlPoint(index, _handleTransform.InverseTransformPoint(point));
                    EditorUtility.SetDirty(_spline);
                }
            }
        }
    }
}