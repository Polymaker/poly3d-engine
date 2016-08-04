using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace OpenTK.Graphics.OpenGL
{
    public sealed class GLDraw : IDisposable
    {
        private static BeginMode currentMode = (BeginMode)(int)-1;

        private GLDraw() { }

        public static IDisposable Begin(BeginMode drawmode)
        {
            if ((int)currentMode != -1 && currentMode != drawmode)
                throw new InvalidOperationException(string.Format("Previous drawing ({0}) has not ended. Cannot change mode.", currentMode));
            GL.Begin(drawmode);
            return new GLDraw();
        }

        public static IDisposable Begin(BeginMode drawmode, MaterialFace face, PolygonMode polymode)
        {
            GL.PolygonMode(face, polymode);
            return Begin(drawmode);
        }

        public void Dispose()
        {
            GL.End();
            currentMode = (BeginMode)(int)-1;
        }
    }
}
