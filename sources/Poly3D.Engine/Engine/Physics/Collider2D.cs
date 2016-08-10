using Poly3D.Engine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Physics
{
    public class Collider2D : UIComponent
    {
        private bool _Enabled;

        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
                _Enabled = value;
            }
        }
    }
}
