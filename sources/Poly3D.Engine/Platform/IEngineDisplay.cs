using Poly3D.Engine;
using Poly3D.Maths;
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

        bool IsIdle { get; }

        System.Drawing.Size Size { get; set; }

        DisplayEngineSettings Configuration { get; }

        float AspectRatio { get; }

        bool Focused { get; }

        event EventHandler DisplayChanged;

        event EventHandler Load;

        event EventHandler Unload;

        void LoadScene(Scene scene);

        System.Drawing.Rectangle GetDisplayBounds();
    }
}
