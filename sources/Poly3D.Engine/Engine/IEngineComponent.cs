using Poly3D.Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public interface IEngineComponent
    {
        EngineObject EngineObject { get; }

        Scene Scene { get; }

        IEngineComponent Clone();
    }

    public interface IEngineComponent<T> : IEngineComponent where T : EngineObject
    {
        T EngineObject { get; }
    }
}
