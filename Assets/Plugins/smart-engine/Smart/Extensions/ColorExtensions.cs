using UnityEngine;

namespace Smart.Extensions
{
	public static class SmartColorUtil
	{
		private struct HSV
		{
			public float Hue;
			public float Saturation;
			public float Highlight;

			public HSV(float hue, float saturation, float highlight)
			{
				Hue = hue;
				Saturation = saturation;
				Highlight = highlight;
			}
		}
		
		public static float GetHue(this Color color)
		{
			return color.ToHSV().Hue;
		}
		
		public static float GetSaturation(this Color color)
		{
			return color.ToHSV().Saturation;
		}
		
		public static float GetHighlight(this Color color)
		{
			return color.ToHSV().Highlight;
		}
		
		public static Color SetHue(this Color color, float value)
		{
			var hsv = color.ToHSV();
			hsv.Hue = value;
			return hsv.ToRGB();
		}
		
		public static Color SetSaturation(this Color color, float value)
		{
			var hsv = color.ToHSV();
			hsv.Saturation = value;
			return hsv.ToRGB();
		}
		
		public static Color SetHighlight(this Color color, float value)
		{
			var hsv = color.ToHSV();
			hsv.Highlight = value;
			return hsv.ToRGB();
		}
		
		public static Color AddHue(this Color color, float value)
		{
			var hsv = color.ToHSV();
			hsv.Hue += value;
			return hsv.ToRGB();
		}
		
		public static Color AddSaturation(this Color color, float value)
		{
			var hsv = color.ToHSV();
			hsv.Saturation += value;
			return hsv.ToRGB();
		}
		
		public static Color AddHighlight(this Color color, float value)
		{
			var hsv = color.ToHSV();
			hsv.Highlight += value;
			return hsv.ToRGB();
		}
		
		public static Color SetHSV(this Color color, float hue, float saturation, float highlight)
		{
			var hsv = color.ToHSV();
			hsv.Hue *= Mathf.Clamp01(hue);
			hsv.Saturation *= Mathf.Clamp01(hue);
			hsv.Highlight *= Mathf.Clamp01(hue);
			return hsv.ToRGB();
		}
		
		private static HSV ToHSV(this Color color) 
        {
        	float high = Mathf.Max(Mathf.Max(color.r, color.g), color.b);
        	
        	if (high <= .0)
        	{
        		return new HSV(0f, 0f, 0f);
        	}
        
        	float low = Mathf.Min(Mathf.Min(color.r, color.g), color.b);
        	float delta = high - low;
        	float sat = delta / high;
        
        	float hue;
        	
        	if (color.r == high) 
        	{ 
        		hue = (color.g - color.b) / delta;
        	} 
        	else if (color.g == high) 
        	{
        		hue = 2 + (color.b - color.r) / delta;
        	}
        	else 
        	{
        		hue = 4 + (color.r - color.g) / delta;
        	}
        
        	hue /= 6.0f;
        	
		    if (hue < 0)
		    {
		    	hue += 1.0f;
			}
			
			if (hue > 1)
			{
				hue -= 1.0f;
			}
        
        	return new HSV(hue, sat, high);
        }
    
        private static Color ToRGB(this HSV hsv) 
        {
        	float h = Fmod(hsv.Hue, 1.0f);
        	float s = hsv.Saturation;
        	float v = hsv.Highlight;
    
        	if (s == 0)
        	{
        		return new Color(v, v, v);
        	}
        
        	h *= 6.0f;
        	
        	var i = Mathf.FloorToInt(h);
        	var f = h - i;
        	var p = v * (1.0f-s);
        	var q = v * (1.0f-s * f);
        	var t = v * (1.0f - s * (1.0f - f) );
        
        	switch (i)
        	{
        		case 0:  return new Color(v, t, p);
        		case 1:  return new Color(q, v, p);
        		case 2:  return new Color(p, v, t);
        		case 3:  return new Color(p, q, v);
        		case 4:  return new Color(t, p, v);
        		default: return new Color(v, p, q);
        	}
        }
        
        private static float Fmod(float a, float b)
        {
        	float c = Frac(Mathf.Abs(a / b)) * Mathf.Abs(b);
        	return a < 0 ? -c : c;
        }
        
        private static float Frac(float v)
        {
        	return v - Mathf.Floor(v);
        }
	}
}