using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class Manipulator/* : SceneObject*/
    {
        public static int SCREEN_SIZE = 64;

        public TransformType Type { get; set; }

        public SceneObject Target { get; set; }
    }
}
