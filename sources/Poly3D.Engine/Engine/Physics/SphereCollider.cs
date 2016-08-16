using OpenTK;
using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Physics
{
    public class SphereCollider : Collider
    {
        private BoundingSphere _SphereBounds;
        private BoundingBox _BoxBounds;

        public override BoundingBox Bounds
        {
            get { return _BoxBounds; }
        }

        public Vector3 Center
        {
            get { return _SphereBounds.Center; }
            set
            {
                _SphereBounds.Center = value;
                _BoxBounds = new BoundingBox(_SphereBounds.Center, Vector3.One * _SphereBounds.Radius);
            }
        }

        public float Radius
        {
            get { return _SphereBounds.Radius; }
            set
            {
                _SphereBounds.Radius = value;
                _BoxBounds = new BoundingBox(_SphereBounds.Center, Vector3.One * _SphereBounds.Radius);
            }
        }

        public SphereCollider()
        {
            _SphereBounds = new BoundingSphere();
            _BoxBounds = new BoundingBox();
        }

        public SphereCollider(BoundingSphere bounds)
        {
            _SphereBounds = bounds;
            _BoxBounds = new BoundingBox(_SphereBounds.Center, Vector3.One * _SphereBounds.Radius);
        }

        public override bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            var localRay = Ray.Transform(ray, EngineObject.Transform.WorldToLocalMatrix);
            float hitDistance;
            if (_SphereBounds.Intersects(ray, out hitDistance))
            {
                var localPt = localRay.GetPoint(hitDistance);
                var worldPt = EngineObject.Transform.ToWorldSpace(localPt);
                var realDistance = (worldPt - ray.Origin).Length;
                if (maxDistance == 0 || realDistance <= maxDistance)
                {
                    hitInfo = new RaycastHit(ray, EngineObject, realDistance, -1);
                    return true;
                }
            }
            hitInfo = null;
            return false;
        }
    }
}
