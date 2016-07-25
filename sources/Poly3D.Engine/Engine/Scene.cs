using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            internal set
            {
                _Viewport = value;
            }
        }

        public IEnumerable<SceneObject> Objects
        {
            get { return _Objects.AsReadOnly(); }
        }

        public IEnumerable<SceneObject> RootObjects
        {
            get { return Objects.Where(o => o.Parent == null); }
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
            InitMainCamera();
        }

        public Scene(IViewPort viewport)
        {
            _Viewport = viewport;
            _Objects = new List<SceneObject>();
            InitMainCamera();
        }

        private void InitMainCamera()
        {
            var camera = AddObject<Camera>();
            camera.Active = true;
            camera.Transform.Position = new OpenTK.Vector3(0, 3, 4);
            //camera.Transform.Rotation = new Rotation(0, 180, 0);
            camera.Transform.LookAt(new OpenTK.Vector3(0, 0f, 0));
            //camera.Transform.Rotation.Yaw += Angle.FromDegrees(-180);
            Trace.WriteLine(camera.Transform.Rotation);

        }

        public T AddObject<T>() where T : SceneObject
        {
            T sceneObj = Activator.CreateInstance<T>();
            sceneObj.Scene = this;
            _Objects.Add(sceneObj);
            return sceneObj;
        }

        internal void RenderScene()
        {
            foreach (var camera in ActiveCameras)
                camera.Render();
        }
    }
}
