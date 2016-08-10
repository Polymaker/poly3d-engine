using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.UI
{
    public class UIContainer : UIObject
    {
        // Fields...
        private List<UIObject> _Childs;

        public List<UIObject> Childs
        {
            get { return _Childs; }
        }

        public UIContainer()
        {
            _Childs = new List<UIObject>();
        }
    }
}
