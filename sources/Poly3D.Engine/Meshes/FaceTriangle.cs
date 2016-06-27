using System;
using System.Collections.Generic;
using System.Linq;

namespace Poly3D.Engine.Meshes
{
    public class FaceTriangle : Face
    {
        public override FaceType Type
        {
            get { return FaceType.Triangle; }
        }

        public Vertex Vertex0
        {
            get { return Vertices[0]; }
            set
            {
                SetVertexValue(value, 0);
            }
        }

        public Vertex Vertex1
        {
            get { return Vertices[1]; }
            set
            {
                SetVertexValue(value, 1);
            }
        }

        public Vertex Vertex2
        {
            get { return Vertices[2]; }
            set
            {
                SetVertexValue(value, 2);
            }
        }

        public FaceTriangle(Vertex v0, Vertex v1, Vertex v2)
            : base(new Vertex[] { v0, v1, v2 }) { }

        public FaceTriangle(IEnumerable<Vertex> vertices)
        {
            if (vertices.Count() != 3)
                throw new ArgumentException(string.Format("Incorrect number of vertices ({0}, 3 expected).", vertices.Count()));
            _Vertices = vertices.ToArray();
            ComputeNormal();
        }

    }
}
