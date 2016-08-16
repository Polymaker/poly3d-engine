using OpenTK;
using Poly3D.Engine.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Maths
{
    public class Plane
    {
        private Vector3 _Normal;

        public float Distance { get; set; }

        public Vector3 Origin { get; set; }

        public Vector3 Normal
        {
            get { return _Normal; }
            set
            {
                _Normal = value.Normalized();
            }
        }

        public Plane(Vector3 normal, float distance)
        {
            _Normal = normal;
            Distance = distance;
            Origin = Vector3.Zero;
        }

        public bool Raycast(Ray ray, out float distance)
        {
            return PhysicsHelper.RayIntersectsPlane(ray, this, out distance);
        }

    }
}
