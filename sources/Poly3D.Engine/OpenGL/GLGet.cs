using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenTK.Graphics.OpenGL
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

        public static bool Boolean(GetPName pname)
        {
            bool val;
            GL.GetBoolean(pname, out val);
            return val;
        }

        public static Matrix4 Matrix(MatrixMode mMode)
        {
            float[] matValues = new float[16];
            switch (mMode) {
                case MatrixMode.Modelview:
                    GL.GetFloat(GetPName.ModelviewMatrix, matValues);
                    break;
                case MatrixMode.Projection:
                    GL.GetFloat(GetPName.ProjectionMatrix, matValues);
                    break;
                default:
                    return Matrix4.Identity;
            }
            
            int idx = 0;
            return new Matrix4(
                matValues[idx++], matValues[idx++], matValues[idx++], matValues[idx++], 
                matValues[idx++], matValues[idx++], matValues[idx++], matValues[idx++], 
                matValues[idx++], matValues[idx++], matValues[idx++], matValues[idx++], 
                matValues[idx++], matValues[idx++], matValues[idx++], matValues[idx++]);
        }
    }
}
