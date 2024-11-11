using System;
using UnityEngine;

namespace Smart.Splines.DataTypes
{
    [Serializable]
    public readonly struct CatmullRomSplinePoint
    {
        public readonly Vector3 Position;
        public readonly Vector3 Tangent;
        public readonly Vector3 Normal;

        public CatmullRomSplinePoint(Vector3 position, Vector3 tangent, Vector3 normal)
        {
            Position = position;
            Tangent = tangent;
            Normal = normal;
        }
    }
}