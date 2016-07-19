using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class Scene
    {
        // Fields...
        private IViewPort _DisplayTarget;

        public IViewPort DisplayTarget
        {
            get { return _DisplayTarget; }
        }
        
    }
}
