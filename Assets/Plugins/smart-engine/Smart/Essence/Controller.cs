using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Smart.DataTypes;
using Smart.Signals;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Smart.Essence
{
	internal class InternalRootController : Controller {}
    
	public class Controller : IDisposable
	{
        private static int _lastUsedID;
        
        internal readonly List<IUpdatable> UpdatableChildren;
        internal readonly List<ILateUpdatable> LateUpdatableChildren;
        internal readonly List<IFixedUpdatable> FixedUpdatableChildren;
        
		private readonly int _id;
		private readonly string _className;
		private readonly EventBus _eventBus;
		private readonly Dictionary<int, SmartCoroutine> _coroutines;
		
		private bool _isDisposed;
		private ContextBase _context;
		
        internal List<Controller> Children { get; private set; }
		internal bool IsInitialized { get; private set; }
		internal ControllerEventPriority EventsPriority { get; private set; }
		public bool IsActive { get; private set; } = true;
		public Controller Parent { get; private set; }

        public readonly Signal Initialized = new Signal();

		public Controller()
		{
            _id = ++_lastUsedID;
            _className = GetType().Name;
            EventsPriority = ControllerEventPriority.Regular;
            
			Children = new List<Controller>();
			UpdatableChildren = new List<IUpdatable>();
			LateUpdatableChildren = new List<ILateUpdatable>();
			FixedUpdatableChildren = new List<IFixedUpdatable>();
			
			_eventBus = new EventBus();
            _coroutines = new Dictionary<int, SmartCoroutine>();
		}
		
		protected virtual void OnInitialized() {}
		protected virtual void OnApplicationQuit() {}
		protected virtual void OnApplicationResume() {}
		protected virtual void OnEnabled() {}
		protected virtual void OnDisabled() {}
		protected virtual void OnDispose() {}

		internal void Initialize()
		{
			if (!IsInitialized)
			{
				OnInitialized();
                
                if (IsActive)
                {
                    OnEnabled();
                }
                
                Initialized.Invoke();
				IsInitialized = true;

				foreach (var controller in Children)
				{
					controller.Initialize();
				}
			}
		}
		
		public void Dispose()
		{
            if (!_isDisposed)
            {
                OnDispose();
                StopAllCoroutines();
                RemoveFromParentController();
                RemoveAllEvents();
			
                foreach (var controller in Children)
                {
                    controller.Parent = null;
                    controller.Dispose();
                }
			
                Children.Clear();

                if (!(this is InternalRootController))
                {
                    _context.Container.ReleaseInstance(this);
                }
                
                _isDisposed = true;
            }
            else
            {
	            LogWarning("Controller is already disposed.");
            }
		}

        internal void ApplicationQuit()
        {
            OnApplicationQuit();
            
            foreach (var controller in Children)
            {
                controller.ApplicationQuit();
            }
        }

        internal void ApplicationResume()
        {
            OnApplicationResume();
            
            foreach (var controller in Children)
            {
                controller.ApplicationResume();
            }
        }

		public void SetContext(ContextBase context)
		{
			if (_context == null)
			{
				_context = context;
			}
			else
			{
				throw new SmartException(this, "Context is already assigned!");
			}
		}
		
		public void SetActive(bool value)
		{
            if (value && !IsActive)
            {
                OnEnabled();
            }
            else if (!value && IsActive)
            {
                OnDisabled();
            }
            
			IsActive = value;
		}
		
		public void SetEventsPriority(ControllerEventPriority priority)
		{
			EventsPriority = priority;
			
			if (Parent != null)
			{
				Parent.SortControllers();
			}
		}

		public void AddController(Controller controller, int index = -1)
		{
			if (controller._context == null)
			{
				controller.SetContext(_context);
			}
			else if (!controller._context.Equals(_context))
			{
				throw new Exception("Can't add controller with different context.");
			}
				
			controller.RemoveFromParentController();
			controller.Parent = this;
			
			if (IsInitialized && !controller.IsInitialized)
			{
				controller.Initialize();
			}
            
            if (index >= 0)
            {
                Children.Insert(index, controller);
            }
            else
            {
			    Children.Add(controller);
            }
            
            SortControllers();

            if (controller is IUpdatable updatable)
            {
	            UpdatableChildren.Add(updatable);
            }

            if (controller is ILateUpdatable lateUpdatable)
            {
	            LateUpdatableChildren.Add(lateUpdatable);
            }

            if (controller is IFixedUpdatable fixedUpdatable)
            {
	            FixedUpdatableChildren.Add(fixedUpdatable);
            }
		}

		public void RemoveAllControllers(bool dispose = false)
		{
			for (int i = 0; i < Children.Count; i++)
			{
				var controller = Children[i];
				controller.Parent = null;
				
				if (dispose)
				{
					controller.Dispose();
				}
			}
			
			Children.Clear();
			UpdatableChildren.Clear();
			LateUpdatableChildren.Clear();
			FixedUpdatableChildren.Clear();
		}

		public void RemoveController(Controller controller, bool dispose = false)
		{
			Assert.IsTrue(Children.Contains(controller), $"Controller {controller.GetType().Name} is not child of {GetType().Name}.");
			Children.Remove(controller);
			SortControllers();
			controller.Parent = null;
			
			if (controller is IUpdatable updatable)
			{
				UpdatableChildren.Remove(updatable);
			}

			if (controller is ILateUpdatable lateUpdatable)
			{
				LateUpdatableChildren.Remove(lateUpdatable);
			}

			if (controller is IFixedUpdatable fixedUpdatable)
			{
				FixedUpdatableChildren.Remove(fixedUpdatable);
			}

			if (dispose)
			{
				controller.Dispose();
			}
		}

		public void RemoveFromParentController(bool dispose = false)
		{
			if (Parent != null)
			{
				Parent.RemoveController(this, dispose);
			}
		}

		private void SortControllers()
		{
			Children = Children.OrderBy(controller => controller.EventsPriority).ToList();
		}

		public TCommand CreateCommand<TCommand>() where TCommand : CommandBase
		{
			var command = _context.Container.GetInstance<TCommand>();
			command.InitContext(_context);
			return command;
		}
        
        public Controller CreateController(Type type)
        {
            var typeName = type.Name;
            
            if (_context == null)
            {
                throw new Exception($"Can't create child controllers for {typeName} before initialization.");
            }
            
            var instance = _context.Container.GetInstance(type);

            if (instance is Controller controller)
            {
                AddController(controller);
                return controller;
            }
            else
            {
                throw new Exception($"Type {typeName} is not Controller.");
            }
        }
        
		public T CreateController<T>() where T : Controller
		{
            var controller = CreateControllerInstance<T>();
            AddController(controller);
            return controller;
		}
        
		public T CreateController<T>(params object[] parameters) where T : Controller
		{
            var controller = CreateControllerInstance<T>(parameters);
            AddController(controller);
            return controller;
		}
        
		public T CreateControllerAt<T>(int index) where T : Controller
		{
            var controller = CreateControllerInstance<T>();
            AddController(controller, index);
            return controller;
		}
        
		public T CreateControllerAt<T>(int index, params object[] parameters) where T : Controller
		{
            var controller = CreateControllerInstance<T>(parameters);
            AddController(controller, index);
            return controller;
		}
        
        public T CreateControllerInstance<T>() where T : Controller
        {
            HandleCreateControllerErrors<T>();
            return _context.Container.GetInstance<T>();
        }
        
        public T CreateControllerInstance<T>(object[] parameters) where T : Controller
        {
            HandleCreateControllerErrors<T>();
            return _context.Container.GetInstance<T>(parameters);
        }

        private void HandleCreateControllerErrors<T>() where T : Controller
        {
	        var typeName = typeof(T).Name;
			
	        if (_context == null)
	        {
		        throw new Exception($"Can't create child controllers for {typeName} before initialization.");
	        }
            
	        if (!_context.Container.HasBinding<T>())
	        {
		        throw new Exception($"There is no bindigns for {typeName}.");
	        }
        }

		protected SmartCoroutine StartCoroutine(IEnumerator routine)
		{
            var coroutine = new SmartCoroutine(routine);
            _coroutines.Add(coroutine.Id, coroutine);
            _context.StartCoroutine(ProcessCoroutine(coroutine));
            return coroutine;
		}

        private IEnumerator ProcessCoroutine(SmartCoroutine coroutine)
        {
            yield return coroutine.Routine;
            _coroutines.Remove(coroutine.Id);
        }

        protected void StopCoroutine(SmartCoroutine coroutine)
		{
			if (coroutine != null)
			{
				_context.StopCoroutine(coroutine.Routine);
				_coroutines.Remove(coroutine.Id);
			}
		}

		protected void StopAllCoroutines()
		{
            foreach (var coroutine in _coroutines.Values)
            {
                _context.StopCoroutine(coroutine.Routine);
            }
            
            _coroutines.Clear();
		}
		
		protected SmartCoroutine DelayedCall(float delay, Action callback)
		{
			return StartCoroutine(WaitForDelayAndCall(delay, callback));
		}
		
		private IEnumerator WaitForDelayAndCall(float delay, Action callback)
		{
			yield return new WaitForSeconds(delay);
			callback();
		}
		
		protected SmartCoroutine DelayedCallUnscaled(float delay, Action callback)
		{
			return StartCoroutine(WaitForDelayAndCall(delay, callback));
		}
		
		private IEnumerator WaitForUnscaledDelayAndCall(float delay, Action callback)
		{
			yield return new WaitForSecondsRealtime(delay);
			callback();
		}
		
		protected void NextFrameCall(Action callback)
		{
			_context.StartCoroutine(WaitForNextFrameAndCall(callback));
		}
		
		private IEnumerator WaitForNextFrameAndCall(Action callback)
		{
			yield return null;
			callback();
		}
		
		protected void DelayedCall<T>(float delay, Action<T> callback, T argument)
		{
			_context.StartCoroutine(WaitForDelayAndCall(delay, callback, argument));
		}
		
		private IEnumerator WaitForDelayAndCall<T>(float delay, Action<T> callback, T argument)
		{
			yield return new WaitForSecondsRealtime(delay);
			callback(argument);
		}
		
		protected void NextFrameCall<T>(Action<T> callback, T argument)
		{
			_context.StartCoroutine(WaitForNextFrameAndCall(callback, argument));
		}
		
		private IEnumerator WaitForNextFrameAndCall<T>(Action<T> callback, T argument)
		{
			yield return null;
			callback(argument);
		}
		
		public IEnumerator LoadSceneAsync(string sceneName)
		{
			return _context.LoadSceneAsync(sceneName);
		}
		
		public IEnumerator UnloadSceneAsync(string sceneName)
		{
			return _context.UnloadSceneAsync(sceneName);
		}

		protected void AddListener(SmartEvent @event, Action callback)
		{
			_eventBus.AddListener(@event, callback);
		}

		protected void AddListener<T>(SmartEvent<T> @event, Action<T> callback)
		{
			_eventBus.AddListener(@event, callback);
		}

		protected void AddListener<T>(SmartEvent<T> @event, Action callback)
		{
			_eventBus.AddListener(@event, callback);
		}

		protected void RemoveListener(SmartEvent @event, Action callback)
		{
			_eventBus.RemoveListener(@event, callback);
		}

		protected void RemoveListener<T>(SmartEvent<T> @event, Action<T> callback)
		{
			_eventBus.RemoveListener(@event, callback);
		}

		protected void DispatchEvent(SmartEvent @event)
		{
			_context.DispatchEvent(@event);
		}

		protected void DispatchEvent<T>(SmartEvent<T> @event, T value)
		{
			_context.DispatchEvent(@event, value);
		}

		protected void DispatchEventWithDelay(float delay, SmartEvent @event)
		{
			DelayedCall(delay, DispatchEvent, @event);
		}

		protected void DispatchEventWithDelay<T>(float delay, SmartEvent<T> @event, T value)
		{
			DelayedCall(delay, () => DispatchEvent(@event, value));
		}

		protected void RemoveAllEvents()
		{
			_eventBus.RemoveAllEvents();
		}

		internal void ProcessEvent(SmartEvent @event)
		{
			if (!IsActive) return;
			
			_eventBus.ReceiveEvent(@event);
			
			for (int i = 0; i < Children.Count; i++)
			{
				var controller = Children[i];
				controller.ProcessEvent(@event);
                
                if (controller._isDisposed)
                {
                    i--;
                }
			}
		}

		internal void ProcessEvent<T>(SmartEvent<T> @event, T value)
		{
			if (!IsActive) return;
			
			_eventBus.ReceiveEvent(@event, value);

            // TODO use linked list
			for (int i = 0; i < Children.Count; i++)
			{
				var controller = Children[i];
				controller.ProcessEvent(@event, value);
                
                if (controller._isDisposed)
                {
                    i--;
                }
			}
		}

        public override int GetHashCode()
        {
            return _id;
        }

        protected void LogInfo(string message)
        {
            Debug.Log($"[{_className}] {message}");
        }
        
        protected void LogWarning(string message)
        {
            Debug.LogWarning($"<color=yellow>[{_className}] {message}</color>");
        }
        
        protected void LogError(string message)
        {
            Debug.LogError($"<color=red>[{_className}] {message}</color>");
        }
    }
}