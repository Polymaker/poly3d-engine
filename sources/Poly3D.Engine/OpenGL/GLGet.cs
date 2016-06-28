using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Poly3D.Engine.OpenGL
{
    public static class GLGet
    {
        public static bool InContext
        {
            get { return GraphicsContext.CurrentContext != null; }
        }

        public static int Integer(GetPName pname)
        {
            int val;
            GL.GetInteger(pname, out val);
            return val;
        }

        public static float Float(GetPName pname)
        {
            float val;
            GL.GetFloat(pname, out val);
            return val;
        }

    }
}
