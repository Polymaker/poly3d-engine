using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Physics
{
    public class RaycastHit
    {
        // Fields...
        private int _FaceIndex;
        private SceneObject _Target;
        private float _Distance;
        private Ray _Ray;

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
        
    }
}
