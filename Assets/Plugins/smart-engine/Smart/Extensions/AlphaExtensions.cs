using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Smart.Extensions
{
    public static class AlphaExtensions
    {
        public static IEnumerator TweenAlpha(this CanvasGroup source, float startValue, float endValue, float time)
        {
            var timer = 0f;
            
            source.alpha = startValue;
            
            while (timer < time)
            {
                timer += Time.unscaledDeltaTime;
                source.alpha = startValue + timer / time * (endValue - startValue);
                yield return null;
            }
        }
        
        public static IEnumerator TweenAlpha(this Image source, float startValue, float endValue, float time)
        {
        	var color = source.color;
            var timer = 0f;
            
            source.SetAlpha(startValue);
            
            while (timer < time)
            {
                timer += Time.unscaledDeltaTime;
                color.a = startValue + timer / time * (endValue - startValue);
                source.color = color;
                yield return null;
            }
        }
        
        public static IEnumerator TweenAlpha(this RawImage source, float startValue, float endValue, float time)
        {
        	var color = source.color;
            var timer = 0f;
            
            source.SetAlpha(startValue);
            
            while (timer < time)
            {
                timer += Time.unscaledDeltaTime;
                color.a = startValue + timer / time * (endValue - startValue);
                source.color = color;
                yield return null;
            }
        }
        
        public static IEnumerator TweenAlpha(this SpriteRenderer source, float startValue, float endValue, float time)
        {
        	var color = source.color;
            var timer = 0f;
            
            source.SetAlpha(startValue);
            
            while (timer < time)
            {
                timer += Time.unscaledDeltaTime;
                color.a = startValue + timer / time * (endValue - startValue);
                source.color = color;
                yield return null;
            }
        }
        
        public static IEnumerator TweenAlpha(this MeshRenderer source, float startValue, float endValue, float time)
        {
            var material = source.material;
        	var color = material.color;
            var timer = 0f;
            
            source.SetAlpha(startValue);
            
            while (timer < time)
            {
                timer += Time.unscaledDeltaTime;
                color.a = startValue + timer / time * (endValue - startValue);
                material.color = color;
                yield return null;
            }
        }
        
        // set alpha
        
        public static void SetAlpha(this Image source, float value)
        {
        	var color = source.color;
            color.a = value;
            source.color = color;
        }
        
        public static void SetAlpha(this RawImage source, float value)
        {
        	var color = source.color;
            color.a = value;
            source.color = color;
        }
        
        public static void SetAlpha(this SpriteRenderer source, float value)
        {
        	var color = source.color;
            color.a = value;
            source.color = color;
        }
        
        public static void SetAlpha(this MeshRenderer source, float value)
        {
            var material = source.material;
        	var color = material.color;
            color.a = value;
            material.color = color;
        }
        
        public static void SetAlpha(this Text source, float value)
        {
        	var color = source.color;
            color.a = value;
            source.color = color;
        }
        
        public static void SetStartAlpha(this LineRenderer source, float value)
        {
            var color = source.startColor;
            color.a = value;
            source.startColor = color;
        }
        
        public static void SetEndAlpha(this LineRenderer source, float value)
        {
            var color = source.endColor;
            color.a = value;
            source.endColor = color;
        }
        
        public static void SetStartAlpha(this TrailRenderer source, float value)
        {
            var color = source.startColor;
            color.a = value;
            source.startColor = color;
        }
        
        public static void SetEndAlpha(this TrailRenderer source, float value)
        {
            var color = source.endColor;
            color.a = value;
            source.endColor = color;
        }
        
        public static float GetAlpha(this Image source)
        {
        	return source.color.a;
        }
        
        public static float GetAlpha(this SpriteRenderer source)
        {
        	return source.color.a;
        }
        
        public static float GetAlpha(this MeshRenderer source)
        {
            return source.material.color.a;
        }
        
        public static float GetAlpha(this Text source)
        {
            return source.color.a;
        }
    }
}