using System;
using UnityEngine;

namespace Smart.Pooling
{
	public abstract class PoolItem : IDisposable
	{
		public bool isDisposed { get; internal set; }
		public float instanceID { get; private set; }
		public float disposeCounter { get; private set; }
	
		private Action<PoolItem> _disposeAction;
	
		internal void Instantiate(Action<PoolItem> disposeAction, float id)
		{
			_disposeAction = disposeAction;
			isDisposed = false;
			instanceID = id;
		}
	
		public void Dispose()
		{
			if (!isDisposed)
			{
				isDisposed = true;
				++disposeCounter;
				OnDisposed();
				_disposeAction(this);
			}
		}
	
		protected abstract void OnDisposed();
	}
}