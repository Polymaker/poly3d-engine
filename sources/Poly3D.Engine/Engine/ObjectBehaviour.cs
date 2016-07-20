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
                _Enabled = value;
            }
        }

        protected virtual void OnUpdate(float deltaTime)
        {

        }
    }
}
