using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Platform
{
    public interface IEngineDisplay
    {
        int Width { get; set; }

        int Height { get; set; }

        System.Drawing.Size Size { get; set; }

        float AspectRatio { get; }

        event EventHandler SizeChanged;

        event EventHandler Load;

        event EventHandler Unload;
    }
}
