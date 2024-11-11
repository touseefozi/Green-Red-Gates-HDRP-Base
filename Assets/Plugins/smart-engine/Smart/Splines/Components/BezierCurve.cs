using Smart.Splines.Utils;
using UnityEngine;

namespace Smart.Splines.Components
{
    public class BezierCurve : MonoBehaviour
    {
        [SerializeField] private Vector3[] _points;
        
        public Vector3[] Points => _points;

        public void Reset()
        {
            _points = new []
            {
                new Vector3(1f, 0f, 0f),
                new Vector3(2f, 0f, 0f),
                new Vector3(3f, 0f, 0f),
                new Vector3(4f, 0f, 0f)
            };
        }

        public Vector3 GetPoint(float t)
        {
            return transform.TransformPoint(BezierMath.GetCubicBezierPoint(_points[0], _points[1], _points[2], _points[3], t));
        }

        public Vector3 GetVelocity(float t)
        {
            return transform.TransformPoint(BezierMath.GetFirstDerivative(_points[0], _points[1], _points[2], _points[3], t)) - transform.position;
        }

        public Vector3 GetDirection(float t)
        {
            return GetVelocity(t).normalized;
        }
    }
}