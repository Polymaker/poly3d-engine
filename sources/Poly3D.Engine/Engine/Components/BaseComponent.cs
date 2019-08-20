using Poly3D.Engine.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public abstract class BaseComponent<T> : IEngineComponent<T>, IEngineComponent, IInternalInitialize where T : EngineObject
    {
        private T _EngineObject;
        private bool isInitialized;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public T EngineObject
        {
            get { return _EngineObject; }
        }

        public Scene Scene
        {
            get { return EngineObject.Scene ?? null; }
        }

        public BaseComponent()
        {
            _EngineObject = null;
        }

        internal void Attach(T owner)
        {
            _EngineObject = owner;
            if(owner.isInitialized)
                Initialize();
        }

        EngineObject IEngineComponent.EngineObject
        {
            get { return EngineObject; }
        }

        void IInternalInitialize.Initialize()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (!isInitialized && EngineObject != null && EngineObject.isInitialized)
            {
                OnInitialize();
                isInitialized = true;
            }
        }

        protected virtual void OnInitialize() { }


        public void Destroy()
        {
            EngineObject.RemoveComponent(this);
        }

        public virtual IEngineComponent Clone()
        {
            return (IEngineComponent)Activator.CreateInstance(GetType());
        }
    }
}
