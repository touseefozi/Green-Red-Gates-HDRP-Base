using UnityEngine.Assertions;

namespace Smart.Dependency
{
	internal class BindInfoInstance<T> : BindInfoUntyped
	{
		public BindInfoInstance(DIContainer container, T instance) : base(container, instance)
		{
			Assert.IsTrue(instance != null, $"Bindable instance for type {typeof(T)} can't be null");
		}
	}
    
    internal class BindInfoInstance<T, TImpl> : BindInfoUntyped where TImpl : T
    {
        public BindInfoInstance(DIContainer container, TImpl instance) : base(container, instance) 
        {
            ServiceType = typeof(T);
            ImplementationType = typeof(TImpl);
            Assert.IsTrue(instance != null, $"Bindable instance for type {typeof(T)} can't be null");
        }
    }
}