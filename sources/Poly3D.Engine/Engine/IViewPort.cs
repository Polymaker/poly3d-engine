using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public interface IViewPort
    {
        int Width { get; }
        int Height { get; }
        float AspectRatio { get; }
    }
}
