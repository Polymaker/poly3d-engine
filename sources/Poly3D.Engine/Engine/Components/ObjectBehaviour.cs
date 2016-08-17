using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class ObjectBehaviour : ObjectComponent
    {
        // Fields...
        private bool _Initialized;
        private bool _Enabled;

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

        public ObjectBehaviour()
        {
            _Initialized = false;
            _Enabled = true;
        }

        public bool Initialized
        {
            get { return _Initialized; }
        }

        protected virtual void OnInitialize() { }

        protected virtual void OnUpdate(float deltaTime) { }

        protected virtual void OnRender(Camera camera) { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

        internal void DoUpdate(float deltaTime)
        {
            if (!Initialized)
                DoInitialize();
            OnUpdate(deltaTime);
        }

        internal void DoInitialize()
        {
            OnInitialize();
            _Initialized = true;
        }
    }
}
