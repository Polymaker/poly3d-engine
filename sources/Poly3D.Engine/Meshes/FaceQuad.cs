using System;
using System.Collections.Generic;
using System.Linq;

namespace Poly3D.Engine.Meshes
{
    public class FaceQuad : Face
    {
        public override FaceType Type
        {
            get { return FaceType.Quad; }
        }

        public Vertex Vertex0
        {
            get { return Vertices[0]; }
            set
            {
                Vertices[0] = value;
            }
        }

        public Vertex Vertex1
        {
            get { return Vertices[1]; }
            set
            {
                Vertices[1] = value;
            }
        }

        public Vertex Vertex2
        {
            get { return Vertices[2]; }
            set
            {
                Vertices[2] = value;
            }
        }

        public Vertex Vertex3
        {
            get { return Vertices[3]; }
            set
            {
                Vertices[3] = value;
            }
        }

        public FaceQuad(Vertex v0, Vertex v1, Vertex v2, Vertex v3)
            : base(new Vertex[] { v0, v1, v2, v3 }) { }

        public FaceQuad(IEnumerable<Vertex> vertices)
        {
            if (vertices.Count() != 4)
                throw new ArgumentException(string.Format("Incorrect number of vertices ({0}, 4 expected).", vertices.Count()));
            _Vertices = vertices.ToArray();
            ComputeNormal();
        }

    }
}
