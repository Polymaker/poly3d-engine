using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    [Obsolete("Replaced by IEngineDisplay from EngineControl and EngineWindow.", false)]
    public interface IViewPort
    {
        int Width { get; }
        int Height { get; }
        float AspectRatio { get; }
    }
}
