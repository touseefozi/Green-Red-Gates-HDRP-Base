using System;

namespace Smart.Signals
{
	internal class SignalListener<T> : IDisposable
	{
		public Action<T> Action;
		public bool Once;
		
		public SignalListener(Action<T> action, bool once = false) 
		{
			Action = action;
			Once = once;
		}

		public void Dispose()
		{
			Action = null;
			Once = false;
		}
	}
}