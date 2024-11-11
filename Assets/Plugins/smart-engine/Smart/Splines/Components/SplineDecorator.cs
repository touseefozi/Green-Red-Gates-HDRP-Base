using System.Collections.Generic;
using Smart.Basics.Extensions;
using UnityEngine;

namespace Smart.Splines.Components
{
    [ExecuteAlways]
    public class SplineDecorator : MonoBehaviour
    { 
        public const string UnselectableTag = "Unselectable";
        
        [SerializeField] private BezierSpline _spline;
        [SerializeField] private Transform _prefab;
        [SerializeField] private int _frequency = 10;
        [SerializeField] private bool _lookForward = true;
        [SerializeField] private bool _realtimeMode = true;

        [HideInInspector] [SerializeField] private List<Transform> _items = new List<Transform>();

        public List<Transform> Items => _items;

        public bool RealtimeMode => _realtimeMode;
        
        private void Awake()
        {
            if (!_realtimeMode)
            {
                Redraw();
            }
        }

        public void Clear()
        {
            var children = GetComponentsInChildren<Transform>();
            
            foreach (var child in children)
            {
                if (child != transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
            
            _items.Clear();
        }

        public void Redraw()
        {
            if (_frequency > 0 && _prefab != null && _spline != null)
            {
                var stepSize = _spline.Loop || _frequency == 1 ? 1f / _frequency : 1f / (_frequency - 1);
                var pointIndex = 0;

                UpdateItemsCount();

                foreach (var item in _items)
                {
                    var position = _spline.GetPoint(pointIndex * stepSize);
                    item.localPosition = transform.InverseTransformPoint(position);

                    if (_lookForward)
                    {
                        item.LookAt(position + _spline.GetDirection(pointIndex * stepSize));
                    }

                    pointIndex++;
                }
            }
        }

        private void UpdateItemsCount()
        {
            for (var i = 0; i < _items.Count; i++)
            {
                var item = _items[i];

                if (item == null || item.gameObject == null)
                {
                    Clear();
                    break;
                }
            }
            
            while (_items.Count < _frequency)
            {
                var item = InstantiateItem(transform);
                var gameObject = item.gameObject;
                gameObject.tag = UnselectableTag;
                gameObject.hideFlags = HideFlags.NotEditable;
                _items.Add(item);
            }

            while (_items.Count > _frequency)
            {
                var item = _items.Pop();
                DestroyImmediate(item.gameObject);
            }
        }

        private Transform InstantiateItem(Transform transform)
        {
#if UNITY_EDITOR
            return UnityEditor.PrefabUtility.InstantiatePrefab(_prefab, transform) as Transform;
#else
            return Instantiate(_prefab, transform);
#endif
        }
    }
}