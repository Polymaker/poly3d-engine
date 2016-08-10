using Poly3D.Engine.GUI;
using Poly3D.Engine.Rendering;
using Poly3D.Maths;
using Poly3D.Platform;
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
        private IEngineDisplay _Display;
        private List<SceneObject> _ObjectsOld;
        private IViewPort _Viewport;
        private List<EngineObject> _Objects;

        [Obsolete("Use Scene.Display (when it will be implemented...).", false)]
        public IViewPort Viewport
        {
            get { return _Viewport; }
            internal set
            {
                _Viewport = value;
            }
        }

        public IEngineDisplay Display
        {
            get { return _Display; }
        }

        public IEnumerable<EngineObject> EngineObjects
        {
            get { return _Objects.AsReadOnly(); }
        }

        public IEnumerable<SceneObject> Objects
        {
            get { return EngineObjects.OfType<SceneObject>(); }
        }

        public IEnumerable<UIObject> UIObjects
        {
            get { return EngineObjects.OfType<UIObject>(); }
        }

        public IEnumerable<SceneObject> RootObjects
        {
            get { return Objects.Where(o => o.Parent == null); }
        }

        public IEnumerable<Camera> ActiveCameras
        {
            get { return Objects.OfType<Camera>().Where(c => c.IsActive); }
        }

        public IEnumerable<Camera> AllCameras
        {
            get { return Objects.OfType<Camera>(); }
        }

        public Scene()
        {
            _ObjectsOld = new List<SceneObject>();
            _Objects = new List<EngineObject>();
            _Viewport = null;
            InitMainCamera();
        }

        public Scene(IViewPort viewport)
        {
            _Viewport = viewport;
            _ObjectsOld = new List<SceneObject>();
            _Objects = new List<EngineObject>();
            InitMainCamera();
        }

        private void InitMainCamera()
        {
            var camera = AddObject<Camera>();
            camera.Active = true;
            camera.Transform.Position = new OpenTK.Vector3(10, 10, 10);
            camera.Transform.LookAt(new OpenTK.Vector3(0, 0f, 0));
        }

        public T AddObject<T>() where T : EngineObject
        {
            T sceneObj = Activator.CreateInstance<T>();
            sceneObj.Initialize(this);
            _Objects.Add(sceneObj);
            return sceneObj;
        }

        internal void RenderScene()
        {
            if (!ActiveCameras.Any())
            {
                Trace.WriteLine("Nothing to render, no active camera!");
                return;
            }
            foreach (var camera in ActiveCameras.OrderBy(c => c.RenderPriority))
            {
                SceneRenderer.RenderCamera(camera);
            }
        }

        public void OnUpdate(float deltaTime)
        {
            var delay = Stopwatch.StartNew();
            foreach (var so in EngineObjects)
            {
                if (!so.IsActive)
                    continue;
                foreach (var objBehave in so.Components.OfType<ObjectBehaviour>())
                {
                    if (!objBehave.Enabled)
                        continue;
                    
                    objBehave.DoUpdate(deltaTime + (float)delay.Elapsed.TotalSeconds);
                }
            }
        }

        #region GetObject

        public EngineObject GetObjectById(long instanceId)
        {
            return _Objects.FirstOrDefault(o => o.GetInstanceId() == instanceId);
        }

        public EngineObject GetObjectByName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return null;
            return _Objects.FirstOrDefault(o => o.Name == name);
        }

        public EngineObject GetObjectByTag(object tag)
        {
            if (tag == null)
                return null;
            return _Objects.FirstOrDefault(o => o.Tag == tag);
        }

        public EngineObject[] GetObjectsByTag(object tag)
        {
            if(tag == null)
                return new EngineObject[0];
            return _Objects.Where(o => o.Tag == tag).ToArray();
        }

        #endregion

        #region EngineObject name management

        internal void SetObjectName(EngineObject engineObject, ref string currentName, string newName)
        {
            if (_Objects.Any(o => !string.IsNullOrEmpty(o.Name) && o.Name.Equals(newName) && o != engineObject))
            {
                if (string.IsNullOrEmpty(currentName))
                    currentName = "EngineObject" + engineObject.GetInstanceId();
            }
            else
                currentName = newName;
        }

        #endregion

        public static Scene Current
        {
            get
            {
                //TODO: implement a way to know the current scene from the caller/calling thread
                throw new NotImplementedException();
            }
        }
    }
}
