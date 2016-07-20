using Poly3D.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class ComponentCollection : OwnerCollectionBase<SceneObject, ObjectComponent>
    {
        public ComponentCollection(SceneObject owner)
            : base(owner)
        {
            
        }

        public ComponentCollection(SceneObject owner, IEnumerable<ObjectComponent> source)
            : base(owner, source)
        {
            
        }

        protected override void SetChildOwner(ObjectComponent child)
        {
            child.SetParent(Owner, true);
        }

        protected override void UnsetChildOwner(ObjectComponent child)
        {
            child.SetParent(null, true);
        }

    }
}
