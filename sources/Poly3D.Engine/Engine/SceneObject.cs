using Poly3D.Maths;
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
            get { return GetAllChilds(); }
            //get { return Childs.Concat(Childs.SelectMany(c => c.AllChilds)); }
        }

        public int RenderLayer { get; set; }

        public event EventHandler HierarchyChanged;

        public event EventHandler ParentChanged;

        public SceneObject()
        {
            _Transform = new Transform();
            _Transform.Attach(this);
            AddComponent(_Transform);
            //_Components.Add(_Transform);
            _Childs = new SceneObjectCollection(this);
            _Parent = null;
        }

        private void OnHierarchyChangedInternal()
        {
            //if (Transform != null)
            //    Transform.isWorldMatrixDirty = true;
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

        private void OnParentChangedInternal()
        {
            OnParentChanged(EventArgs.Empty);
        }

        protected virtual void OnParentChanged(EventArgs ea)
        {
            var handler = ParentChanged;
            if (handler != null)
                handler(this, ea);
        }

        public SceneObject AddObject() 
        {
            if (Scene == null)
                return null;
            var newObject = Scene.AddObject<SceneObject>();
            _Childs.Add(newObject);
            return newObject;
        }

        public T AddObject<T>() where T : SceneObject
        {
            if (Scene == null)
                return default(T);
            var newObject = Scene.AddObject<T>();
            _Childs.Add(newObject);
            return newObject;
        }

        public IEnumerable<SceneObject> GetAllChilds()
        {
            foreach (var child in Childs)
            {
                yield return child;
            }
            foreach (var child in Childs)
            {
                foreach (var grandChild in child.GetAllChilds())
                    yield return grandChild;
            }
        }

        public T GetComponentDownward<T>() where T : IEngineComponent
        {
            var foundComp = GetComponent<T>();
            if (foundComp != null)
                return foundComp;
            foreach (var child in AllChilds)
            {
                foundComp = child.GetComponent<T>();
                if (foundComp != null)
                    return foundComp;
            }
            return default(T);
        }

        public T GetComponentUpward<T>() where T : IEngineComponent
        {
            var foundComp = GetComponent<T>();
            if (foundComp != null)
                return foundComp;

            if (Parent != null)
                return Parent.GetComponentUpward<T>();

            return default(T);
        }

        public void SetTransform(Transform transform, bool keepWorldPosition)
        {
            transform = transform ?? new Transform();

            if (_Transform == transform)
                return;

            if (keepWorldPosition)
            {
                _Transform.WorldScale = transform.WorldScale;
                _Transform.WorldPosition = transform.WorldPosition;
                _Transform.WorldRotation = transform.WorldRotation;
            }
            else
            {
                _Transform.Scale = transform.Scale;
                _Transform.Position = transform.Position;
                _Transform.Rotation = transform.Rotation;
            }
        }

        public void SetTransform(ComplexTransform transform, SceneSpace space)
        {
            Transform.SetTransform(transform, space);
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

                //adding itself into one of its child (this check is already done when adding object from the collection)
                if (!fromCollection && AllChilds.Contains(newParent))
                {
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
                OnParentChangedInternal();
                OnHierarchyChangedInternal();
            }
        }

    }
}
