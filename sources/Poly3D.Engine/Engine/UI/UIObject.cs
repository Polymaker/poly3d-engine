using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.UI
{
    public class UIObject : EngineObject
    {
        private UIContainer _Parent;

        public override bool IsActive
        {
            get { return Active && (Parent == null || Parent.IsActive); }
        }

        public UIContainer Parent
        {
            get { return _Parent; }
            internal set
            {
                _Parent = value;
            }
        }
        
    }
}
