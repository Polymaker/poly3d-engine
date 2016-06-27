using System;
using System.Collections.Generic;
using System.Linq;

namespace Poly3D.Engine.Meshes
{
    public interface IEdge
    {
        IVertex A { get; }
        IVertex B { get; }
        float Length { get; }

        bool Contains(IVertex vertex);
    }
}
