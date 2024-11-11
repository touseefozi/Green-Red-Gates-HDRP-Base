using System;
using System.Collections.Generic;

namespace Smart.Signals
{	
	public class Signal : ISignal
	{
		private readonly List<SignalListener> _listeners;
		private readonly Dictionary<Action, SignalListener> _dictionary;
		
		public Signal()
		{
			_listeners = new List<SignalListener>();
			_dictionary = new Dictionary<Action, SignalListener>();
		}
		
		public void Dispose()
		{
			Clear();
		}

		public virtual void Clear() 
		{
			_listeners.Clear();
			_dictionary.Clear();
		}

		public void AddListener(Action action, bool once = false) 
		{
			if (!_dictionary.ContainsKey(action))
			{
				var listener = new SignalListener(action, once);
				_listeners.Add(listener);
				_dictionary.Add(action, listener);
			}
		}

		public void RemoveListener(Action action)
		{
			if (_dictionary.ContainsKey(action))
			{
				var listener = _dictionary[action];
				_listeners.Remove(listener);
				_dictionary.Remove(action);
				listener.Dispose();
			}
		}

		public void AddOnce(Action action)
		{
			AddListener(action, true);
		}

		public bool Has(Action action) 
		{
			return _dictionary.ContainsKey(action);
		}

		public void Invoke() 
		{
			for (var i = 0; i < _listeners.Count; i++)
			{
				var item = _listeners[i];
				item.action();
				
				if (item.once) 
				{
					RemoveListener(item.action);
					i--;
				}
			}
		}
	}
}