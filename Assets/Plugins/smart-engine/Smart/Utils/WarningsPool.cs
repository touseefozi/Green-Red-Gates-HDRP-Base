using System.Collections.Generic;
using UnityEngine;

namespace MyBox
{
	public static class WarningsPool
	{
		public static bool Log(string message, Object target = null)
		{
			if (Pool.Contains(message)) return false;

			if (target != null) Debug.Log(message, target);
			else Debug.Log(message);

			Pool.Add(message);
			return true;
		}

		public static bool LogWarning(string message, Object target = null)
		{
			if (Pool.Contains(message)) return false;

			if (target != null) Debug.LogWarning(message, target);
			else Debug.LogWarning(message);

			Pool.Add(message);
			return true;
		}

		public static bool LogError(string message, Object target = null)
		{
			if (Pool.Contains(message)) return false;

			if (target != null) Debug.LogError(message, target);
			else Debug.LogError(message);

			Pool.Add(message);
			return true;
		}

		private static readonly HashSet<string> Pool = new HashSet<string>();
	}
}