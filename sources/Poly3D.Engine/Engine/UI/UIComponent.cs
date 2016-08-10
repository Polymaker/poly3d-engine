using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.UI
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
