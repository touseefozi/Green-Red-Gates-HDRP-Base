using System;
using System.Collections.Generic;
using Smart.Basics.Extensions;
using UnityEngine;

namespace Smart.Splines.DataTypes
{
    [Serializable]
    public class CatmullRomSpline
    {
        [SerializeField] private int _resolution = 16;
        [SerializeField] private bool _isClosed;
        [SerializeField] private Vector3[] _controlPoints;
        
        private CatmullRomSplinePoint[] _splinePoints = new CatmullRomSplinePoint[0];
        
        public CatmullRomSpline()
        {
        }
        
        public CatmullRomSpline(List<Vector3> controlPoints, int resolution, bool isClosed = false)
        {
            Setup(controlPoints, resolution, isClosed);
        }
        
        public void Setup(List<Vector3> controlPoints, int resolution, bool isClosed = false)
        {
            if (controlPoints.IsNullOrEmpty())
            {
                throw new ArgumentException("Invalid control points");
            }
            
            if (resolution < 2)
            {
                throw new ArgumentException("Invalid Resolution. Make sure it's >= 1");
            }
            
            _controlPoints = controlPoints.ToArray();
            _resolution = resolution;
            _isClosed = isClosed;
            GenerateSplinePoints();
        }
        
        public void SetControlPoints(List<Vector3> controlPoints)
        {
            if (controlPoints.IsNullOrEmpty())
            {
                throw new ArgumentException("Invalid control points");
            }

            _controlPoints = controlPoints.ToArray();
            GenerateSplinePoints();
        }

        public Vector3 GetControlPoint(int index)
        {
            return _controlPoints[index];
        }

        public void SetControlPoint(int index, Vector3 point)
        {
            _controlPoints[index] = point;
            GenerateSplinePoints();
        }

        public void SetResolution(int value)
        {
            if (value < 2)
            {
                throw new ArgumentException("Invalid Resolution. Make sure it's >= 1");
            }

            _resolution = value;
            GenerateSplinePoints();
        }

        public void SetIsClosed(bool value)
        {
            _isClosed = value;
            GenerateSplinePoints();
        }

        public CatmullRomSplinePoint[] GetPoints()
        {
            return _splinePoints;
        }

        private void GenerateSplinePoints()
        {
            var length = _isClosed ? _resolution * _controlPoints.Length : _resolution * (_controlPoints.Length - 1);
            _splinePoints = new CatmullRomSplinePoint[length];

            Vector3 p0;
            Vector3 p1;
            Vector3 m0;
            Vector3 m1;

            var closedAdjustment = _isClosed ? 0 : 1;
            
            for (int currentPoint = 0; currentPoint < _controlPoints.Length - closedAdjustment; currentPoint++)
            {
                var closedLoopFinalPoint = _isClosed && currentPoint == _controlPoints.Length - 1;

                p0 = _controlPoints[currentPoint];

                if (closedLoopFinalPoint)
                {
                    p1 = _controlPoints[0];
                }
                else
                {
                    p1 = _controlPoints[currentPoint + 1];
                }

                // m0
                if (currentPoint == 0) // Tangent M[k] = (P[k+1] - P[k-1]) / 2
                {
                    if (_isClosed)
                    {
                        m0 = p1 - _controlPoints[_controlPoints.Length - 1];
                    }
                    else
                    {
                        m0 = p1 - p0;
                    }
                }
                else
                {
                    m0 = p1 - _controlPoints[currentPoint - 1];
                }

                // m1
                if (_isClosed)
                {
                    if (currentPoint == _controlPoints.Length - 1) //Last point case
                    {
                        m1 = _controlPoints[(currentPoint + 2) % _controlPoints.Length] - p0;
                    }
                    else if (currentPoint == 0) //First point case
                    {
                        m1 = _controlPoints[currentPoint + 2] - p0;
                    }
                    else
                    {
                        m1 = _controlPoints[(currentPoint + 2) % _controlPoints.Length] - p0;
                    }
                }
                else
                {
                    if (currentPoint < _controlPoints.Length - 2)
                    {
                        m1 = _controlPoints[(currentPoint + 2) % _controlPoints.Length] - p0;
                    }
                    else
                    {
                        m1 = p1 - p0;
                    }
                }

                m0 *= 0.5f;
                m1 *= 0.5f;

                var pointStep = 1.0f / _resolution;

                if ((currentPoint == _controlPoints.Length - 2 && !_isClosed) || closedLoopFinalPoint)
                {
                    pointStep = 1.0f / (_resolution - 1);
                }

                for (var tesselatedPoint = 0; tesselatedPoint < _resolution; tesselatedPoint++)
                {
                    var t = tesselatedPoint * pointStep;
                    var point = CreateSplinePoint(p0, p1, m0, m1, t);

                    _splinePoints[currentPoint * _resolution + tesselatedPoint] = point;
                }
            }
        }

        private CatmullRomSplinePoint CreateSplinePoint(Vector3 start, Vector3 end, Vector3 tanPoint1, Vector3 tanPoint2, float t)
        {
            var position = CalculatePosition(start, end, tanPoint1, tanPoint2, t);
            var tangent = CalculateTangent(start, end, tanPoint1, tanPoint2, t);
            var normal = NormalFromTangent(tangent);

            return new CatmullRomSplinePoint(position, tangent, normal);
        }

        private Vector3 CalculatePosition(Vector3 start, Vector3 end, Vector3 tanPoint1, Vector3 tanPoint2, float t)
        {
            var doubleT = t * t;
            var tripleT = doubleT * t;
            var position = (2.0f * tripleT - 3.0f * doubleT + 1.0f) * start
                               + (tripleT - 2.0f * doubleT + t) * tanPoint1
                               + (-2.0f * tripleT + 3.0f * doubleT) * end
                               + (tripleT - doubleT) * tanPoint2;

            return position;
        }

        private Vector3 CalculateTangent(Vector3 start, Vector3 end, Vector3 tanPoint1, Vector3 tanPoint2, float t)
        {
            var doubleT = t * t;
            var tangent = (6 * doubleT - 6 * t) * start
                              + (3 * doubleT - 4 * t + 1) * tanPoint1
                              + (-6 * doubleT + 6 * t) * end
                              + (3 * doubleT - 2 * t) * tanPoint2;

            return tangent.normalized;
        }

        private Vector3 NormalFromTangent(Vector3 tangent)
        {
            return Vector3.Cross(tangent, Vector3.up).normalized / 2;
        }
    }
}