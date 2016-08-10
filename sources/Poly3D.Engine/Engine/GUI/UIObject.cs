using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.GUI
{
    public class UIObject : EngineObject
    {
        public override bool IsActive
        {
            get { return Active; }
        }
    }
}
