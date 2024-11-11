using System;

namespace Smart.Signals
{
	public interface ISignal : IDisposable
	{
		void Clear();
	}
}