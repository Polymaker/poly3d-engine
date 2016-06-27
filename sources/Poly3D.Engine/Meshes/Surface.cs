using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public class Surface : ISurface
    {
        // Fields...
        private bool _IsBoundingEdgesComputed;
        private List<Edge> _BoundingEdges;
        private List<Face> _Faces;

        public IList<Face> Faces
        {
            get { return _Faces.AsReadOnly(); }
        }

        IList<IFace> ISurface.Faces
        {
            get { return _Faces.ToList<IFace>(); }
        }

        public IEnumerable<Vertex> Vertices
        {
            get { return _Faces.SelectMany(f=>f.Vertices).Distinct(); }
        }

        public Mesh Mesh
        {
            get { return Faces.Count > 0 ? Faces[0].Mesh : null; }
        }

        public IList<Edge> BoundingEdges
        {
            get { return _BoundingEdges.AsReadOnly(); }
        }

        public bool IsBoundingEdgesComputed
        {
            get { return _IsBoundingEdgesComputed; }
        }

        internal Surface()
        {
            _Faces = new List<Face>();
            _BoundingEdges = new List<Edge>();
        }

        internal Surface(IEnumerable<Face> faces)
        {
            _Faces = new List<Face>(faces);
            _BoundingEdges = new List<Edge>();
        }

        public bool Contains(IVertex vertex)
        {
            return Faces.Any(f => f.Contains(vertex));
        }

        public bool Contains(IFace face)
        {
            return Faces.Contains(face);
        }

        #region Sufaces computing

        public static List<Surface> ComputeSurfaces(IEnumerable<Face> faces)
        {
            var faceList = faces.ToList();
            var surfaces = new List<Surface>();
            var connectedFaces = new List<Face>();
            while (faceList.Count > 0)
            {
                AddConnectedFaces(faceList[0], faceList, connectedFaces);
                surfaces.Add(new Surface(connectedFaces));
                connectedFaces.Clear();
            }
            if(connectedFaces.Count > 0)
                surfaces.Add(new Surface(connectedFaces));
            return surfaces;
        }

        private static void AddConnectedFaces(Face curFace, List<Face> remainingFaces, List<Face> connectedFaces)
        {
            var addedFaces = GetConnectedFaces(curFace, remainingFaces).ToList();
            connectedFaces.AddRange(addedFaces);
            foreach (var face in addedFaces)
            {
                if (face == curFace)
                    continue;
                AddConnectedFaces(face, remainingFaces, connectedFaces);
            }
        }

        private static IEnumerable<Face> GetConnectedFaces(Face face, List<Face> remainingFaces)
        {
            return face.Vertices.SelectMany(v => GetConnectedFaces(v, remainingFaces));
        }

        private static IEnumerable<Face> GetConnectedFaces(Vertex vert, List<Face> remainingFaces)
        {
            var faces = remainingFaces.Where(f => f.Contains(vert)).ToList();
            faces.ForEach(f => remainingFaces.Remove(f));
            return faces;
        }

        #endregion

        public IList<Edge> GetBoundingEdges()
        {
            if (!IsBoundingEdgesComputed)
                ComputeBoundingEdges();
            return BoundingEdges;
        }

        public void ComputeBoundingEdges()
        {
            var allEdges = Faces.SelectMany(f => f.GetEdges()).ToList();

            _BoundingEdges = allEdges.GroupBy(e => e).Where(g => g.Count() == 1).Select(g => g.Key).ToList();
            _IsBoundingEdgesComputed = true;
        }
    }
}
