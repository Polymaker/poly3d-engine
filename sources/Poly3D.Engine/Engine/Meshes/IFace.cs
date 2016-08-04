using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poly3D.Engine.Meshes
{
    public interface IFace
    {
        IVertex[] Vertices { get; }
        Vector3 Normal { get; }
        FaceType Type { get; }
        bool IsNormalComputed { get; }

        bool Contains(IVertex vertex);
    }
}
