using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class SceneObject : EngineObject
    {
        // Fields...
        private SceneObject _Parent;
        private SceneObjectCollection _Childs;
        private Transform _Transform;
        
        /// <summary>
        /// 
        /// </summary>
        public override bool IsActive
        {
            get
            {
                return Active && (Parent == null || Parent.IsActive);
            }
        }

        public SceneObject Parent
        {
            get { return _Parent; }
            set
            {
                if (_Parent == value)
                    return;

                SetParent(value, false);
            }
        }

        public int HierarchyLevel
        {
            get { return Parent == null ? 0 : Parent.HierarchyLevel + 1; }
        }

        public SceneObject RootObject
        {
            get
            {
                if (Parent == null)
                    return this;
                return Parent.RootObject;
            }
        }

        public bool IsRoot
        {
            get { return HierarchyLevel == 0; }
        }

        public Transform Transform
        {
            get { return _Transform; }
            set
            {
                SetTransform(value, true);
            }
        }

        public IList<SceneObject> Childs
        {
            get { return _Childs/*.AsReadOnly()*/; }
        }

        public IEnumerable<SceneObject> AllChilds
        {
            get { return Childs.Concat(Childs.SelectMany(c => c.AllChilds)); }
        }

        public event EventHandler HierarchyChanged;

        public SceneObject()
        {
            _Transform = new Transform(this);
            AddComponent(_Transform);
            //_Components.Add(_Transform);
            _Childs = new SceneObjectCollection(this);
            _Parent = null;
        }

        private void OnHierarchyChangedInternal()
        {
            if (Transform != null)
                Transform.isWorldMatrixDirty = true;
            OnHierarchyChanged(EventArgs.Empty);
            foreach (var child in Childs)
                child.OnHierarchyChangedInternal();
        }

        protected virtual void OnHierarchyChanged(EventArgs ea)
        {
            var handler = HierarchyChanged;
            if (handler != null)
                handler(this, ea);
        }

        public T AddComponent<T>() where T : ObjectComponent
        {
            if (typeof(T) == typeof(Transform))
            {
                if (Transform == null)
                    Transform = new Transform(this);
                return Transform as T;
            }

            T comp = Activator.CreateInstance<T>();
            comp.SetOwner(this);
            AddComponent(comp);
            //_Components.Add(comp);
            return comp;
        }

        public T AddObject<T>() where T : SceneObject
        {
            if (Scene == null)
                return default(T);
            var newObject = Scene.AddObject<T>();
            _Childs.Add(newObject);
            return newObject;
        }

        public void SetTransform(Transform transform, bool keepWorldPosition)
        {
            transform = transform ?? new Transform();

            if (_Transform == transform)
                return;

            if (_Transform != null && Components.Contains(_Transform))
            {
                RemoveComponent(_Transform);
                _Transform.SetOwner(null);
            }

            _Transform = (Transform)transform.Clone();
            AddComponent(_Transform);
            _Transform.SetOwner(this);

            if (keepWorldPosition)
            {
                _Transform.WorldScale = transform.WorldScale;
                _Transform.WorldPosition = transform.WorldPosition;
                _Transform.WorldRotation = transform.WorldRotation;
            }
        }

        public IEnumerable<SceneObject> GetHierarchy(bool includeSelf = true)
        {
            var nodeList = new List<SceneObject>();
            SceneObject curParent = this;
            do
            {
                if (curParent != this || includeSelf)
                    nodeList.Add(curParent);
            }
            while ((curParent = curParent.Parent) != null);
            return nodeList.Reverse<SceneObject>();
        }

        internal void SetParent(SceneObject newParent, bool fromCollection = false)
        {
            if (_Parent == newParent)
                return;

            lock (this)
            {
                if (_Parent == newParent)
                    return;

                if (!fromCollection)
                {
                    if (AllChilds.Contains(newParent))
                        return;
                }

                if (_Parent != null)
                {
                    if (newParent != null)//previous and new parent are not null (changing owner)
                    {
                        _Parent._Childs.Remove(this);//remove current elem from previous parent

                        if (!fromCollection && !newParent._Childs.Contains(this))//if not called by ComponentCollection, add elem to the parent collection
                            newParent._Childs.Add(this);
                    }
                    else if (newParent == null && !fromCollection)//previous parent is not null but the new one is (removing from list)
                    {
                        _Parent._Childs.Remove(this);//remove current elem from previous parent
                    }
                }
                else if (newParent != null && !fromCollection)
                {
                    if (!newParent._Childs.Contains(this))
                        newParent._Childs.Add(this);
                }
                _Parent = newParent;
                if (Scene == null)
                    Scene = _Parent.Scene;
                OnHierarchyChangedInternal();
            }
        }

    }
}
