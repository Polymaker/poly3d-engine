using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public abstract class BaseBehavior<T> : BaseComponent<T> where T : EngineObject
    {
        private bool _Enabled;
        private bool isStarted;

        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                if (_Enabled == value)
                    return;
                _Enabled = value;
                if (value)
                    OnEnable();
                else
                    OnDisable();
            }
        }

        public BaseBehavior()
        {
            _Enabled = true;
            isStarted = false;
        }

        protected virtual void OnStart() { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        protected virtual void OnUpdate(float deltaTime) { }

        protected virtual void OnRender(Camera camera) { }

        internal void DoUpdate(float deltaTime)
        {
            if (!IsInitialized || !Enabled)
                return;

            if (!isStarted)
            {
                OnStart();
                isStarted = true;
            }
            OnUpdate(deltaTime);
        }
    }
}
