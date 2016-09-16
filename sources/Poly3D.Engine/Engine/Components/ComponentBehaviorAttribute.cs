using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ComponentBehaviorAttribute : Attribute
    {
        public bool IsSingleton { get; set; }

    }
}
