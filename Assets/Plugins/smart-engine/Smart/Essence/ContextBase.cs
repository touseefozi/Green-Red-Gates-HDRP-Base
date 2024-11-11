using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Smart.Basics.Extensions;
using Smart.Dependency;
using Smart.Signals;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

namespace Smart.Essence
{
	public abstract class ContextBase : MonoBehaviour
	{
		[SerializeField] private ScriptableObject[] _settings;
		[SerializeField] private MonoBehaviour[] _monoBehaviours;
		
		private readonly int _hash = Guid.NewGuid().GetHashCode();
		private readonly List<ContextBase> _children = new List<ContextBase>();
		private readonly Dictionary<Scene, List<ContextBase>> _childrenByScene = new Dictionary<Scene, List<ContextBase>>();
        private readonly List<MediatorBindInfo> _mediators = new List<MediatorBindInfo>();
		
		protected Controller RootController { get; private set; }
		protected bool _isAutoInitialize = true;
		
        public ContextBase ParentContext { get; private set; }
		public MvcContainer Container { get; private set; }
        public bool IsEnabled { get; set; } = true;
        
        private bool _isPaused;
        private bool _hasParent;
        private bool _isInitialized;
        private bool _isDestroyed;
        private float _maxDeltaTime = 0.05f;

        public readonly Signal Initialized = new Signal();
        
        public float MaxDeltaTime
        {
            set => _maxDeltaTime = Mathf.Clamp01(value);
            get => _maxDeltaTime;
        }
		
		protected abstract void InstallBindings();
		
		protected virtual void OnAwake() {}
		protected virtual void OnStart() {}
		protected virtual void OnInitialized() {}
        
        protected virtual void OnDispose() {}
		
		private void Awake()
		{
			Container = new MvcContainer();
			RootController = new InternalRootController();
			RootController.SetContext(this);
            OnAwake();
		}
        
        private void OnApplicationQuit()
        {
            RootController.ApplicationQuit();
        }

        protected virtual void Start()
        {
            if (_isAutoInitialize)
            {
                Initialize();
            }
            
            OnStart();
        }

        private void OnDestroy()
		{
            if (!_isDestroyed)
            {
                RootController.Dispose();
                RootController = null;
                Container.Dispose();
                OnDispose();
                _isDestroyed = true;
            }
		}
        
		private void InstallSettings()
		{
			foreach (var scriptableObject in _settings)
			{
				Assert.IsNotNull(scriptableObject);
				Container.BindUntypedInstance(scriptableObject);
			}
		}

		private void InstallMonoBehaviours()
		{
			foreach (var monoBehaviour in _monoBehaviours)
			{
				Assert.IsNotNull(monoBehaviour);
				Container.BindUntypedInstance(monoBehaviour);
			}
		}

		public void Initialize()
		{
            if (!_isInitialized)
            {
                InstallSettings();
                InstallMonoBehaviours();
                InstallBindings();
            
                Container.Initialize();
                CreateMediators();
                OnInitialized();
                RootController.Initialize();
                Initialized.Invoke();
            
                _isInitialized = true;
            }
            else
            {
                throw new Exception("Context already initialized!");
            }
		}

        private void CreateMediators()
        {
            foreach (var bindInfo in _mediators)
            {
                var mediator = Container.CreateMediator(bindInfo.ControllerType, bindInfo.ViewType, bindInfo.View);
                RootController.AddController(mediator);
            }
        }

        protected void CreateLazySingletonControllers()
        {
        	var bindings = Container.GetBindingsOfBase<Controller>();

            foreach (var bindInfo in bindings)
            {
                if (bindInfo.IsSingleton && bindInfo.IsLazy)
                {
                    RootController.CreateController(bindInfo.ServiceType);
                }
            }
        }
        
