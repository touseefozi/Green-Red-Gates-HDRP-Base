using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace SmartEditor.Utils
{
	public static class AnimationWindowHelper
	{
		private const BindingFlags _flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

		public static AnimationClip GetAnimationWindowCurrentClip()
	    {
		    return GetProperty("activeAnimationClip") as AnimationClip; 
	    }
			
		public static int GetCurrentFrame()
	    {
		    var frame = GetProperty("currentFrame");
	        return frame != null ? (int) frame : -1;
	    }
	 
		public static float GetCurrentTime()
	    {
		    var frame = GetProperty("currentTime");
		    return frame != null ? (float) frame : 0f;
	    }

		public static bool IsRecordingMode()
		{
			var value = GetProperty("recording");
			return value != null && (bool) value;
		}
		
		private static object GetProperty(string propertyName)
		{
			var editorAssembly = typeof (Editor).Assembly;
			var windowType = editorAssembly.GetType("UnityEditorInternal.AnimationWindowState");
			var property = windowType.GetProperty(propertyName, _flags);
			var windows = Resources.FindObjectsOfTypeAll(windowType);
			var window = windows.FirstOrDefault();
			
			return window != null && property != null ? property.GetValue(window, null) : null;
		}
		
		private static void SetProperty(string propertyName, object value)
		{
			var editorAssembly = typeof (Editor).Assembly;
			var windowType = editorAssembly.GetType("UnityEditorInternal.AnimationWindowState");
			var property = windowType.GetProperty(propertyName, _flags);
			var windows = Resources.FindObjectsOfTypeAll(windowType);
			var window = windows.FirstOrDefault();

			if (window != null && property != null)
			{
				property.SetValue(window, value);
			}
		}
	}
}