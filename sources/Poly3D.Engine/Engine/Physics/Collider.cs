using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Physics
{
    public abstract class Collider : ObjectComponent
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

        public abstract BoundingBox Bounds { get; }

        public Collider()
        {
            _Enabled = false;
        }

        protected override void Initialize()
        {
            base.Initialize();
            _Enabled = true;
        }

        public abstract bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance);

    }
}
