using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    //all vertices of a submesh will have the sames elements (normal, tangent, uv)
    public class SubMesh
    {
        private Vector3[] _Positions;
        private Vector3[] _Normals;
        private Vector2[] _UVs;
        private Vector3[] _Tangents;

        public Vector3[] Positions
        {
            get { return _Positions; }
            internal set { SetPositions(value); }
        }

        public Vector3[] Normals
        {
            get { return _Normals; }
            internal set { SetNormals(value); }
        }

        public Vector3[] Tangents
        {
            get { return _Tangents; }
            internal set { SetTangents(value); }
        }

        public Vector2[] UVs
        {
            get { return _UVs; }
            internal set { SetUVs(value); }
        }

        public bool HasNormals
        {
            get { return _Normals != null && _Normals.Length > 0; }
        }

        public bool HasTangents
        {
            get { return _Tangents != null && _Tangents.Length > 0; }
        }

        public bool IsTextured
        {
            get { return _UVs != null && _UVs.Length > 0; }
        }

        public SubMesh()
        {
            _Positions = new Vector3[0];
            _Normals = new Vector3[0];
            _Tangents = new Vector3[0];
            _UVs = new Vector2[0];
        }

        public SubMesh(Vector3[] positions, Vector3[] normals, Vector2[] uVs, Vector3[] tangents)
        {
            _Positions = positions;
            _Normals = normals;
            _UVs = uVs;
            _Tangents = tangents;
        }

        internal void SetPositions(Vector3[] positions)
        {
            //_Vertices.Clear();
            _Positions = positions;

            if (Normals.Length != positions.Length)
                _Normals = new Vector3[0];
            if (Tangents.Length != positions.Length)
                _Tangents = new Vector3[0];
            if (UVs.Length != UVs.Length)
                _UVs = new Vector2[0];
            
        }

        internal void SetNormals(Vector3[] normals)
        {
            if (normals.Length != 0 && normals.Length != Positions.Length)
                throw new Exception("number");

            _Normals = normals;
        }

        internal void SetTangents(Vector3[] tangents)
        {
            if (tangents.Length != 0 && tangents.Length != Positions.Length)
                throw new Exception("number");

            _Tangents = tangents;
        }

        internal void SetUVs(Vector2[] uvs)
        {
            if (uvs.Length != 0 && uvs.Length != Positions.Length)
                throw new Exception("number");

            _UVs = uvs;
        }

    }
}
