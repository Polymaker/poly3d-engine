using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class Vertex
    {
        // Fields...
        private Mesh _Mesh;
        private int _Index;

        public Mesh Mesh
        {
            get { return _Mesh; }
        }

        public int Index
        {
            get { return _Index; }
        }
        
        public Vector3 Position
        {
            get { return Mesh.Positions[Index]; }
        }

        public Vector3? Normal
        {
            get { return Mesh.Normals != null && Mesh.Normals.Length > 0 ? Mesh.Normals[Index] : (Vector3?)null; }
        }

        public Vector3? Tangent
        {
            get { return Mesh.Tangents != null && Mesh.Tangents.Length > 0 ? Mesh.Tangents[Index] : (Vector3?)null; }
        }

        public Vector2? UV
        {
            get { return Mesh.UVs != null && Mesh.UVs.Length > 0 ? Mesh.UVs[Index] : (Vector2?)null; }
        }

        internal Vertex(Mesh mesh, int index)
        {
            _Mesh = mesh;
            _Index = index;
        }
    }
}
