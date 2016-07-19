using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class SceneObject
    {
        // Fields...
        private Transform _Transform;
        private Scene _Scene;

        public Scene Scene
        {
            get { return _Scene; }
        }

        public Transform Transform
        {
            get { return _Transform; }
            set
            {
                _Transform = value;
            }
        }
        
    }
}
