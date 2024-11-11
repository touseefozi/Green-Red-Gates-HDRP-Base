using System;

namespace Smart.Dependency
{
	public interface IBindInfo
	{
		object GetInstance();
		object GetInstance(object[] parameters);
		void SetSingleton(bool value);
		void SetLazy(bool value);
		void SetUnique(bool value);
		void CrossContext();
		
		Type ServiceType { get; }
		Type ImplementationType { get; }
		bool IsSingleton { get; }
		bool IsUnique { get; }
		bool IsLazy { get; }
		bool IsCrossContext { get; }
	}
}