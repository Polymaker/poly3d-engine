using OpenTK;
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
        }

        public bool Raycast(Ray ray, out float distance)
        {
            var denom = Vector3.Dot(Normal, ray.Direction);
            if (Math.Abs(denom) > 0)
            {
                var center = Normal * Distance;
                distance = Vector3.Dot(center - ray.Origin, Normal) / denom;
                return distance >= 0;
            }
            distance = 0;
            return false;
        }
    }
}
