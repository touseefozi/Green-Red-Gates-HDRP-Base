using System;
using System.Collections.Generic;

namespace Smart.Pooling
{
	public interface IPool
	{
		float lastID { get; set; }
		void Add(PoolItem obj);
	}
	
}