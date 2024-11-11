using System;

namespace Smart.Signals
{
	internal class SignalListener : IDisposable
	{
		public Action action;
		public bool once;
		
		public SignalListener(Action action, bool once = false) 
		{
			this.action = action;
			this.once = once;
		}

		public void Dispose()
		{
			action = null;
			once = false;
		}
	}
}