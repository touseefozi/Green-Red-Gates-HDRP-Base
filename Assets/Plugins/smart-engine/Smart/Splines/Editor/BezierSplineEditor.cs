using Smart.Splines.Components;
using Smart.Splines.Constants;
using UnityEditor;
using UnityEngine;

namespace Smart.Splines
{
    [CustomEditor(typeof(BezierSpline))]
    public class BezierSplineEditor : Editor
    {
        private const int ButtonHeight = 35;
        private const int StepsPerCurve = 10;
        private const float DirectionScale = 0.5f;
        private const float HandleSize = 0.05f;
        private const float PickSize = 0.07f;

        private static readonly Color[] _modeColors =
        {
            Color.white,
            Color.yellow,
            Color.cyan
        };

        private BezierSpline _spline;
        private Transform _handleTransform;
        private Quaternion _handleRotation;
        private int _selectedIndex = -1;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            var buttonHeight = GUILayout.Height(ButtonHeight);
            
            _spline = (BezierSpline)target;
            EditorGUI.BeginChangeCheck();
        
            var loop = EditorGUILayout.Toggle("Loop", _spline.Loop);
        
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_spline, "Toggle Loop");
                EditorUtility.SetDirty(_spline);
                _spline.Loop = loop;
            }

            if (_selectedIndex >= 0 && _selectedIndex < _spline.ControlPointCount)
            {
                DrawSelectedPointInspector();
            }
            
            GUILayout.Space(10);
            GUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Add Curve", buttonHeight))
            {
                Undo.RecordObject(_spline, "Add Curve");
                _spline.AddCurve();
                TryRedrawDecorator();
                EditorUtility.SetDirty(_spline);
            }

            if (GUILayout.Button("Remove Curve", buttonHeight))
            {
                Undo.RecordObject(_spline, "Remove Curve");
                _spline.RemoveCurve();
                TryRedrawDecorator();
                EditorUtility.SetDirty(_spline);
            }
            
            GUILayout.EndHorizontal();
        }

        private void DrawSelectedPointInspector()
        {
            GUILayout.Label("Selected Point:", EditorStyles.boldLabel);
            EditorGUI.BeginChangeCheck();
            
            var point = EditorGUILayout.Vector3Field("Position", _spline.GetControlPoint(_selectedIndex));
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_spline, "Move Point");
                EditorUtility.SetDirty(_spline);
                _spline.SetControlPoint(_selectedIndex, point);
            }

            EditorGUI.BeginChangeCheck();
            
            var mode = (BezierControlPointMode)EditorGUILayout.EnumPopup("Mode", _spline.GetControlPointMode(_selectedIndex));
            
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_spline, "Change Point Mode");
                _spline.SetControlPointMode(_selectedIndex, mode);
                EditorUtility.SetDirty(_spline);
            }
        }

        private void OnSceneGUI()
        {
            _spline = (BezierSpline)target;
            _handleTransform = _spline.transform;
            _handleRotation = Tools.pivotRotation == PivotRotation.Local ? _handleTransform.rotation : Quaternion.identity;

            var p0 = ShowPoint(0);
            
            for (int i = 1; i < _spline.ControlPointCount; i += 3)
            {
                var p1 = ShowPoint(i);
                var p2 = ShowPoint(i + 1);
                var p3 = ShowPoint(i + 2);

                Handles.color = Color.yellow;
                Handles.DrawLine(p0, p1);
                Handles.DrawLine(p2, p3);

                Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 3f);
                p0 = p3;
            }

            //ShowDirections();
        }

        private void ShowDirections()
        {
            var point = _spline.GetPoint(0f);
            
            Handles.color = Color.green;
            Handles.DrawLine(point, point + _spline.GetDirection(0f) * DirectionScale);
            
            var steps = StepsPerCurve * _spline.CurveCount;
            
            for (var i = 1; i <= steps; i++)
            {
                point = _spline.GetPoint(i / (float) steps);
                Handles.DrawLine(point, point + _spline.GetDirection(i / (float) steps) * DirectionScale);
            }
        }

        private Vector3 ShowPoint(int index)
        {
            var point = _handleTransform.TransformPoint(_spline.GetControlPoint(index));
            var size = HandleUtility.GetHandleSize(point);
            
            Handles.color = _modeColors[(int) _spline.GetControlPointMode(index)];
            
            if (Handles.Button(point, _handleRotation, size * HandleSize, size * PickSize, Handles.DotHandleCap))
            {
                _selectedIndex = index;
                Repaint();
            }

            if (_selectedIndex == index)
            {
                EditorGUI.BeginChangeCheck();
                
                if (_spline.LockY)
                {
                    var pointY = point.y;
                    point = Handles.DoPositionHandle(point, _handleRotation);
                    point.y = pointY;
                }
                else
                {
                    point = Handles.DoPositionHandle(point, _handleRotation);
                }
                
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_spline, "Move Point");
                    _spline.SetControlPoint(index, _handleTransform.InverseTransformPoint(point));
                    EditorUtility.SetDirty(_spline);
                    TryRedrawDecorator();
                }
            }

            return point;
        }

        private void TryRedrawDecorator()
        {
            var decorator = _spline.GetComponent<SplineDecorator>();
                    
            if (decorator != null && decorator.RealtimeMode)
            {
                decorator.Redraw();
            }
        }
    }
}