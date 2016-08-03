using Poly3D.Engine.Meshes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine
{
    public class ObjectMesh : ObjectComponent
    {
        // Fields...
        private Material _Material;
        private Mesh _Mesh;

        public Mesh Mesh
        {
            get { return _Mesh; }
            set
            {
                _Mesh = value;
            }
        }

        public Material Material
        {
            get { return _Material; }
            set
            {
                _Material = value;
            }
        }

        public int MeshMaterialCount
        {
            get
            {
                if (Mesh == null)
                    return 0;
                return Mesh.Materials.Count();
            }
        }

        public ObjectMesh()
        {
            _Material = new Material();
            _Mesh = null;
        }
        
    }
}
