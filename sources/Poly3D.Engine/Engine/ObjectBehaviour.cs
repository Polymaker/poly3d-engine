using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class ObjectBehaviour : ObjectComponent
    {
        // Fields...
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

        protected virtual void OnUpdate(float deltaTime) { }

        protected virtual void OnRender(float deltaTime) { }

        protected virtual void OnEnable() { }

        protected virtual void OnDisable() { }

    }
}
