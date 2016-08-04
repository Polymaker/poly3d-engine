using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class Vertex : MeshElement, IVertex
    {
        
        private Vector3 _Position;
        private Vector3 _Normal;
        private Vector2? _UV;

        public Vector3 Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
            }
        }

        public Vector3 Normal
        {
            get { return _Normal; }
            set
            {
                _Normal = value;
            }
        }

        public Vector2? UV
        {
            get { return _UV; }
            set
            {
                _UV = value;
            }
        }

        public bool IsTextured
        {
            get { return UV.HasValue; }
        }

        public Vertex()
            : this(Vector3.Zero, Vector3.Zero, null) { }

        public Vertex(Vector3 position)
            : this(position, Vector3.Zero, null) { }

        public Vertex(Vector3 position, Vector3 normal)
            : this(position, normal, null) { }

        public Vertex(Vector3 position, Vector3 normal, Vector2? uV)
        {
            _Position = position;
            _Normal = normal;
            _UV = uV;
        }

        public IEnumerable<Face> GetConnectedFaces()
        {
            if (Mesh == null)
                return new Face[0];
            return Mesh.Faces.Where(f => f.Contains(this));
        }

        public IEnumerable<Edge> GetConnectedEdges()
        {
            var faces = GetConnectedFaces();
            var edges = faces.SelectMany(f => f.GetEdges().Where(e => e.Contains(this)));
            return edges.Distinct();
        }

        internal Vertex CloneValue()
        {
            return new Vertex(Position, Normal, UV);
        }
    }
}
