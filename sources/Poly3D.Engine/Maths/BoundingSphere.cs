using OpenTK;

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
            distance = 0f;
            return false;
        }
    }
}
