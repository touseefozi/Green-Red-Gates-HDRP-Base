using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Smart.Essence
{
	public class Factory<T> where T : MonoBehaviour
	{
		private readonly T _prefab;
		private readonly string _name;
		private readonly Stack<T> _pool;
        
		private Transform _poolContainerTransform;
		private GameObject _poolContainer;
        private bool _hasPoolContainer;
        
		public bool AutoActivate { get; set; } = true;
		public bool UsePoolContainer { get; set; } = true;
		public Transform Placeholder { get; set; }

		public Factory(T prefab)
		{
			_prefab = prefab;
			_name = prefab.name;
			_pool = new Stack<T>();
		}

		public Factory(T prefab, string name) : this(prefab)
		{
			_name = name;
		}

		public Factory(T prefab, Transform placeholder) : this(prefab)
		{
			_name = prefab.name;
			Placeholder = placeholder;
		}

		public Factory(T prefab, Transform placeholder, string name) : this(prefab, placeholder)
		{
			Assert.IsNotNull(prefab);
			_name = name ?? prefab.name;
			_pool = new Stack<T>();
		}

		public T Create()
		{
			return Create(Placeholder);
		}

		public T Create(Transform parent)
		{
			if (_pool.Count > 0)
			{
                var instance = _pool.Pop();
                instance.transform.SetParent(parent, false);

                if (AutoActivate)
                {
					instance.gameObject.SetActive(true);
                }
                
				return instance;
			}
			else
			{
				return Instantiate(parent);
			}
		}

		private T Instantiate(Transform parent)
		{
			var instance = parent != null ? Object.Instantiate(_prefab, parent) : Object.Instantiate(_prefab);
			
			if (!string.IsNullOrEmpty(_name))
			{
                instance.gameObject.name = _name;
			}
			
			return instance;
		}

		public void Dispose(List<T> list)
		{
            for (int i = 0; i < list.Count; i++)
            {
                Dispose(list[i]);
            }
            
            list.Clear();
        }

		public void Dispose(T[] array)
		{
            for (int i = 0; i < array.Length; i++)
            {
                Dispose(array[i]);
                array[i] = null;
            }
        }
        
		public void Dispose(T instance)
		{
			var gameObject = instance.gameObject;
			gameObject.SetActive(false);
			
			var transform = gameObject.transform;
			transform.localPosition = Vector3.zero;
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;

			if (UsePoolContainer && !_hasPoolContainer)
			{
				InitPoolContainer();
			}

			if (UsePoolContainer)
			{
				transform.SetParent(_poolContainerTransform, false);
			}
			
			_pool.Push(instance);
		}

		public void Fill(int amount)
		{
			Assert.IsTrue(amount <= 1000, "To big amount for filling factory pool.");

            if (UsePoolContainer && !_hasPoolContainer)
            {
                InitPoolContainer();
            }
            
			for (int i = 0; i < amount; i++)
			{
                var instance = Instantiate(UsePoolContainer ? _poolContainerTransform : Placeholder);
                instance.gameObject.SetActive(false);
				_pool.Push(instance);
			}
		}

        private void InitPoolContainer()
        {
	        _poolContainer = new GameObject($"{typeof(T).Name} Pool");
	        _poolContainer.SetActive(false);
	        _poolContainerTransform = _poolContainer.transform;
	        _hasPoolContainer = true;
        }
	}
}