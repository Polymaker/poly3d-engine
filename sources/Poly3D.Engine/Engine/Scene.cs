using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class Scene
    {
        // Fields...
        private List<SceneObject> _Objects;
        private IViewPort _Viewport;

        public IViewPort Viewport
        {
            get { return _Viewport; }
        }

        public IEnumerable<SceneObject> Objects
        {
            get { return _Objects.AsReadOnly(); }
        }

        public IEnumerable<Camera> ActiveCameras
        {
            get { return Objects.OfType<Camera>().Where(c => c.Active); }
        }

        public IEnumerable<Camera> AllCameras
        {
            get { return Objects.OfType<Camera>(); }
        }

        public Scene()
        {
            _Objects = new List<SceneObject>();
            _Viewport = null;
        }
    }
}
