using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class Mesh : IMesh
    {
        private MeshMaterial _Material;
        private MeshElementCollection<Face> _Faces;
        private MeshElementCollection<Vertex> _Vertices;
        private List<Surface> _Surfaces;

        public MeshElementCollection<Face> Faces
        {
            get { return _Faces; }
        }

        public MeshElementCollection<Vertex> Vertices
        {
            get { return _Vertices; }
        }

        public IList<Surface> Surfaces
        {
            get { return _Surfaces.AsReadOnly(); }
        }

        public MeshMaterial Material
        {
            get { return _Material; }
            set
            {
                value = value ?? MeshMaterial.Default;
                if (value == _Material)
                    return;
                _Material = value;
            }
        }

        public IEnumerable<MeshMaterial> Materials
        {
            get
            {
                return Faces.Select(f => f.Material).Distinct();
            }
        }

        #region IMesh properties

        IList<IFace> IMesh.Faces
        {
            get { return _Faces.ToList<IFace>(); }
        }

        IList<IVertex> IMesh.Vertices
        {
            get { return _Vertices.ToList<IVertex>(); }
        }

        IList<ISurface> IMesh.Surfaces
        {
            get { return _Surfaces.ToList<ISurface>(); }
        } 

        #endregion

        public bool IsTextured
        {
            get { return Vertices.Any(v => v.IsTextured); }
        }

        public Mesh()
        {
            _Faces = new MeshElementCollection<Face>(this);
            _Vertices = new MeshElementCollection<Vertex>(this);
            _Surfaces = new List<Surface>();
            _Vertices.CollectionChanged += Vertices_CollectionChanged;
            _Material = MeshMaterial.Default;
        }

        public Mesh(IEnumerable<Face> faces)
        {
            _Vertices = new MeshElementCollection<Vertex>(this);
            _Faces = new MeshElementCollection<Face>(this, faces);
            _Surfaces = new List<Surface>();
            _Vertices.CollectionChanged += Vertices_CollectionChanged;
            _Material = MeshMaterial.Default;
        }

        public Mesh(IEnumerable<Vertex> vertices)
        {
            _Vertices = new MeshElementCollection<Vertex>(this, vertices);
            _Faces = new MeshElementCollection<Face>(this);
            _Surfaces = new List<Surface>();
            _Vertices.CollectionChanged += Vertices_CollectionChanged;
            _Material = MeshMaterial.Default;
        }

        private void Vertices_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                foreach (Vertex removedVert in e.OldItems)
                {
                    _Faces.RemoveAll(f => f.Contains(removedVert));
                }
            }
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Reset)
            {
                _Faces.Clear();
            }
        }

        internal IList GetElements(Type elemType)
        {
            if (elemType == typeof(Vertex))
                return Vertices;
            else if (elemType == typeof(Face) || elemType.IsSubclassOf(typeof(Face)))
                return Faces;
            return null;
        }

        internal IList GetElements(IMeshElement element)
        {
            return GetElements(element.GetType());
        }

        public Face AddFaceFromIndices(params int[] indices)
        {
            if (indices.Length < 3)
                throw new ArgumentException("Minimum indices count is 3.");

            var vertList = new List<Vertex>();
            for (int i = 0; i < indices.Length; i++)
            {
                if (indices[i] < 0 || indices[i] > Vertices.Count)
                    throw new ArgumentOutOfRangeException("Invalid index specified.");
                vertList.Add(_Vertices[indices[i]]);
            }

            Face face = null;
            if (indices.Length == 3)
                face = new FaceTriangle(vertList);
            else if (indices.Length == 4)
                face = new FaceQuad(vertList);
            else
                face = new Face(vertList.ToArray());
            _Faces.Add(face);

            return face;
        }

        public void ComputeSurfaces()
        {
            _Surfaces.Clear();
            if (_Faces.Count > 0)
                _Surfaces.AddRange(Surface.ComputeSurfaces(_Faces));
        }
    }
}
