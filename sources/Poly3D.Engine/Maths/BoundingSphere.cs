using OpenTK;
using Poly3D.Engine.Physics;

namespace Poly3D.Maths
{
    public struct BoundingSphere
    {

        private Vector3 _Center;
        private float _Radius;

        public Vector3 Center
        {
            get { return _Center; }
            set
            {
                _Center = value;
            }
        }
        
        public float Radius
        {
            get { return _Radius; }
            set
            {
                _Radius = value;
            }
        }

        public float Diameter
        {
            get { return Radius * 2f; }
            set
            {
                Radius = value * 0.5f;
            }
        }

        public BoundingSphere(Vector3 center, float radius)
        {
            _Center = center;
            _Radius = radius;
        }

        public bool Intersects(BoundingSphere other)
        {
            return (Center - other.Center).Length < Radius + other.Radius;
        }

        public bool Intersects(Ray ray, out float distance)
        {
            return PhysicsHelper.RayIntersectsSphere(ray, this, out distance);
        }

        public bool Intersects(Ray ray, out Vector3 intersectPoint)
        {
            float dist;
            if (Intersects(ray, out dist))
            {
                intersectPoint = ray.GetPoint(dist);
                return true;
            }
            intersectPoint = Vector3.Zero;
            return false;
        }
    }
}
