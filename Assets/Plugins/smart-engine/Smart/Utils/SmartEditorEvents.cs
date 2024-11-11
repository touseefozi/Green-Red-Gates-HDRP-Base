#if UNITY_EDITOR
using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Smart.Utils
{
	[InitializeOnLoad]
	public class SmartEditorEvents : UnityEditor.AssetModificationProcessor
	{
		public static event Action OnSave;
		public static event Action OnFirstFrame;
		public static event Action<Event> OnEditorInput;
		
		static SmartEditorEvents()
		{
			EditorApplication.update += CheckOnceOnPlaymode;
			RegisterRawInputHandler();


			void RegisterRawInputHandler()
			{
				var globalEventHandler = typeof(EditorApplication).GetField("globalEventHandler",
					BindingFlags.Static | BindingFlags.NonPublic);
				if (globalEventHandler == null) return;
				var callback = (EditorApplication.CallbackFunction)globalEventHandler.GetValue(null);
				callback += RawInputHandler;
				globalEventHandler.SetValue(null, callback);
			}
		}

		private static string[] OnWillSaveAssets(string[] paths)
		{
			if (paths.Length == 1 && (paths[0] == null || paths[0].EndsWith(".prefab")))
			{
				return paths;
			}

			OnSave?.Invoke();

			return paths;
		}

		private static void CheckOnceOnPlaymode()
		{
			if (Application.isPlaying)
			{
				EditorApplication.update -= CheckOnceOnPlaymode;
				OnFirstFrame?.Invoke();
			}
		}
		
		private static void RawInputHandler()
		{
			var e = Event.current;
			if (e.type != EventType.KeyDown || e.keyCode == KeyCode.None) return;

			OnEditorInput?.Invoke(e);
		}
	}
}
#endif