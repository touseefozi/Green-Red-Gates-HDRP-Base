using System;

namespace Smart.Dependency
{
	internal class DIBinder : IBinder
	{
		private readonly IBindInfo _bindInfo;
		
		public DIBinder(IBindInfo bindInfo)
		{
			_bindInfo = bindInfo;
		}

        public IInstanceBinder CrossContext()
        {
            if (_bindInfo.IsUnique)
            {
                throw new Exception($"Unique binding can't be cross-context.");
            }
            else
            {
                _bindInfo.CrossContext();
            }
            return this;
        }
		
		public IBinder AsMultiply()
		{
			_bindInfo.SetSingleton(false);
			return this;
		}

		public IBinder AsSingle()
		{
			_bindInfo.SetSingleton(true);
			return this;
		}

		public IBinder NonLazy()
		{
			_bindInfo.SetLazy(false);
			return this;
		}

		public IBinder Lazy()
		{
			_bindInfo.SetLazy(true);
			return this;
		}

		public IBinder ForUniqueHolder()
		{
            if (_bindInfo.IsCrossContext)
            {
                throw new Exception($"Cross-context binding can't be unique.");
            }
            else
            {
			    _bindInfo.SetUnique(true);
            }
			return this;
		}

		public IBinder ForAnyHolders()
		{
			_bindInfo.SetUnique(false);
			return this;
		}
	}

	public interface IInstanceBinder
	{
        IInstanceBinder CrossContext();
    }

	public interface IBinder : IInstanceBinder
	{
		IBinder AsSingle(); // default
		IBinder AsMultiply();
		IBinder Lazy(); // default
		IBinder NonLazy();
		IBinder ForAnyHolders(); // default
		IBinder ForUniqueHolder();
	}
}