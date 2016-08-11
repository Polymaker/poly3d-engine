using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Platform
{
    internal interface IGLSurface
    {
        IntPtr Handle { get; }
        bool Exists { get; }
        IGraphicsContext Context { get; }

        void MakeCurrent();

        void SwapBuffers();
    }
}
