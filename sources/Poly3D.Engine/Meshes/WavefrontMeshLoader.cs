using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using System.IO;

namespace Poly3D.Engine.Meshes
{
    public static class WavefrontMeshLoader
    {
        class FaceHelper
        {
            /// <summary>
            /// v,n,t
            /// </summary>
            public List<Tuple<int, int, int>> Vertices { get; private set; }

            public int Count { get { return Vertices.Count; } }

            public FaceHelper()
            {
                Vertices = new List<Tuple<int, int, int>>();
            }

            public FaceHelper(IEnumerable<Tuple<int, int, int>> verts)
            {
                Vertices = new List<Tuple<int, int, int>>(verts);
            }
            public static Tuple<int, int, int> GetIndices(string argStr)
            {
                int[] indices = new int[] { 0, -1, -1 };
                var vntIdx = argStr.Split('/');//keep empty element

                for (int i = 0; i < vntIdx.Length; i++)
                {
                    int idx = 0;
                    int.TryParse(vntIdx[i], out idx);
                    indices[i] = idx - 1;
                }
                //Vertex Indices:
                //f v1 v2 v3 ....
                //Vertex, Texture Indices:
                //f v1/vt1 v2/vt2 v3/vt3 ...
                //Vertex, Texture, Normal Indices
                //f v1/vt1/vn1 v2/vt2/vn2 v3/vt3/vn3 ....
                //Vertex, Normal Indices
                //f v1//vn1 v2//vn2 v3//vn3 ...
                return new Tuple<int, int, int>(indices[0], indices[2], indices[1]);
            }
        }

        public static Mesh LoadWavefrontObj(string filePath)
        {
            using (var fs = File.OpenRead(filePath))
                return LoadWavefrontObj(fs);
        }

        public static Mesh LoadWavefrontObj(Stream stream)
        {
            var vertices = new List<Vector3>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();
            var faces = new List<FaceHelper>();

            using (var sr = new StreamReader(stream))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    if (line.StartsWith("#") || string.IsNullOrEmpty(line))
                        continue;
                    var args = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    switch (args[0])
                    {
                        case "v":
                            if (args.Length >= 4)
                                vertices.Add(new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])));
                            break;
                        case "vn":
                            if (args.Length >= 4)
                                normals.Add(new Vector3(float.Parse(args[1]), float.Parse(args[2]), float.Parse(args[3])));
                            break;
                        case "vt":
                            if (args.Length >= 3)
                                uvs.Add(new Vector2(float.Parse(args[1]), float.Parse(args[2])));
                            break;
                        case "f":
                            var faceInfo = new FaceHelper();
                            for (int i = 1; i < args.Length; i++)
                                faceInfo.Vertices.Add(FaceHelper.GetIndices(args[i]));
                            faces.Add(faceInfo);
                            break;
                    }
                }
            }
            var vntList = faces.SelectMany(f => f.Vertices).Distinct().ToList();
            var vntDict = new Dictionary<Tuple<int, int, int>, Vertex>();
            foreach (var vntIdx in vntList)
            {
                var vert = new Vertex();
                vert.Position = vertices[vntIdx.Item1];
                vert.Normal = vntIdx.Item2 >= 0 ? normals[vntIdx.Item2] : Vector3.Zero;
                vert.UV = vntIdx.Item3 >= 0 ? uvs[vntIdx.Item3] : (Vector2?)null;
                vntDict.Add(vntIdx, vert);
            }
            var faceList = new List<Face>();
            foreach (var faceInfo in faces)
            {
                if (faceInfo.Count == 3)
                    faceList.Add(new FaceTriangle(faceInfo.Vertices.Select(vnt => vntDict[vnt])));
                else if (faceInfo.Count == 4)
                    faceList.Add(new FaceQuad(faceInfo.Vertices.Select(vnt => vntDict[vnt])));
            }
            return new Mesh(faceList);
        }

        public static void SaveWavefrontObj(Mesh mesh, string filepath)
        {
            using (var fs = File.Create(filepath))
                SaveWavefrontObj(mesh, fs);
        }

        public static void SaveWavefrontObj(Mesh mesh, Stream stream)
        {
            var orderedVerts = mesh.Vertices.OrderBy(v => v.Index);
            var vertices = orderedVerts.Select(v => v.Position).Distinct().ToList();
            var normals = orderedVerts.Select(v => v.Normal).Distinct().ToList();
            var uvs = orderedVerts.Where(v => v.IsTextured).Select(v => v.UV.Value).Distinct().ToList();

            using (var sw = new StreamWriter(stream))
            {
                foreach (var v in vertices)
                    sw.WriteLine(string.Format("v {0:0.######} {1:0.######} {2:0.######}", v.X, v.Y, v.Z));
                foreach (var v in normals)
                    sw.WriteLine(string.Format("vn {0:0.######} {1:0.######} {2:0.######}", v.X, v.Y, v.Z));
                foreach (var v in uvs)
                    sw.WriteLine(string.Format("vt {0:0.######} {1:0.######}", v.X, v.Y));

                foreach (var face in mesh.Faces)
                {
                    sw.Write("f");
                    foreach (var fv in face.Vertices)
                    {
                        WriteVertex(sw,
                            vertices.IndexOf(fv.Position),
                            normals.IndexOf(fv.Normal),
                            fv.UV.HasValue ? uvs.IndexOf(fv.UV.Value) : -1);
                    }
                    sw.Write(Environment.NewLine);
                }
                //var norms = mesh.Vertices.
            }
        }

        private static void WriteVertex(StreamWriter sw, int vIdx, int vnIdx, int vtIdx)
        {
            sw.Write(string.Format(" {0}/", vIdx + 1));
            if (vtIdx >= 0)
                sw.Write(vtIdx + 1);
            sw.Write("/");
            sw.Write(vnIdx + 1);
        }

    }
}
