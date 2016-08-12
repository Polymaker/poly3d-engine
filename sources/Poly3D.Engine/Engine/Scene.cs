using Poly3D.Engine.UI;
using Poly3D.Engine.Rendering;
using Poly3D.Maths;
using Poly3D.Platform;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Poly3D.Prefabs.Scripts;

namespace Poly3D.Engine
{
    public class Scene
    {
        // Fields...
        private SceneState _State;
        private IEngineDisplay _Display;
        private List<EngineObject> _Objects;
        private LoopController EngineLoop;

        public IEngineDisplay Display
        {
            get { return _Display; }
        }

        public bool IsRunning
        {
            get { return State == SceneState.Running; }
        }

        public SceneState State
        {
            get { return _State; }
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
            _Objects = new List<EngineObject>();
            _State = SceneState.Initializing;
            UpdateDelay = new Stopwatch();
            //Initialize();
        }

        #region Initialization

        internal void AssignDisplay(IEngineDisplay display)
        {
            _Display = display;
            Initialize();
        }

        private void Initialize()
        {
            EngineLoop = new LoopController(Display);
            EngineLoop.RenderFrame += EngineLoop_RenderFrame;
            EngineLoop.UpdateFrame += EngineLoop_UpdateFrame;
            _State = SceneState.Suspended;
            EngineLoop.ForceRender();
        }

        public static Scene CreateDefault()
        {
            var scene = new Scene();
            scene.CreateDefaultCamera();
            return scene;
        }

        private void CreateDefaultCamera()
        {
            var camera = AddObject<Camera>();
            camera.Active = true;
            camera.Transform.Position = new OpenTK.Vector3(10, 10, 10);
            camera.Transform.LookAt(new OpenTK.Vector3(0, 0f, 0));
            camera.AddComponent<PanOrbitCamera>();
        }

        #endregion

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

        private void EngineLoop_RenderFrame(object sender, OpenTK.FrameEventArgs e)
        {
            RenderScene();
        }

        private Stopwatch UpdateDelay;

        private void EngineLoop_UpdateFrame(object sender, OpenTK.FrameEventArgs e)
        {
            UpdateDelay.Restart();
            var engineObjects = EngineObjects.ToArray();
            foreach (var so in engineObjects)
            {
                if (!so.IsActive)
                    continue;
                var objectComponents = so.Components.ToArray();
                foreach (var objBehave in objectComponents.OfType<ObjectBehaviour>())
                {
                    if (!objBehave.Enabled)
                        continue;

                    objBehave.DoUpdate((float)e.Time + (float)UpdateDelay.Elapsed.TotalSeconds);
                }
            }
            UpdateDelay.Stop();
            
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

        #region Pause/Resume

        public void Pause()
        {
            if (IsRunning)
            {
                EngineLoop.Stop();
                _State = SceneState.Suspended;
            }
        }

        public void Resume()
        {
            if (State == SceneState.Suspended)
            {
                EngineLoop.Start();
                _State = SceneState.Running;
            }
        }

        #endregion

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
            if (NameExists(newName, engineObject))
            {
                if (string.IsNullOrEmpty(currentName))
                    currentName = GetDefaultName(engineObject);
            }
            else
                currentName = newName;
        }

        internal bool NameExists(string name, EngineObject exclude = null)
        {
            return _Objects.Any(o => !string.IsNullOrEmpty(o.Name) && o.Name.Equals(name) && o != exclude);
        }

        internal string GetDefaultName(EngineObject engineObject)
        {
            var objectType = engineObject.GetType();
            var instanceNb = Objects.Where(o => o.GetType() == objectType && o != engineObject).Count() + 1;
            var defaultName = objectType.Name + instanceNb;
            while (NameExists(defaultName, engineObject))
            {
                defaultName = objectType.Name + (++instanceNb);
            }
            return defaultName;
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
