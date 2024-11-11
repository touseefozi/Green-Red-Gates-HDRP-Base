using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace Smart.Dependency
{
	internal class BindInfoUntyped : BindInfoBase
	{
		private readonly object _instance;

		public BindInfoUntyped(DIContainer container, object instance) : base(container)
		{
			Assert.IsTrue(instance != null);
			_instance = instance;
			ServiceType = ImplementationType = _instance.GetType();
			
			IsSingleton = true;
			IsLazy = false;
		}

		public override object GetInstance()
		{
			return _instance;
		}

		public override object GetInstance(object[] parameters)
		{
			throw new Exception("Can't create untyped instance with parameters.");
		}

		public override void SetSingleton(bool value)
		{
			if (value)
			{
				Debug.LogWarning($"You can't use multiply instances for service type {ServiceType}.");
			}
		}

		public override void SetLazy(bool value)
		{
			if (!value)
			{
				Debug.LogWarning($"Service with type {ServiceType} is always initialized.");
			}
		}
	}
}