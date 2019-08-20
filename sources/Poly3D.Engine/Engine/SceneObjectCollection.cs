using Poly3D.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public sealed class SceneObjectCollection : OwnerCollectionBase<SceneObject, SceneObject>
    {
        public SceneObjectCollection(SceneObject owner)
            : base(owner)
        {
            
        }

        public SceneObjectCollection(SceneObject owner, IEnumerable<SceneObject> source)
            : base(owner, source)
        {
            
        }

        protected override void SetChildOwner(SceneObject child)
        {
            if (!child.isInitialized && Owner.isInitialized)
                child.AssignScene(Owner.Scene);
            child.SetParent(Owner, true);
        }

        protected override bool OnAddingItem(SceneObject item)
        {
            if (item.AllChilds.Contains(Owner))
                return false;
            return base.OnAddingItem(item);
        }

        protected override void UnsetChildOwner(SceneObject child)
        {
            child.SetParent(null, true);
        }
    }
}
