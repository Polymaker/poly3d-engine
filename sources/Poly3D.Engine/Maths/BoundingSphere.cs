using OpenTK;

namespace Poly3D.Maths
{
    public struct BoundingSphere
    {

        public Vector3 Center { get; set; }

        public float Radius { get; set; }

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
            Center = center;
            Radius = radius;
        }
    }
}
