using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Shaders
{
    public class ShaderParameter
    {
        private string _Name;
        private int _Location;

        public int Location
        {
            get { return _Location; }
        }

        public string Name
        {
            get { return _Name; }
        }
    }

    public class ShaderParameter<T> : ShaderParameter
    {
        // Fields...
        private T _Value;

        public T Value
        {
            get { return _Value; }
            set
            {
                _Value = value;
            }
        }
    }
}
