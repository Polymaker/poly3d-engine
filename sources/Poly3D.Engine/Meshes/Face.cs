using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Poly3D.Engine.Meshes
{
    public /*abstract*/ class Face : MeshElement, IFace
    {
        private MeshMaterial _Material;
        private Surface _Surface;
        private MeshGroup _Group;
        private bool _IsNormalComputed;
        private Vector3 _Normal = Vector3.Zero;
        protected Vertex[] _Vertices;

        IVertex[] IFace.Vertices
        {
            get { return _Vertices; }
        }

        public Vertex[] Vertices
        {
            get { return _Vertices; }
        }

        public Vertex this[int index]
        {
            get { return Vertices[index]; }
            set
            {
                if (SetVertexValue(value, index))
                    _IsNormalComputed = false;
            }
        }

        public Surface Surface
        {
            get { return _Surface; }
            internal set
            {
                _Surface = value;
            }
        }

        public MeshGroup Group
        {
            get { return _Group; }
            set
            {
                value = value ?? MeshGroup.Default;

                if (_Group == value)
                    return;
                
                _Group = value;
            }
        }

        public MeshMaterial Material
        {
            get
            {
                if (_Material == MeshMaterial.Default && Surface != null)
                    return Surface.Material;
                return _Material;
            }
            set
            {
                value = value ?? MeshMaterial.Default;
                if (value == _Material)
                    return;
                _Material = value;
            }
        }

        public int[] Indices
        {
            get
            {
                if (Mesh == null)
                    return new int[0];
                return Vertices.Select(v => v.Index).ToArray();
            }
        }

        public Vector3 Normal
        {
            get { return _Normal; }
        }

        public bool IsNormalComputed
        {
            get { return _IsNormalComputed; }
        }
        
        public virtual FaceType Type
        {
            get 
            {
                if (Vertices.Length == 4)
                    return FaceType.Quad;
                else if (Vertices.Length == 3)
                    return FaceType.Triangle;
                return FaceType.Complex;
            }
        }

        internal Face()
        {
            _IsNormalComputed = false;
            _Vertices = null;
            _Surface = null;
            _Group = MeshGroup.Default;
            _Material = MeshMaterial.Default;
        }

        public Face(params Vertex[] vertices)
        {
            if (vertices.Length < 3)
                throw new ArgumentException("Minimum vertex number is 3.");
            _Vertices = vertices;
            ComputeNormal();
        }

        public void ComputeNormal()
        {
            var u = Vertices[1].Position - Vertices[0].Position;
            var v = Vertices[2].Position - Vertices[0].Position;
            _Normal = new Vector3((u.Y * v.Z) - (u.Z * v.Y), (u.Z * v.X) - (u.X * v.Z), (u.X * v.Y) - (u.Y * v.X));
            _IsNormalComputed = true;
        }

        public virtual bool Contains(IVertex vertex)
        {
            return Vertices.Contains(vertex);
        }

        public IList<Edge> GetEdges()
        {
            var edges = new List<Edge>();

            for (int i = 0; i < Vertices.Length; i++)
                edges.Add(new Edge(Vertices[i], Vertices[(i + 1) % Vertices.Length]));

            //for (int i = 0; i < Vertices.Length - 1; i++)
            //    edges.Add(new Edge(Vertices[i], Vertices[i + 1]));
            //edges.Add(new Edge(Vertices[Vertices.Length - 1], Vertices[0]));
            return edges.AsReadOnly();
        }

        protected override void OnParentAssigned()
        {
            foreach (var vert in Vertices)
            {
                if (vert.Mesh != Mesh)
                    vert.SetParent(Mesh, false);
            }
            if (Mesh == null)
                _Surface = null;
        }

        protected bool SetVertexValue(Vertex vert, int index)
        {
            if (vert == null || index < 0 || index >= Vertices.Length)
                return false;
            if (Vertices[index] == vert)
                return false;

            if (vert.Mesh == null)
                _Vertices[index] = vert;
            else if(Mesh != vert.Mesh)
            {
                _Vertices[index] = vert.CloneValue();
            }

            if(Mesh != null)
                _Vertices[index].SetParent(Mesh, false);

            return true;
        }
    }
}
