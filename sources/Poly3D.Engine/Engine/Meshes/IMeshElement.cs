using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public interface IMeshElement
    {
        IMesh Mesh { get; }
        int Index { get; }
    }
}
