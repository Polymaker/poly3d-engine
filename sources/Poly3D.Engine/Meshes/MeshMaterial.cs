using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class MeshMaterial
    {
        private int _Id;
        private string _Name;

        public int Id
        {
            get { return _Id; }
            set
            {
                if (ReferenceEquals(this, Default))
                    return;
                _Id = value;
            }
        }

        public string Name
        {
            get { return _Name; }
            set
            {
                if (ReferenceEquals(this, Default))
                    return;
                _Name = value;
            }
        }

        public MeshMaterial()
        {
            Id = 0;
            Name = String.Empty;
        }

        public MeshMaterial(int id)
        {
            Id = id;
            Name = "Material" + id;
        }

        public MeshMaterial(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public static readonly MeshMaterial Default = new MeshMaterial(0, "Default");
    }
}
