using OpenTK;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Meshes
{
    public static class MeshBuilder
    {

        public static Mesh LoadFromVNF(Vector3[] vertices, Vector3[] normals, Tuple<int, int, int>[] triangles)
        {
            if (vertices.Length != normals.Length)
                throw new InvalidDataException("vertices and normals must have the same number of elements");

            return new Mesh(triangles.Select(f =>
            new FaceTriangle(
                new Vertex(vertices[f.Item1], normals[f.Item1]),
                new Vertex(vertices[f.Item2], normals[f.Item2]),
                new Vertex(vertices[f.Item3], normals[f.Item3])
                )));
        }

        public static Mesh LoadFromVNTF(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, Tuple<int, int, int>[] triangles)
        {
            if (vertices.Length != normals.Length)
                throw new InvalidDataException("vertices and normals must have the same number of elements");

            if (uvs != null && vertices.Length != uvs.Length)
                throw new InvalidDataException("vertices and uvs must have the same number of elements");

            return new Mesh(triangles.Select(f =>
            new FaceTriangle(
                new Vertex(vertices[f.Item1], normals[f.Item1], f.Item1 >= 0 && uvs != null ? uvs[f.Item1] : (Vector2?)null),
                new Vertex(vertices[f.Item2], normals[f.Item2], f.Item2 >= 0 && uvs != null ? uvs[f.Item2] : (Vector2?)null),
                new Vertex(vertices[f.Item3], normals[f.Item3], f.Item3 >= 0 && uvs != null ? uvs[f.Item3] : (Vector2?)null)
                )));
        }

        public static Mesh LoadFromVNF(Vector3[] vertices, Vector3[] normals, Tuple<int, int, int, int>[] quads)
        {
            if (vertices.Length != normals.Length)
                throw new InvalidDataException("vertices and normals must have the same number of elements");

            return new Mesh(quads.Select(f =>
            new FaceQuad(
                new Vertex(vertices[f.Item1], normals[f.Item1]),
                new Vertex(vertices[f.Item2], normals[f.Item2]),
                new Vertex(vertices[f.Item3], normals[f.Item3]),
                new Vertex(vertices[f.Item4], normals[f.Item4])
                )));
        }

        public static Mesh LoadFromVNTF(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, Tuple<int, int, int, int>[] quads)
        {
            if (vertices.Length != normals.Length)
                throw new InvalidDataException("vertices and normals must have the same number of elements");

            if (uvs != null && vertices.Length != uvs.Length)
                throw new InvalidDataException("vertices and uvs must have the same number of elements");

            return new Mesh(quads.Select(f =>
            new FaceQuad(
                new Vertex(vertices[f.Item1], normals[f.Item1], f.Item1 >= 0 && uvs != null ? uvs[f.Item1] : (Vector2?)null),
                new Vertex(vertices[f.Item2], normals[f.Item2], f.Item2 >= 0 && uvs != null ? uvs[f.Item2] : (Vector2?)null),
                new Vertex(vertices[f.Item3], normals[f.Item3], f.Item3 >= 0 && uvs != null ? uvs[f.Item3] : (Vector2?)null),
                new Vertex(vertices[f.Item4], normals[f.Item4], f.Item4 >= 0 && uvs != null ? uvs[f.Item4] : (Vector2?)null)
                )));
        }
    
    }
}
