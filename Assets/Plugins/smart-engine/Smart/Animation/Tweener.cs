using System;
using System.Collections;
using UnityEngine;

namespace Smart.Animation
{
    public static class Tweener
    {
        public static IEnumerator DelayedCall(float totalTime, Action completeAction)
        {
            yield return new WaitForSecondsRealtime(totalTime);
            completeAction();
        }
        
        public static IEnumerator UpdateDuringTime(float totalTime, Action updateAction)
        {
            var time = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                updateAction();
                yield return null;
            }
        }
        
        public static IEnumerator UpdateDuringTimeUnscaled(float totalTime, Action updateAction)
        {
            var time = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                updateAction();
                yield return null;
            }
        }
        
        public static IEnumerator Tween(float totalTime, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                updateAction(ratio);
                yield return null;
            }
        }
        
        public static IEnumerator TweenUnscaled(float totalTime, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            
            while (time < totalTime)
            {
                time += Time.unscaledDeltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                updateAction(ratio);
                yield return null;
            }
        }
        
        public static IEnumerator Tween(float totalTime, AnimationCurve curve, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                ratio = curve.Evaluate(ratio);
                updateAction(ratio);
                yield return null;
            }
        }
        
        public static IEnumerator TweenUnscaled(float totalTime, AnimationCurve curve, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            
            while (time < totalTime)
            {
                time += Time.unscaledDeltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                ratio = curve.Evaluate(ratio);
                updateAction(ratio);
                yield return null;
            }
        }
        
        public static IEnumerator Tween(float totalTime, Func<float, float> easing, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                ratio = easing(ratio);
                updateAction(ratio);
                yield return null;
            }
        }
        
        public static IEnumerator TweenUnscaled(float totalTime, Func<float, float> easing, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            
            while (time < totalTime)
            {
                time += Time.unscaledDeltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                ratio = easing(ratio);
                updateAction(ratio);
                yield return null;
            }
        }
        
        public static IEnumerator TweenValue(float startValue, float endValue, float totalTime, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            var value = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                value = Mathf.LerpUnclamped(startValue, endValue, ratio);
                updateAction(value);
                yield return null;
            }
        }
        
        public static IEnumerator TweenValueUnscaled(float startValue, float endValue, float totalTime, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            var value = 0f;
            
            while (time < totalTime)
            {
                time += Time.unscaledDeltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                value = Mathf.LerpUnclamped(startValue, endValue, ratio);
                updateAction(value);
                yield return null;
            }
        }
        
        public static IEnumerator TweenValue(float startValue, float endValue, float totalTime, AnimationCurve curve, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            var value = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                ratio = curve.Evaluate(ratio);
                value = Mathf.LerpUnclamped(startValue, endValue, ratio);
                updateAction(value);
                yield return null;
            }
        }
        
        public static IEnumerator TweenValueUnscaled(float startValue, float endValue, float totalTime, AnimationCurve curve, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            var value = 0f;
            
            while (time < totalTime)
            {
                time += Time.unscaledDeltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                ratio = curve.Evaluate(ratio);
                value = Mathf.LerpUnclamped(startValue, endValue, ratio);
                updateAction(value);
                yield return null;
            }
        }
        
        public static IEnumerator TweenValue(float startValue, float endValue, float totalTime, Func<float, float> easing, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            var value = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                ratio = easing(ratio);
                value = Mathf.LerpUnclamped(startValue, endValue, ratio);
                updateAction(value);
                yield return null;
            }
        }
        
        public static IEnumerator TweenValueUnscaled(float startValue, float endValue, float totalTime, Func<float, float> easing, Action<float> updateAction)
        {
            var time = 0f;
            var ratio = 0f;
            var value = 0f;
            
            while (time < totalTime)
            {
                time += Time.unscaledDeltaTime;
                ratio = Mathf.Clamp01(time / totalTime);
                ratio = easing(ratio);
                value = Mathf.LerpUnclamped(startValue, endValue, ratio);
                updateAction(value);
                yield return null;
            }
        }
        
        public static IEnumerator TweenAlphaToOne(this CanvasGroup canvasGroup, float totalTime)
        {
            var time = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(time / totalTime);
                yield return null;
            }
        }
        
        public static IEnumerator TweenAlphaToZero(this CanvasGroup canvasGroup, float totalTime)
        {
            var time = 0f;
            
            while (time < totalTime)
            {
                time += Time.deltaTime;
                canvasGroup.alpha = Mathf.Clamp01(1f - time / totalTime);
                yield return null;
            }
        }
        
        public static IEnumerator TweenAlphaToOneUnscaled(this CanvasGroup canvasGroup, float totalTime)
        {
            var time = 0f;
            
            while (time < totalTime)
            {
                time += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(time / totalTime);
                yield return null;
            }
        }
        
        public static IEnumerator TweenAlphaToZeroUnscaled(this CanvasGroup canvasGroup, float totalTime)
        {
            var time = 0f;
            
            while (time < totalTime)
            {
                time += Time.unscaledDeltaTime;
                canvasGroup.alpha = Mathf.Clamp01(1f - time / totalTime);
                yield return null;
            }
        }
    }
}