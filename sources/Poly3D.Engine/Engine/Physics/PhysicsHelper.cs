using OpenTK;
using Poly3D.Engine.Meshes;
using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Physics
{
    public static class PhysicsHelper
    {
        #region Ray intersection

        public static bool RayIntersectsBox(Ray ray, BoundingBox box, out float distance)
        {
            var rayInv = Vector3.Divide(Vector3.One, ray.Direction);
            var t1 = Vector3.Multiply(box.Min - ray.Origin, rayInv);
            var t2 = Vector3.Multiply(box.Max - ray.Origin, rayInv);

            var vMin = Vector3.ComponentMin(t1, t2);
            var vMax = Vector3.ComponentMax(t1, t2);
            var min = Math.Max(vMin.X, System.Math.Max(vMin.Y, vMin.Z));
            var max = Math.Min(vMax.X, System.Math.Min(vMax.Y, vMax.Z));

            if (max < 0)
            {
                distance = max;
                return false;
            }

            if (min > max)
            {
                distance = max;
                return false;
            }

            distance = min;
            return true;
        }

        public static bool RayIntersectsSphere(Ray ray, BoundingSphere sphere, out float distance)
        {
            distance = 0f;

            var a = Vector3.Dot(ray.Direction, ray.Origin - sphere.Center);
            var b = (ray.Origin - sphere.Center).Length;
            var c = (a * a) - (b * b) + (sphere.Radius * sphere.Radius);

            if (c < 0f)
                return false;

            if (NearZero(c))
            {
                distance = -a;
                return true;
            }

            var d = (float)Math.Sqrt(c);
            distance = Math.Min(-a + d, -a - d);//+|-
            return true;
        }

        public static bool RayIntersectsPlane(Ray ray, Plane plane, out float distance)
        {
            var denom = Vector3.Dot(plane.Normal, ray.Direction);

            if (!NearZero(denom))
            {
                var center = plane.Origin + (plane.Normal * plane.Distance);
                distance = Vector3.Dot(center - ray.Origin, plane.Normal) / denom;
                return distance >= 0;
            }
            //else the line and plane are parallel

            distance = 0;
            return false;
        }

        public static bool RayIntersectsTriangle(Ray ray, FaceTriangle triangle, out float distance)
        {
            return RayIntersectsTriangle(ray, triangle.Vertex0.Position, triangle.Vertex1.Position, triangle.Vertex2.Position, out distance);
        }

        public static bool RayIntersectsTriangle(Ray ray, Vector3 v1, Vector3 v2, Vector3 v3, out float distance)
        {
            distance = 0;

            var edge1 = Vector3.Subtract(v2, v1);
            var edge2 = Vector3.Subtract(v3, v1);
            var vP = Vector3.Cross(ray.Direction, edge2);
            //if determinant is near zero, ray lies in plane of triangle or ray is parallel to plane of triangle
            var determinant = Vector3.Dot(edge1, vP);

            if (determinant > -float.Epsilon && determinant < float.Epsilon)//NOT CULLING
                return false;
            var inverseDet = 1f / determinant;

            //calculate distance from V1 to ray origin
            var vT = Vector3.Subtract(ray.Origin, v1);

            var u = Vector3.Dot(vT, vP) * inverseDet;
            //The intersection lies outside of the triangle
            if (u < 0f || u > 1f)
                return false;

            var vQ = Vector3.Cross(vT, edge1);

            var v = Vector3.Dot(ray.Direction, vQ) * inverseDet;
            //The intersection lies outside of the triangle
            if (v < 0f || u + v > 1f)
                return false;

            var t = Vector3.Dot(edge2, vQ) * inverseDet;
            if (t > float.Epsilon)
            {
                distance = t;
                return true;
            }

            return false;
        }

        #endregion

        private static bool NearZero(float value)
        {
            if (value == 0f)
                return true;
            return value > -float.Epsilon && value < float.Epsilon;
        }
    }
}
