using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.GUI
{
    public class UIComponent : BaseComponent<UIObject>
    {
        public UIComponent()
        {

        }

        public UIComponent(UIObject engineObject)
            : base(engineObject)
        {

        }
    }
}
