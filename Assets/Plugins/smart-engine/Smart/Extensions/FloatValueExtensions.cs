using System.Collections;
using Smart.DataTypes;
using UnityEngine;

namespace Smart.Extensions
{
    public static class FloatValueExtensions
    {
        public static IEnumerator TweenValue(this FloatValue floatValue, float startValue, float endValue, float time, AnimationCurve curve = null)
        {
            var timer = 0f;
            var ratio = 0f;
            
            floatValue.Value = startValue;
            
            while (timer < time)
            {
                timer += Time.unscaledDeltaTime;
                ratio = timer / time;
                
                if (curve != null)
                {
                    ratio = curve.Evaluate(ratio);
                }
                
                floatValue.Value = startValue + ratio * (endValue - startValue);
                yield return null;
            }
        }

        public static IEnumerator TweenValueClamped(this FloatValue floatValue, float startValue, float endValue, float time, AnimationCurve curve = null)
        {
            var timer = 0f;
            var ratio = 0f;
            
            floatValue.Value = startValue;
            
            while (timer < time)
            {
                timer += Time.unscaledDeltaTime;
                ratio = Mathf.Clamp01(timer / time);
                
                if (curve != null)
                {
                    ratio = curve.Evaluate(ratio);
                }
                
                floatValue.Value = startValue + ratio * (endValue - startValue);
                yield return null;
            }
        }
    }
}