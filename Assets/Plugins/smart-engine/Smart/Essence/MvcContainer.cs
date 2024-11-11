using System;
using System.Collections.Generic;
using System.Linq;
using Smart.Dependency;
using UnityEngine;

namespace Smart.Essence
{
	public class MvcContainer : DIContainer
	{
        private static readonly Dictionary<Type, IBindInfo> _crossContextBindings = new Dictionary<Type, IBindInfo>();

        internal Controller CreateMediator(Type controllerType, Type viewType, MonoBehaviour view)
        {
            var ctor = controllerType.GetConstructors().Single();
            var parameters = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = new List<object>();

            foreach (var parameterType in parameters)
            {
                var instance = parameterType == viewType ? view : GetInstance(parameterType);
                dependencies.Add(instance);
            }
            
            return (Controller)Activator.CreateInstance(controllerType, dependencies.ToArray());
        }
        
        public override void Dispose()
        {
            foreach (var type in _bindings.Keys)
            {
                var bindInfo = _bindings[type];
                
                if (bindInfo.IsCrossContext)
                {
                    _crossContextBindings.Remove(type);
                }
            }
            
            base.Dispose();
        }
        
        public override void Initialize()
        {
            base.Initialize();

            foreach (var type in _bindings.Keys)
            {
                var bindInfo = _bindings[type];
                
                if (bindInfo.IsCrossContext)
                {
                    if (_crossContextBindings.ContainsKey(type))
                    {
                        throw new Exception($"Type {type.Name} already added to cross-context.");
                    }
                    else
                    {
                        _crossContextBindings.Add(type, bindInfo);
                    }
                }
            }
        }
        
        protected override IBindInfo GetBindInfo(Type type)
        {
            if (!_crossContextBindings.ContainsKey(type))
            {
                if (!_bindings.ContainsKey(type))
                {
                    throw NoBindingsError(type);
                }
                
                return _bindings[type];
            }
			
            return _crossContextBindings[type];
        }
        
        public override bool HasBinding<T>()
        {
            var type = typeof(T);
            return _crossContextBindings.ContainsKey(type) || _bindings.ContainsKey(type);
        }
    }
}