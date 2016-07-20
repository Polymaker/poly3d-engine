﻿using Poly3D.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class SceneObjectChildCollection : OwnerCollectionBase<SceneObject, SceneObject>
    {
        public SceneObjectChildCollection(SceneObject owner)
            : base(owner)
        {
            
        }

        public SceneObjectChildCollection(SceneObject owner, IEnumerable<SceneObject> source)
            : base(owner, source)
        {
            
        }

        protected override void SetChildOwner(SceneObject child)
        {
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
