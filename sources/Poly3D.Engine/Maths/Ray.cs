using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Maths
{
    public class Ray
    {
        private Vector3 _Direction;

        public Vector3 Origin { get; set; }

        public Vector3 Direction
        {
            get { return _Direction; }
            set
            {
                _Direction = value.Normalized();
            }
        }

        public Ray(Vector3 origin, Vector3 direction)
        {
            _Direction = direction.Normalized();
            Origin = origin;
        }

        public Vector3 GetPoint(float distance)
        {
            return Origin + Direction * distance;
        }

        public static Ray FromPoints(Vector3 near, Vector3 far)
        {
            return new Ray(near, (far - near).Normalized());
        }
    }
}
