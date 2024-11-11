using System;
using System.Collections.Generic;

namespace Smart.Pooling
{
	public class Pool<T> where T : PoolItem
	{
		private readonly Stack<T> _pool;
		private float id;
		
		public Pool() 
		{
			_pool = new Stack<T>();
		}
		
		internal void FillInternal(int amount)
		{
			while (_pool.Count < amount)
			{
				_pool.Push(GetInstance());
			}
		}

		internal T CreateInternal()
		{
			var instance = _pool.Count > 0 ? _pool.Pop() : GetInstance();
			instance.isDisposed = false;
			return instance;
		}

		internal void DisposeInternal(PoolItem item)
		{
			_pool.Push((T)item);
		}

		private T GetInstance()
		{
			var instance = (T) Activator.CreateInstance(typeof(T));
			instance.Instantiate(DisposeInternal, id++);
			return instance;
		}

		// *** STATIC *** //
		
		private static Pool<T> _instance;

		public static T Create()
		{
			return Instance.CreateInternal();
		}

		public static void Fill(int amount)
		{
			Instance.FillInternal(amount);
		}

		private static Pool<T> Instance
		{
			get 
			{
				if (_instance == null)
				{
					_instance = new Pool<T>();
				}
				return _instance;
			}
		}
	}
}