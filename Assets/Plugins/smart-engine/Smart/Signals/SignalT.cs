using System;
using System.Collections.Generic;

namespace Smart.Signals
{	
	public class Signal<T> : Signal
	{
		private readonly List<SignalListener<T>> _listeners;
		private readonly Dictionary<Action<T>, SignalListener<T>> _dictionary;
  
		public Signal()
		{
			_listeners = new List<SignalListener<T>>();
			_dictionary = new Dictionary<Action<T>, SignalListener<T>>();
		}

		public override void Clear() 
		{
			base.Clear();
			_listeners.Clear();
			_dictionary.Clear();
		}

		public void AddListener(Action<T> action, bool once = false) 
		{
			if (!_dictionary.ContainsKey(action))
			{
				var listener = new SignalListener<T>(action, once);
				_listeners.Add(listener);
				_dictionary.Add(action, listener);
			}
		}

		public void RemoveListener(Action<T> action)
		{
			if (_dictionary.ContainsKey(action))
			{
				var listener = _dictionary[action];
				_listeners.Remove(listener);
				_dictionary.Remove(action);
				listener.Dispose();
			}
		}

		public void AddOnce(Action<T> action)
		{
			AddListener(action, true);
		}

		public bool Has(Action<T> action) 
		{
			return _dictionary.ContainsKey(action);
		}

		public void Invoke(T value) 
		{
			for (var i = 0; i < _listeners.Count; i++)
			{
				var item = _listeners[i];
				item.Action(value);
				
				if (item.Once) 
				{
					RemoveListener(item.Action);
					i--;
				}
			}
			
			Invoke();
		}
	}
}