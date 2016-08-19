﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class Mesh
    {
        private Vector3[] _Normals;
        private Vector3[] _Positions;
        private Vector3[] _Tangents;
        private Vector2[] _UVs;
        private List<Vertex> _Vertices;
        
        public Vector3[] Positions
        {
            get { return _Positions; }
            set { SetPositions(value); }
        }
        
        public Vector3[] Normals
        {
            get { return _Normals; }
            set { SetNormals(value); }
        }
        
        public Vector3[] Tangents
        {
            get { return _Tangents; }
            set { SetTangents(value); }
        }

        public Vector2[] UVs
        {
            get { return _UVs; }
            set { SetUVs(value); }
        }

        public List<Vertex> Vertices
        {
            get { return _Vertices; }
        }

        public Mesh()
        {
            _Vertices = new List<Vertex>();
            _Positions = new Vector3[0];
            _Normals = new Vector3[0];
            _Tangents = new Vector3[0];
            _UVs = new Vector2[0];
        }

        public void SetPositions(Vector3[] positions)
        {
            _Vertices.Clear();
            _Positions = positions;

            if (Normals.Length != positions.Length)
                _Normals = new Vector3[0];
            if (Tangents.Length != positions.Length)
                _Tangents = new Vector3[0];
            if (UVs.Length != UVs.Length)
                _UVs = new Vector2[0];

            _Vertices.AddRange(Enumerable.Range(0, positions.Length).Select(i => new Vertex(this, i)));
        }

        public void SetNormals(Vector3[] normals)
        {
            if (normals.Length != 0 && normals.Length != Positions.Length)
                throw new Exception("number");

            _Normals = normals;
        }

        public void SetTangents(Vector3[] tangents)
        {
            if (tangents.Length != 0 && tangents.Length != Positions.Length)
                throw new Exception("number");

            _Tangents = tangents;
        }

        public void SetUVs(Vector2[] uvs)
        {
            if (uvs.Length != 0 && uvs.Length != Positions.Length)
                throw new Exception("number");

            _UVs = uvs;
        }

        public void Clear()
        {
            _Vertices = new List<Vertex>();
            _Positions = new Vector3[0];
            _Normals = new Vector3[0];
            _Tangents = new Vector3[0];
            _UVs = new Vector2[0];
        }
    }
}
