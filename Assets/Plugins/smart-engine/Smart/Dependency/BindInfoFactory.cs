using System;
using UnityEngine.Assertions;

namespace Smart.Dependency
{
	internal class BindInfoFactory<T> : BindInfoBase
	{
		private readonly Func<T> _factoryA;
		private readonly Func<object[], T> _factoryB;
		private T _instance;

		public BindInfoFactory(DIContainer container, Func<T> factoryA, Func<object[], T> factoryB) : base(container)
		{
			Assert.IsTrue(factoryA != null, $"Binding factory for type {typeof(T)} can't be null");
			_factoryA = factoryA;
			_factoryB = factoryB;
			ServiceType = ImplementationType = typeof(T);
			IsLazy = true;
		}
		
		public override object GetInstance()
		{
			if (IsSingleton)
			{
				if (_instance != null)
				{
					return _instance;
				}
				else
				{
					_instance = _factoryA();
					return _instance;
				}
			}
			else
			{
				return _factoryA();
			}
		}
		
		public override object GetInstance(object[] parameters)
		{
			Assert.IsFalse(IsSingleton, $"Can't create instance with parameter, because {typeof(T)} is singleton");
			
			return _factoryB(parameters);
		}
	}
    
	internal class BindInfoFactory<T, TImpl> : BindInfoFactory<T>
	{
		public BindInfoFactory(DIContainer container, Func<T> factoryA, Func<object[], T> factoryB) : base(container, factoryA, factoryB)
		{
            ImplementationType = typeof(TImpl);
		}
	}
}