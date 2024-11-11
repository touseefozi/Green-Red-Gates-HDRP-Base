using Smart.Splines.Constants;
using UnityEngine;

namespace Smart.Splines.Components
 {
     public class SplineWalker : MonoBehaviour
     {
         [SerializeField] private SplineWalkerMode _mode;
         [SerializeField] private BezierSpline _spline;
         [SerializeField] private float _duration;
         [SerializeField] private bool _lookForward;

         private float _progress;
         private bool _goingForward = true;

         private void Update()
         {
             if (_goingForward)
             {
                 _progress += Time.deltaTime / _duration;
            
                 if (_progress > 1f)
                 {
                     if (_mode == SplineWalkerMode.Once)
                     {
                         _progress = 1f;
                     }
                     else if (_mode == SplineWalkerMode.Loop)
                     {
                         _progress -= 1f;
                     }
                     else
                     {
                         _progress = 2f - _progress;
                         _goingForward = false;
                     }
                 }
             }
             else
             {
                 _progress -= Time.deltaTime / _duration;
            
                 if (_progress < 0f)
                 {
                     _progress = -_progress;
                     _goingForward = true;
                 }
             }

             var position = _spline.GetPoint(_progress);
             transform.localPosition = position;
        
             if (_lookForward)
             {
                 transform.LookAt(position + _spline.GetDirection(_progress));
             }
         }
     }
 }