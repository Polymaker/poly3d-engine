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

        public static Mesh CreatePrimitive(Primitives type, float size)
        {
            return CreatePrimitive(type, size, size, size);
        }

        public static Mesh CreatePrimitive(Primitives type, float size1, float size2)
        {
            return CreatePrimitive(type, size1, size2, size1);
        }

        public static Mesh CreatePrimitive(Primitives type, float size1, float size2, float size3)
        {
            switch (type)
            {
                case Primitives.Plane:
                    return LoadFromVNTF(new Vector3[]
                        {
                            new Vector3(size1/2f, 0, size2/2f),
                            new Vector3(size1/2f, 0, -size2/2f),
                            new Vector3(-size1/2f, 0, -size2/2f),
                            new Vector3(-size1/2f, 0, size2/2f)
                        },
                        new Vector3[]
                        {
                            Vector3.UnitY,
                            Vector3.UnitY,
                            Vector3.UnitY,
                            Vector3.UnitY
                        },
                        new Vector2[]
                        {
                            new Vector2(1f, 1f),
                            new Vector2(1f, 0f),
                            new Vector2(0f, 0f),
                            new Vector2(0f, 1f)
                        },
                        new Tuple<int, int, int>[]
                        {
                            new Tuple<int, int, int>(0, 1, 2),
                            new Tuple<int, int, int>(0, 2, 3),
                        });
                case Primitives.Cube:

                    break;
            }
            return null;
        }
    
    }
}
