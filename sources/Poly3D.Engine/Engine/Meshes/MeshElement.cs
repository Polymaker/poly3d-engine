using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public abstract class MeshElement : IMeshElement
    {
        private Mesh _Mesh;

        public Mesh Mesh
        {
            get { return _Mesh; }
            protected set
            {
                SetParent(value, false);
            }
        }

        IMesh IMeshElement.Mesh
        {
            get { return _Mesh; }
        }

        public virtual int Index
        {
            get
            {
                if (Mesh != null)
                    return Mesh.GetElements(this).IndexOf(this);
                return -1;
            }
        }

        internal void SetParent(Mesh parent, bool fromCollection)
        {
            if (_Mesh == parent)
                return;

            lock (this)
            {
                if (_Mesh == parent)
                    return;

                if (_Mesh != null)
                {
                    var oldMeshCollection = GetType() == typeof(Vertex) ? (IList)_Mesh.Vertices : (IList)_Mesh.Faces;
                    
                    if (parent != null)//previous and new parent are not null (changing owner)
                    {
                        oldMeshCollection.Remove(this);//remove current elem from previous parent

                        var newMeshCollection = GetType() == typeof(Vertex) ? (IList)parent.Vertices : (IList)parent.Faces;
                        if (!fromCollection && !newMeshCollection.Contains(this))//if not called by MeshElementCollection, add elem to the parent collection
                            newMeshCollection.Add(this);
                    }
                    else if (parent == null && !fromCollection)//previous parent is not null but the new one is (removing from list)
                    {
                        oldMeshCollection.Remove(this);//remove current elem from previous parent
                    }
                }
                else if (parent != null && !fromCollection)
                {
                    var newMeshCollection = GetType() == typeof(Vertex) ? (IList)parent.Vertices : (IList)parent.Faces;
                    if(!newMeshCollection.Contains(this))
                        newMeshCollection.Add(this);
                }
                _Mesh = parent;
                
                //OnParentChanged(EventArgs.Empty);
                //OnPropertyChanged("Parent");
            }
            OnParentAssigned();
        }

        protected virtual void OnParentAssigned()
        {

        }


    }
}
