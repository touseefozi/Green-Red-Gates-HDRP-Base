using UnityEngine;

namespace Smart.Utils
{
	public static class SmartGUI
	{
		public static class Colors
		{
			public static readonly Color Red = new Color(.8f, .6f, .6f);
			public static readonly Color Green = new Color(.4f, .6f, .4f);
			public static readonly Color Blue = new Color(.6f, .6f, .8f);
			public static readonly Color Gray = new Color(.3f, .3f, .3f);
			public static readonly Color Yellow = new Color(.8f, .8f, .2f, .6f);
			public static readonly Color Brown = new Color(.7f, .5f, .2f, .6f);
		}
		
		public static void DrawColouredRect(Rect rect, Color color)
		{
			var defaultBackgroundColor = GUI.backgroundColor;
			GUI.backgroundColor = color;
			GUI.Box(rect, "");
			GUI.backgroundColor = defaultBackgroundColor;
		}
	}
}