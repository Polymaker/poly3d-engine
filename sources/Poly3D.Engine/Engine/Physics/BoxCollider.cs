using OpenTK;
using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Physics
{
    public class BoxCollider : Collider
    {
        private BoundingBox _Bounds;

        public override BoundingBox Bounds
        {
            get { return _Bounds; }
        }

        public Vector3 Center
        {
            get { return Bounds.Center; }
            set
            {
                _Bounds.Center = value;
            }
        }

        public Vector3 Size
        {
            get { return Bounds.Size; }
            set
            {
                _Bounds.Size = value;
            }
        }

        public BoxCollider()
        {
            _Bounds = BoundingBox.Zero;
        }

        public BoxCollider(BoundingBox bounds)
        {
            _Bounds = bounds;
        }

        public override bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            throw new NotImplementedException();
        }
    }
}
