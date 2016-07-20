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
            //set
            //{
            //    SetParent(value, false);
            //}
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

        internal void SetParent(SceneObject parent, bool fromCollection)
        {
            //if (_SceneObject == parent)
            //    return;

            //lock (this)
            //{
            //    if (_SceneObject == parent)
            //        return;

            //    if (_SceneObject != null)
            //    {
            //        if (parent != null)//previous and new parent are not null (changing owner)
            //        {
            //            _SceneObject.Components.Remove(this);//remove current elem from previous parent
                        
            //            if (!fromCollection && !parent.Components.Contains(this))//if not called by ComponentCollection, add elem to the parent collection
            //                parent.Components.Add(this);
            //        }
            //        else if (parent == null && !fromCollection)//previous parent is not null but the new one is (removing from list)
            //        {
            //            _SceneObject.Components.Remove(this);//remove current elem from previous parent
            //        }
            //    }
            //    else if (parent != null && !fromCollection)
            //    {
            //        if (!parent.Components.Contains(this))
            //            parent.Components.Add(this);
            //    }
            //    _SceneObject = parent;
                
            //}
        }
    }
}
