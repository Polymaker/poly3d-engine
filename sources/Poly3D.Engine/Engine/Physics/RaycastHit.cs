using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Physics
{
    public class RaycastHit
    {
        private Ray _Ray;
        private SceneObject _Target;
        private float _Distance;
        private int _FaceIndex;

        public Ray Ray
        {
            get { return _Ray; }
        }

        public float Distance
        {
            get { return _Distance; }
        }

        public SceneObject Target
        {
            get { return _Target; }
        }

        public int FaceIndex
        {
            get { return _FaceIndex; }
        }

        public RaycastHit(Ray ray, SceneObject target, float distance, int faceIndex)
        {
            _Ray = ray;
            _Target = target;
            _Distance = distance;
            _FaceIndex = faceIndex;
        }
    }
}
