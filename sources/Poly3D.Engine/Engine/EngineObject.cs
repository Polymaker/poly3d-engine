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
        private List<IComponent> _Components;

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
                _Active = value;
            }
        }

        public abstract bool IsActive { get; }

        public IList<IComponent> Components
        {
            get { return _Components.AsReadOnly(); }
        }

        public EngineObject()
        {
            _Name = String.Empty;
            _Scene = null;
            Tag = null;
            _Active = true;
            _Components = new List<IComponent>();
        }

        internal void Initialize(Scene scene)
        {
            _Scene = scene;
            Scene.SetObjectName(this, ref _Name, GetType().Name + GetInstanceId());
        }

        internal void AddComponent(IComponent component)
        {
            _Components.Add(component);
        }

        internal void RemoveComponent(IComponent component)
        {
            _Components.Remove(component);
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
