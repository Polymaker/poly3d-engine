using Poly3D.Engine.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public interface IComponent
    {
        EngineObject EngineObject { get; }
    }
}
