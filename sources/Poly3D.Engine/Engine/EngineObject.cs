using Poly3D.Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Poly3D.Engine
{
    public abstract class EngineObject
    {
        private string _Name;
        private static long CurrentId = 0;

        private readonly long InstanceId = GenerateInstanceId();
        private Scene _Scene;
        private bool _Active;
        private List<IEngineComponent> _Components;

        public Scene Scene
        {
            get { return _Scene; }
            internal set
            {
                Initialize(value);
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                Scene.SetObjectName(this, ref _Name, value);
            }
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
            _Name = String.Empty;
            _Scene = null;
            Tag = null;
            _Active = false;
            _Components = new List<IEngineComponent>();
        }

        internal void Initialize(Scene scene)
        {
            _Scene = scene;
            _Active = true;
            _Name = Scene.GetDefaultName(this);
        }

        #region Components



        #endregion

        internal void AddComponent(IEngineComponent component)
        {
            _Components.Add(component);
            
        }

        //internal void AddComponent<T>(IEngineComponent<T> component) where T : EngineObject
        //{
        //    if (GetType() == typeof(SceneObject) && typeof(T) == typeof(UIObject))
        //        return;
        //    else if (GetType() == typeof(UIObject) && typeof(T) == typeof(SceneObject))
        //        return;
        //    _Components.Add(component);
        //}

        internal void RemoveComponent(IEngineComponent component)
        {
            _Components.Remove(component);
        }

        public T GetComponent<T>() where T : IEngineComponent
        {
            return _Components.OfType<T>().FirstOrDefault();
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
