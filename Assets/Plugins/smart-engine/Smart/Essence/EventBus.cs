using System;
using System.Collections.Generic;
using Smart.Signals;

namespace Smart.Essence
{
	public class EventBus
	{
		private readonly Dictionary<int, ISignal> _signals;
	
		public EventBus()
		{
			_signals = new Dictionary<int, ISignal>();
		}

		public void AddListener(SmartEvent smartEvent, Action callback)
		{
			Signal signal;

			if (!HasSignal(smartEvent))
			{
				signal = new Signal();
				_signals.Add(smartEvent.Hash, signal);
			}
			else
			{
				signal = GetSignal(smartEvent);
			}
			signal.AddListener(callback);
		}

		public void RemoveListener(SmartEvent smartEvent, Action callback)
		{
			if (HasSignal(smartEvent))
			{
				var signal = GetSignal(smartEvent);
				signal.RemoveListener(callback);
			}
		}

		public void ReceiveEvent(SmartEvent smartEvent)
		{
			if (HasSignal(smartEvent))
			{
				var signal = GetSignal(smartEvent);
				signal.Invoke();
			}
		}

		private Signal GetSignal(IEvent @event)
		{
			return (Signal) _signals[@event.Hash];
		}

		// *** GENERIC *** //

		public void AddListener<T>(SmartEvent<T> @event, Action<T> callback)
		{
			Signal<T> signal;

			if (!HasSignal(@event))
			{
				signal = new Signal<T>();
				_signals.Add(@event.Hash, signal);
			}
			else
			{
				signal = GetSignal(@event);
			}
			signal.AddListener(callback);
		}

		public void RemoveListener<T>(SmartEvent<T> @event, Action<T> callback)
		{
			if (HasSignal(@event))
			{
				var signal = GetSignal(@event);
				signal.RemoveListener(callback);
			}
		}
		public void AddListener<T>(SmartEvent<T> @event, Action callback)
		{
			Signal<T> signal;

			if (!HasSignal(@event))
			{
				signal = new Signal<T>();
				_signals.Add(@event.Hash, signal);
			}
			else
			{
				signal = GetSignal(@event);
			}
			signal.AddListener(callback);
		}

		public void RemoveListener<T>(SmartEvent<T> @event, Action callback)
		{
			if (HasSignal(@event))
			{
				var signal = GetSignal(@event);
				signal.RemoveListener(callback);
			}
		}

		private Signal<T> GetSignal<T>(SmartEvent<T> @event)
		{
			return (Signal<T>) _signals[@event.Hash];
		}
		
		public void ReceiveEvent<T>(SmartEvent<T> @event, T value)
		{
			if (HasSignal(@event))
			{
				var signal = GetSignal(@event);
				signal.Invoke(value);
			}
		}

		// *** COMMON ***
		
		public void RemoveAllEvents()
		{
			foreach (var signal in _signals.Values)
			{
				signal.Clear();
			}
			
			_signals.Clear();
		}

		private bool HasSignal(IEvent @event)
		{
			return _signals.ContainsKey(@event.Hash);
		}
	}
}