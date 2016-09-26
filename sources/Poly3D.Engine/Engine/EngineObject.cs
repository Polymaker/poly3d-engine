using Poly3D.Engine.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace Poly3D.Engine
{
    public abstract class EngineObject : IInternalInitialize
    {
        private static long CurrentId = 0;

        private readonly long InstanceId = GenerateInstanceId();

        private Scene _Scene;
        private string _Name;
        private bool _Active;
        internal bool isInitialized;
        private List<IEngineComponent> _Components;

        public Scene Scene
        {
            get { return _Scene; }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                if (!isInitialized)
                    return;
                Scene.SetObjectName(this, ref _Name, value);
            }
        }

        internal bool IsSceneObject
        {
            get { return this is SceneObject; }
        }

        public object Tag { get; set; }

        /// <summary>
        /// Enables or disables the current scene object.
        /// </summary>
        public bool Active
        {
            get { return _Active; }
            set
            {
                if (_Active == value)
                    return;
                _Active = value;
            }
        }

        public abstract bool IsActive { get; }

        public IList<IEngineComponent> Components
        {
            get { return _Components.AsReadOnly(); }
        }

        #region Events

        public event EventHandler Activated;

        public event EventHandler Deactivated;

        #endregion

        public EngineObject()
        {
            _Name = string.Empty;
            _Scene = null;
            Tag = null;
            _Active = true;
            _Components = new List<IEngineComponent>();
        }

        internal void AssignScene(Scene scene)
        {
            _Scene = scene;
            _Name = Scene.GetDefaultName(this);
            if (scene.Initialized)
                Initialize();
        }

        void IInternalInitialize.Initialize()
        {
            Initialize();
        }

        private void Initialize()
        {
            //redundant check considering this method is internal and I should not call this method twice but it's there for 'safety'
            if (!isInitialized && Scene != null && Scene.Initialized)
            {
                isInitialized = true;
                OnInitialize();
                if (_Components.Count > 0)
                    _Components.ForEach(c => (c as IInternalInitialize).Initialize());
            }
        }

        protected virtual void OnInitialize() { }

        #region Components

        internal void AddComponent(IEngineComponent component)
        {
            if (component == null)
                return;
            _Components.Add(component);
        }

        protected virtual bool CanAddComponent(Type componentType)
        {
            var compBehavior = GetComponentTypeBehavior(componentType);
            var compObjType = GetComponentEngineObjectType(componentType);

            if (compObjType == typeof(SceneObject) && !IsSceneObject)
                return false;

            if (compBehavior != null)
            {
                if (compBehavior.IsSingleton && _Components.Any(c => c.GetType() == componentType))
                    return false;
            }
            return true;
        }

        public T AddComponent<T>() where T : IEngineComponent
        {
            if (!CanAddComponent(typeof(T)))
                return default(T);
            T component = Activator.CreateInstance<T>();

            var compObjType = GetComponentEngineObjectType(typeof(T));

            if (compObjType == typeof(SceneObject))
                (component as BaseComponent<SceneObject>).Attach((SceneObject)this);
            else
                (component as BaseComponent<UIObject>).Attach((UIObject)this);


            _Components.Add(component);
            return component;
        }

        public bool HasComponent<T>() where T : IEngineComponent
        {
            return _Components.Any(c => c.GetType() == typeof(T));
        }

        internal void RemoveComponent(IEngineComponent component)
        {
            _Components.Remove(component);
        }

        public T GetComponent<T>() where T : IEngineComponent
        {
            return _Components.OfType<T>().FirstOrDefault();
        }
        
        public IEnumerable<T> GetComponents<T>() where T : IEngineComponent
        {
            return _Components.OfType<T>();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static ComponentBehaviorAttribute GetComponentTypeBehavior(Type compType)
        {
            var compAttrs = (ComponentBehaviorAttribute[])compType.GetCustomAttributes(typeof(ComponentBehaviorAttribute), false);
            if (compAttrs.Length > 0)
                return compAttrs[0];
            return null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Type GetComponentEngineObjectType(Type compType)
        {
            var engineObjType = compType.GetProperty("EngineObject", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            return engineObjType.PropertyType;
        }

        #endregion

        public void Destroy()
        {
            if (Scene != null)
                Scene.RemoveObject(this);
        }

        public long GetInstanceId()
        {
            return InstanceId;
        }

        //defined as a function in case someday we want to handle id re-allocation
        internal static long GenerateInstanceId()
        {
            return Interlocked.Increment(ref CurrentId);
        }

        
    }
}
