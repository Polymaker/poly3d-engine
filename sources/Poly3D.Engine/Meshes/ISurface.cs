using System;
using System.Collections.Generic;
using System.Linq;

namespace Poly3D.Engine.Meshes
{
    public interface ISurface
    {
        IList<IFace> Faces { get; }
    }
}
