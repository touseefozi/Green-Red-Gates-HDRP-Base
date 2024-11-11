using System;
using System.Collections.Generic;
using System.Linq;
using Smart.DataTypes;
using UnityEngine;

namespace Smart.Dependency
{
	public class DIContainer : IDisposable
	{
		protected readonly Dictionary<Type, IBindInfo> _bindings = new Dictionary<Type, IBindInfo>();
		protected readonly HashSet<IBindInfo> _lockedBindings = new HashSet<IBindInfo>();
		
		internal bool IsInitialized { get; private set; }

		public virtual void Initialize()
		{
            if (!IsInitialized)
            {
			    IsInitialized = true;
            }
		}

        public virtual void Dispose()
        {
            _bindings.Clear();
            _lockedBindings.Clear();
        }
        
        public List<IBindInfo> GetBindingsOfBase<T>()
        {
            var baseType = typeof(T);
            
            return _bindings.Values.Where(bindInfo =>
            {
	            var type = bindInfo.ImplementationType;

	            do
	            {
		            if (type == baseType)
		            {
			            return true;
		            }
		            
		            type = type.BaseType;
	            }
	            while (type != null);
	            
                return false;
            })
            .ToList();
        }
        
        public List<Type> GetTypesOfBase<T>()
        {
            var baseType = typeof(T);
            
            return _bindings.Values.Select(bindInfo => bindInfo.ServiceType).Where(type => type.BaseType == baseType).ToList();
        }

        public virtual bool HasBinding<T>()
        {
            return _bindings.ContainsKey(typeof(T));
        }
		
		public IBinder Bind<T>()
		{
			if (IsInitialized) throw LockedContainerError();
			
			var info = new BindInfoFactory<T>(this, CreateInstance<T>, CreateInstance<T>);
			RegisterBindInfo(info);
			return new DIBinder(info);
		}
		
		public IBinder Bind<T, TImpl>() where TImpl : T
		{
			if (IsInitialized) throw LockedContainerError();

			var info = new BindInfoFactory<T, TImpl>(this,
				() => CreateInstance<TImpl>(),
				parameters => CreateInstance<T>(parameters));
			
			RegisterBindInfo(info);
			return new DIBinder(info);
		}

		public IBinder Bind<T>(Func<T> factoryA, Func<object, T> factoryB = null)
		{
			if (IsInitialized) throw LockedContainerError();

			var info = new BindInfoFactory<T>(this, factoryA, factoryB);
			RegisterBindInfo(info);
			return new DIBinder(info);
		}

		public IInstanceBinder BindInstance<T>(T instance)
		{
			if (IsInitialized) throw LockedContainerError();

			var info = new BindInfoInstance<T>(this, instance);
			RegisterBindInfo(info);
			return new DIBinder(info);
		}

		public IInstanceBinder BindInstance<T, TImpl>(TImpl instance) where TImpl : T
		{
			if (IsInitialized) throw LockedContainerError();

			var info = new BindInfoInstance<T, TImpl>(this, instance);
			RegisterBindInfo(info);
			return new DIBinder(info);
		}

		public void BindUntypedInstance(object instance)
		{
			if (IsInitialized) throw LockedContainerError();

			var info = new BindInfoUntyped(this, instance);
			RegisterBindInfo(info);
		}

		public T GetInstance<T>()
		{
			return (T)GetInstance(typeof(T));
		}

		public T GetInstance<T>(object[] parameters)
		{
			return (T)GetInstance(typeof(T), parameters);
		}

		public object GetInstance(Type type)
		{
			var info = GetBindInfo(type);
			CheckUnique(info);
			return info.GetInstance();
		}

		public object GetInstance(Type type, object[] parameters)
		{
			var info = GetBindInfo(type);
			CheckUnique(info);
			return info.GetInstance(parameters);
		}

		private void CheckUnique(IBindInfo info)
		{
			if (info.IsUnique)
			{
				if (_lockedBindings.Contains(info))
				{
					throw Error($"Service type {info.ServiceType} is locked.");
				}
				else
				{
					_lockedBindings.Add(info);
				}
			}
		}

		public void ReleaseInstance(object instance)
		{
			var type = instance.GetType();
            
            if (HasBindingInfo(type))
            {
                var info = GetBindInfo(type);

                if (info.IsUnique && _lockedBindings.Contains(info))
                {
                    _lockedBindings.Remove(info);
                }
            }
		}
		
		internal T CreateInstance<T>()
		{
			var instanceType = typeof(T);
			var constructorInfo = instanceType.GetConstructors().Single();
			var parameters = constructorInfo.GetParameters();
			var dependencies = new object[parameters.Length];

			for (int i = 0; i < parameters.Length; i++)
			{
				var type = parameters[i].ParameterType;
				dependencies[i] = GetInstance(type);
			}
			
			return (T)Activator.CreateInstance(instanceType, dependencies);
		}
		
		internal T CreateInstance<T>(object[] definedParameters)
		{
			var instanceType = typeof(T);
			var constructorInfo = instanceType.GetConstructors().Single();
			var parameters = constructorInfo.GetParameters();
			var dependencies = new object[parameters.Length];

			if (definedParameters.Length > parameters.Length)
			{
				throw Error($"Can't create {typeof(T)} controller with {definedParameters.Length} arguments.");
			}

			for (int i = 0; i < parameters.Length; i++)
			{
				var type = parameters[i].ParameterType;

				if (i < definedParameters.Length)
				{
					var parameter = definedParameters[i];
					var definedType = parameter.GetType();

					if (type.IsInterface && definedType.GetInterfaces().Contains(type))
					{
						dependencies[i] = parameter;
					}
					else if (type == definedType || type == definedType.BaseType)
					{
						dependencies[i] = parameter;
					}
					else
					{
						throw Error($"Can't create {typeof(T)} controller with argument of type {definedType}.");
					}
				}
				else
				{
					dependencies[i] = GetInstance(type);
				}
			}
			
			return (T)Activator.CreateInstance(instanceType, dependencies);
		}
		
		protected void RegisterBindInfo(IBindInfo info)
		{
			var type = info.ServiceType;
			
			if (_bindings.ContainsKey(type))
			{
				throw Error($"Service with type {type} is already binded.");
			}
			
			_bindings.Add(type, info);
		}
		
		protected virtual IBindInfo GetBindInfo(Type type)
		{
			if (!_bindings.ContainsKey(type))
			{
				throw NoBindingsError(type);
			}
			
			return _bindings[type];
		}
        
        private bool HasBindingInfo(Type type)
        {
            return _bindings.ContainsKey(type);
        }
		
		protected Exception Error(string message)
		{
			return new SmartException(this, message);
		}
		
		protected Exception NoBindingsError(Type type)
		{
			return new SmartException(this, $"No bindings for {type}");
		}
		
		protected Exception LockedContainerError()
		{
			return new SmartException(this, "DIContainer is locked, you can't add new bindings.");
		}
	}
}