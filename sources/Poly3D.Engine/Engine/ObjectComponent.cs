using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class ObjectComponent : EngineObject
    {
        // Fields...
        private SceneObject _SceneObject;

        public SceneObject SceneObject
        {
            get { return _SceneObject; }
        }

        public ObjectComponent()
        {
            _SceneObject = null;
        }

        internal ObjectComponent(SceneObject sceneObject)
        {
            _SceneObject = sceneObject;
        }

        internal void SetOwner(SceneObject owner)
        {
            _SceneObject = owner;
        }
    }
}
