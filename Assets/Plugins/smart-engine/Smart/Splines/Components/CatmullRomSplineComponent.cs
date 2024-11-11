using System.Collections.Generic;
using Smart.Basics.Extensions;
using Smart.Splines.DataTypes;
using UnityEngine;

namespace Smart.Splines.Components
{
    [ExecuteAlways]
    public class CatmullRomSplineComponent : MonoBehaviour
    {
        [SerializeField] [Range(2, 64)] private int _resolution = 16;
        [SerializeField] private bool _isClosed;
        [SerializeField] private List<Vector3> _controlPoints = new List<Vector3>{new Vector3(0, 0), new Vector3(0, 1), new Vector3(0, 2)};
        [SerializeField] private Color _color = Color.white;
        
        [HideInInspector] [SerializeField] private CatmullRomSpline _spline = new CatmullRomSpline();
        
        public int ControlPointsCount => _controlPoints?.Count ?? 0;

        private void Start()
        {
            Recalculate();
            DrawSpline();
        }

        private void OnDrawGizmos()
        {
            DrawSpline();
        }

        public Vector3 GetControlPoint(int index)
        {
            return _controlPoints[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            _controlPoints[index] = point;
        }
        
        public void Recalculate()
        {
            if (!_controlPoints.IsNullOrEmpty())
            {
                _spline.Setup(_controlPoints, _resolution, _isClosed);
            }
        }

        public void DrawSpline()
        {
            var points = _spline.GetPoints();

            for (var i = 0; i < points.Length; i++)
            {
                if (i == points.Length - 1 && _isClosed)
                {
                    DrawLine(points[i].Position, points[0].Position, _color);
                }
                else if (i < points.Length - 1)
                {
                    DrawLine(points[i].Position, points[i + 1].Position, _color);
                }
            }
        }
        
        public void DrawNormals(float extrusion, Color color)
        {
            var points = _spline.GetPoints();
            
            for(int i = 0; i < points.Length; i++)
            {
                DrawLine(points[i].Position, points[i].Position + points[i].Normal * extrusion, color);
            }
        }

        public void DrawTangents(float extrusion, Color color)
        {
            var points = _spline.GetPoints();
            
            for(int i = 0; i < points.Length; i++)
            {
                DrawLine(points[i].Position, points[i].Position + points[i].Tangent * extrusion, color);
            }
        }
        
        private void DrawLine(Vector3 pointA, Vector3 pointB, Color color)
        {
            var position = transform.position;
            Debug.DrawLine(position + pointA, position + pointB, color);
        }
    }
}