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

        event EventHandler<EventArgs> SizeChanged;

        event EventHandler<EventArgs> Load;

        event EventHandler<EventArgs> Unload;
    }
}
