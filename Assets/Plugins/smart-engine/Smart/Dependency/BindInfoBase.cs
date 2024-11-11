using System;
using UnityEngine;

namespace Smart.Dependency
{
	internal abstract class BindInfoBase : IBindInfo
	{
		protected readonly DIContainer _container;
		
		public Type ServiceType { get; protected set; }
		public Type ImplementationType { get; protected set; }
		public bool IsSingleton { get; protected set; }
		public bool IsLazy { get; protected set; }
		public bool IsCrossContext { get; protected set; }
        
        public bool IsUnique => _isUnique && IsSingleton;

		private bool _isUnique;
		
		protected BindInfoBase(DIContainer container)
		{
			_container = container;
			IsSingleton = true;
		}
		
		public abstract object GetInstance();
		public abstract object GetInstance(object[] parameters);
        
        public void CrossContext()
        {
            IsCrossContext = true;
        }
		
		public virtual void SetSingleton(bool value)
		{
			if (_container.IsInitialized)
			{
				LockWarning();
			}
			else
			{
				IsSingleton = value;
			}
		}

		public virtual void SetLazy(bool value)
		{
			if (_container.IsInitialized)
			{
				LockWarning();
			}
			else
			{
				IsLazy = value;
			}
		}

		public virtual void SetUnique(bool value)
		{
			if (_container.IsInitialized)
			{
				LockWarning();
			}
			else
			{
				_isUnique = value;
			}
		}

		private void LockWarning()
		{
			Debug.LogWarning("You can't change any settings in BindInfo when DIContainer is locked.");
		}
	}
}