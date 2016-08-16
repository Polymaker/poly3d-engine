using OpenTK;
using Poly3D.Engine.Physics;
using System.Collections.Generic;
using System.Linq;

namespace Poly3D.Maths
{
    public struct BoundingBox
    {
        // Fields...
        private Vector3 _Extents;
        private Vector3 _Center;

        /// <summary>
        /// Gets or sets the center of the bounding box.
        /// </summary>
        public Vector3 Center
        {
            get { return _Center; }
            set
            {
                _Center = value;
            }
        }

        /// <summary>
        /// Gets or sets the extents of the bounding box. This is always half of the size.
        /// </summary>
        public Vector3 Extents
        {
            get { return _Extents; }
            set
            {
                _Extents = value;
            }
        }

        /// <summary>
        /// Gets or sets the total size of the bounding box.
        /// </summary>
        public Vector3 Size
        {
            get { return Extents * 2f; }
            set
            {
                _Extents = value * 0.5f;
            }
        }

        #region Calculated bounds

        public Vector3 Min
        {
            get { return Center - Extents; }
            set
            {
                SetMinMax(value, Max);
            }
        }

        public Vector3 Max
        {
            get { return Center + Extents; }
            set
            {
                SetMinMax(Min, value);
            }
        }

        public float Left
        {
            get { return Min.X; }
            set
            {
                SetMinMax(new Vector3(value, Bottom, Back), Max);
            }
        }

        public float Right
        {
            get { return Max.X; }
            set
            {
                SetMinMax(Min, new Vector3(value, Top, Front));
            }
        }

        public float Top
        {
            get { return Max.Y; }
            set
            {
                SetMinMax(Min, new Vector3(Right, value, Front));
            }
        }

        public float Bottom
        {
            get { return Min.Y; }
            set
            {
                SetMinMax(new Vector3(Left, value, Back), Max);
            }
        }

        public float Front
        {
            get { return Max.Z; }
            set
            {
                SetMinMax(Min, new Vector3(Right, Top, value));
            }
        }

        public float Back
        {
            get { return Min.Z; }
            set
            {
                SetMinMax(new Vector3(Left, Bottom, value), Max);
            }
        }

        #endregion

        public BoundingBox(Vector3 center, Vector3 extents)
        {
            _Extents = extents;
            _Center = center;
        }

        public BoundingBox(float top, float bottom, float left, float right, float front, float back)
        {
            _Extents = Vector3.Zero;
            _Center = Vector3.Zero;
            SetMinMax(new Vector3(left, bottom, back), new Vector3(right, top, front));
        }

        public void SetMinMax(Vector3 min, Vector3 max)
        {
            _Extents = (max - min) * 0.5f;
            _Center = min + _Extents;
        }

        public bool Intersects(BoundingBox other)
        {
            return Min.X <= other.Max.X && Max.X >= other.Min.X && Min.Y <= other.Max.Y && Max.Y >= other.Min.Y && Min.Z <= other.Max.Z && Max.Z >= other.Min.Z;
        }

        public bool Intersects(Ray ray)
        {
            float dist;
            return Intersects(ray, out dist);
        }

        public bool Intersects(Ray ray, out float distance)
        {
            return PhysicsHelper.RayIntersectsBox(ray, this, out distance);
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

        public static BoundingBox FromPoints(IEnumerable<Vector3> points)
        {
            var minx = points.Min(p => p.X);
            var miny = points.Min(p => p.Y);
            var minz = points.Min(p => p.Z);
            var maxx = points.Max(p => p.X);
            var maxy = points.Max(p => p.Y);
            var maxz = points.Max(p => p.Z);
            var bb = new BoundingBox();
            bb.SetMinMax(new Vector3(minx, miny, minz), new Vector3(maxx, maxy, maxz));
            return bb;
        }

        public static readonly BoundingBox Zero = default(BoundingBox);
    }
}
