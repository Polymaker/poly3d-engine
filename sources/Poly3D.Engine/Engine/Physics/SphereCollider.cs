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
        private BoundingSphere _Bounds;

        public override BoundingBox Bounds
        {
            get
            {
                return new BoundingBox(_Bounds.Center, Vector3.One * _Bounds.Radius);
            }
        }

        public Vector3 Center
        {
            get { return _Bounds.Center; }
            set
            {
                _Bounds.Center = value;
            }
        }

        public float Radius
        {
            get { return _Bounds.Radius; }
            set
            {
                _Bounds.Radius = value;
            }
        }

        public SphereCollider()
        {
            _Bounds = new BoundingSphere();
        }

        public SphereCollider(BoundingSphere bounds)
        {
            _Bounds = bounds;
        }

        public override bool Raycast(Ray ray, out RaycastHit hitInfo, float maxDistance)
        {
            var localRay = Ray.Transform(ray, EngineObject.Transform.WorldToLocalMatrix);
            float hitDistance;
            if (_Bounds.Intersects(ray, out hitDistance))
            {
                var localPt = localRay.GetPoint(hitDistance);
                var worldPt = Vector3.Transform(localPt, EngineObject.Transform.LocalToWorldMatrix);
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
