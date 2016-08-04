using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;

namespace Poly3D.Engine.Meshes
{
    public interface IVertex
    {
        Vector3 Position { get; }
        Vector3 Normal { get; }
        Vector2? UV { get; }
        bool IsTextured { get; }
    }
}
