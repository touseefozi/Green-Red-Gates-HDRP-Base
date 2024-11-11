using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Smart.Essence
{
	public abstract class CommandBase : IDisposable
	{
		protected ContextBase _context;
		
		public void InitContext(ContextBase context)
		{
			_context = context;
		}
		
		public void Dispose()
		{
			OnDispose();
			_context = null;
		}
		
		protected virtual void OnDispose() {}
		
		protected Coroutine StartCoroutine(IEnumerator routine)
		{
			return _context.StartCoroutine(routine);
		}

		protected void StopCoroutine(IEnumerator routine)
		{
			_context.StopCoroutine(routine);
		}

		protected void DispatchEvent(SmartEvent @event)
		{
			_context.DispatchEvent(@event);
		}

		protected void DispatchEvent<T>(SmartEvent<T> @event, T value)
		{
			_context.DispatchEvent(@event, value);
		}
	}
		
	public abstract class Command<TResult> : CommandBase
	{
		private readonly TaskCompletionSource<TResult> _source;
		
		protected Command()
		{
			_source = new TaskCompletionSource<TResult>();
		}

		protected abstract void OnCommandRun();

		protected void Resolve(TResult result)
		{
			_source.SetResult(result);
			Dispose();
		}

		public Task<TResult> Run()
		{
			OnCommandRun();
			return _source.Task;
		}
	}
		
	public abstract class Command<TArgument, TResult> : CommandBase
	{
		private readonly TaskCompletionSource<TResult> _source;
		
		protected Command()
		{
			_source = new TaskCompletionSource<TResult>();
		}

		protected abstract void OnCommandRun(TArgument cell);

		protected void Resolve(TResult result)
		{
			_source.SetResult(result);
			Dispose();
		}

		public Task<TResult> Run(TArgument argument)
		{
			OnCommandRun(argument);
			return _source.Task;
		}
	}
}