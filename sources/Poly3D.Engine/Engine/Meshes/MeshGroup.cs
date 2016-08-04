using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class MeshGroup
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

        public MeshGroup()
        {
            Id = 0;
            Name = String.Empty;
        }

        public MeshGroup(int id)
        {
            Id = id;
            Name = "Group" + id;
        }

        public MeshGroup(int id, string name)
        {
            Id = id;
            Name = name;
        }
        
        public static readonly MeshGroup Default = new MeshGroup(0, "Default");
    }
}
