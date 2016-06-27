using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class Edge : IEdge
    {
        private Vertex _A;
        private Vertex _B;

        public Vertex A { get { return _A; } }

        public Vertex B { get { return _B; } }

        IVertex IEdge.A
        {
            get { return _A; }
        }

        IVertex IEdge.B
        {
            get { return _B; }
        }

        public float Length
        {
            get { return Math.Abs((B.Position - A.Position).Length); }
        }

        public float LengthFast
        {
            get { return Math.Abs((B.Position - A.Position).LengthFast); }
        }

        public Edge(Vertex a, Vertex b)
        {
            _A = a;
            _B = b;
        }

        public bool Contains(IVertex vertex)
        {
            return A == vertex || B == vertex;
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null) || !(obj is Edge))
                return base.Equals(obj);
            var edge = (Edge)obj;
            return (A == edge.A && B == edge.B) || (A == edge.B && B == edge.A);
        }

        public override int GetHashCode()
        {
            //to ensure that Edge(A,B) and Edge(B,A) will have the same hashcode we sort the vertices.
            var ha = A.GetHashCode();
            var hb = B.GetHashCode();

            var aa = ha < hb ? ha : hb;
            var bb = ha > hb ? ha : hb;

            return aa + bb * 13;
        }
    }
}