        private void OnApplicationFocus(bool hasFocus)
        {
            if (!Application.isEditor)
            {
                _isPaused = !hasFocus;

                if (!_isPaused)
                {
	                RootController.ApplicationResume();
                }
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            _isPaused = pauseStatus;

            if (!_isPaused)
            {
	            RootController.ApplicationResume();
            }
        }

		private void Update()
		{
            if (IsEnabled && !_isPaused && !_isDestroyed)
            {
                var delta = Mathf.Min(Time.deltaTime, _maxDeltaTime);
                UpdateController(RootController, delta);
            }
		}

		private void UpdateController(Controller controller, float delta)
		{
			if (!controller.IsActive)
			{
				return;
			}
			
			for (int i = 0; i < controller.UpdatableChildren.Count; i++)
			{
				var updatable = controller.UpdatableChildren[i];

				if (updatable.IsActive)
				{
					updatable.Update(delta);
				}
			}
			
			for (int i = 0; i < controller.Children.Count; i++)
			{
				var child = controller.Children[i];

				if (child.Children.Count > 0)
				{
					UpdateController(child, delta);
				}
			}
		}

		private void LateUpdate()
        {
            if (IsEnabled && !_isPaused && !_isDestroyed)
            {
                var delta = Mathf.Min(Time.deltaTime, _maxDeltaTime);
                LateUpdateController(RootController, delta);
            }
        }
		
		private void LateUpdateController(Controller controller, float delta)
		{
			if (!controller.IsActive)
			{
				return;
			}
			
			for (int i = 0; i < controller.LateUpdatableChildren.Count; i++)
			{
				var updatable = controller.LateUpdatableChildren[i];

				if (updatable.IsActive)
				{
					updatable.LateUpdate(delta);
				}
			}
			
			for (int i = 0; i < controller.Children.Count; i++)
			{
				var child = controller.Children[i];

				if (child.Children.Count > 0)
				{
					LateUpdateController(child, delta);
				}
			}
		}

        private void FixedUpdate()
        {
            if (IsEnabled && !_isPaused && !_isDestroyed)
            {
	            FixedUpdateController(RootController);
            }
        }
		
        private void FixedUpdateController(Controller controller)
        {
	        if (!controller.IsActive)
	        {
		        return;
	        }
	        
	        for (int i = 0; i < controller.FixedUpdatableChildren.Count; i++)
	        {
		        var updatable = controller.FixedUpdatableChildren[i];
		        
		        if (updatable.IsActive)
		        {
			        updatable.FixedUpdate();
		        }
	        }
	        
	        for (int i = 0; i < controller.Children.Count; i++)
	        {
		        var child = controller.Children[i];

		        if (child.Children.Count > 0)
		        {
			        FixedUpdateController(child);
		        }
	        }
        }

        protected void BindCommand<T>() where T : CommandBase
		{
			Container.Bind<T>().AsMultiply();
		}

        protected IInstanceBinder BindInstance<T>(T instance)
        {
            return Container.BindInstance(instance);
        }

        protected IInstanceBinder BindInstance<T, TImpl>(TImpl instance) where TImpl : T
        {
            return Container.BindInstance<T, TImpl>(instance);
        }

        protected IInstanceBinder Bind<T>()
        {
            return Container.Bind<T>();
        }

        protected IInstanceBinder Bind<T, TImpl>() where TImpl : T
        {
            return Container.Bind<T, TImpl>();
        }

		protected IBinder BindController<T>() where T : Controller
		{
			return Container.Bind<T>().ForUniqueHolder();
		}
		
		protected IBinder BindController<T, TImpl>() where TImpl : Controller, T
		{
			return Container.Bind<T, TImpl>().ForUniqueHolder();
		}
        
        public void BindMediator<TController, TView>() 
            where TController : Controller
            where TView : MonoBehaviour
        {
            var views = gameObject.GetComponentsInChildren<TView>(true);
            
            if (views.IsNotNullOrEmpty())
            {
                var controllerType = typeof(TController);
                var viewType = typeof(TView);

                for (int i = 0; i < views.Length; i++)
                {
                    var view = views[i];
                    var bindInfo = new MediatorBindInfo(controllerType, viewType, view);
                    _mediators.Add(bindInfo);
                }
            }
            else
            {
                throw new Exception($"Can't find any components of type {typeof(TView)} in hierarchy.");
            }
        }

		protected IInstanceBinder BindFromHierarchy<T>() where T : Component
		{
			var instance = gameObject.GetComponentInChildren<T>(true);

			if (instance == null)
			{
				throw new Exception($"Can't find component {typeof(T)} in hierarchy.");
			}
			return Container.BindInstance(instance);
		}

		protected IInstanceBinder BindFromHierarchy<T, TImpl>() where TImpl : T
		{
			var instance = gameObject.GetComponentInChildren<TImpl>(true);

			if (instance == null)
			{
				throw new Exception($"Can't find component {typeof(TImpl)} in hierarchy.");
			}
			return Container.BindInstance<T, TImpl>(instance);
		}

		protected IInstanceBinder BindListFromHierarchy<T>() where T : Component
		{
			var instances = gameObject.GetComponentsInChildren<T>(true);

			return Container.BindInstance(instances.ToList());
		}

		protected IInstanceBinder BindEmptyList<T>() where T : Component
		{
			return Container.BindInstance(new List<T>());
		}
		
		protected IBinder BindFactory<T>(T prefab, string name = null, Transform placeholder = null) where T : MonoBehaviour
		{
			var factory = new Factory<T>(prefab, placeholder, name);
			Container.BindInstance(factory);
			return Container.Bind(factory.Create).AsMultiply();
		}

        public IEnumerator LoadSceneAsync(string sceneName)
		{
			var asyncOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

			while (!asyncOperation.isDone)
			{
				yield return null;
			}

			var scene = SceneManager.GetSceneByName(sceneName);
			Assert.IsTrue(scene.IsValid());
			
			var gameObjects = scene.GetRootGameObjects();
            var contexts = GetSceneContexts(scene);
            
			foreach (var gameObject in gameObjects)
			{
				var context = gameObject.GetComponentInChildren<ContextBase>();
                
                if (context != null)
                {
                    contexts.Add(context);
                    AddChildContext(context);
                }
			}
            
            Assert.IsTrue(contexts.IsNotNullOrEmpty(), $"There is no contexts in scene {sceneName}.");
		}
        
        public void AddChildContext(ContextBase context)
        {
            context.SetParent(this);
            Assert.IsFalse(_children.Contains(context));
            _children.Add(context);
        }

        public IEnumerator UnloadSceneAsync(string sceneName)
        {
			var scene = SceneManager.GetSceneByName(sceneName);
			
			Assert.IsTrue(scene.IsValid(), $"Trying to unload invalid scene {sceneName}.");
			Assert.IsTrue(scene.isLoaded, $"Trying to unload not loaded scene {sceneName}.");
			Assert.IsTrue(_childrenByScene.ContainsKey(scene), $"There is no registered contexts for scene {sceneName}.");
			
			var contexts = GetSceneContexts(scene);

			foreach (var context in contexts)
			{
				Assert.IsTrue(_children.Contains(context));
                context.SetParent(null);
				_children.Remove(context);
			}
			
			contexts.Clear();
			_childrenByScene.Remove(scene);
			
			var gameObjects = scene.GetRootGameObjects();

			foreach (var gameObject in gameObjects)
			{
				gameObject.SetActive(false);
			}

			var asyncOperation = SceneManager.UnloadSceneAsync(scene);

			while (!asyncOperation.isDone)
			{
				yield return null;
			}
			
			//Resources.UnloadUnusedAssets();
		}

        private void SetParent(ContextBase context)
        {
            ParentContext = context;
            _hasParent = context != null;
        }

        private List<ContextBase> GetSceneContexts(Scene scene)
        {
            if (!_childrenByScene.ContainsKey(scene))
            {
                _childrenByScene.Add(scene, new List<ContextBase>());
            }
			
            return _childrenByScene[scene];
        }

		public void DispatchEvent(SmartEvent @event)
		{
            if (@event.IsCrossContext && _hasParent)
            {
                ParentContext.DispatchEvent(@event);
            }
            else
            {
                ProcessEvent(@event);
            }
		}
        
        internal void ProcessEvent(SmartEvent @event)
        {
            RootController.ProcessEvent(@event);
			
            if (@event.IsCrossContext)
            {
                for (int i = 0; i < _children.Count; i++)
                {
                    var context = _children[i];
                    context.ProcessEvent(@event);
                }
            }
        }
		
		public void DispatchEvent<T>(SmartEvent<T> @event, T value)
		{
            if (@event.IsCrossContext && _hasParent)
            {
                ParentContext.DispatchEvent(@event, value);
            }
            else
            {
                ProcessEvent(@event, value);
            }
		}
		
		public void ProcessEvent<T>(SmartEvent<T> @event, T value)
		{
			RootController.ProcessEvent(@event, value);
			
			if (@event.IsCrossContext)
			{
				for (int i = 0; i < _children.Count; i++)
				{
					var context = _children[i];
					context.ProcessEvent(@event, value);
				}
			}
		}

		public override int GetHashCode()
		{
			return _hash;
		}
	}
}