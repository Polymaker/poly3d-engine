using Poly3D.Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public abstract class BaseComponent<T> : IEngineComponent<T>, IEngineComponent where T : EngineObject
    {
        private T _EngineObject;

        public T EngineObject
        {
            get { return _EngineObject; }
        }

        public Scene Scene
        {
            get { return EngineObject.Scene; }
        }

        public BaseComponent()
        {
            _EngineObject = null;
        }

        public BaseComponent(T engineObject)
        {
            _EngineObject = engineObject;
        }

        internal void SetOwner(T owner)
        {
            _EngineObject = owner;
            InitializeInternal();
        }

        EngineObject IEngineComponent.EngineObject
        {
            get { return EngineObject; }
        }

        protected virtual void Initialize() { }

        internal void InitializeInternal()
        {
            Initialize();
        }

        public virtual IEngineComponent Clone()
        {
            return (IEngineComponent)Activator.CreateInstance(GetType());
        }

        public void Destroy()
        {
            EngineObject.RemoveComponent(this);
        }
    }
}
