using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public interface IMesh
    {
        IList<IFace> Faces { get; }
        IList<IVertex> Vertices { get; }
        IList<ISurface> Surfaces { get; }
        bool IsTextured { get; }
    }
}
