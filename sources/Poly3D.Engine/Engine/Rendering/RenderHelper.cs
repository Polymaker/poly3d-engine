﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Poly3D.Graphics;
using Poly3D.Maths;
using Poly3D.Engine.Meshes;

namespace Poly3D.Engine
{
    public static class RenderHelper
    {
        public static void RenderAxes(float length, float thickness)
        {
            var rad = thickness / 2f;
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            RenderAxis(Vector3.UnitX, rad, length, Color.Red);
            RenderAxis(Vector3.UnitY, rad, length, Color.LawnGreen);
            RenderAxis(Vector3.UnitZ, rad, length, Color.FromArgb(0x00, 0x66, 0xFF));
        }

        public static void RenderAxesContour(float length, float thickness)
        {
            var rad = thickness / 2f;
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            RenderAxis(Vector3.UnitX, rad, length, Color.Black);
            RenderAxis(Vector3.UnitY, rad, length, Color.Black);
            RenderAxis(Vector3.UnitZ, rad, length, Color.Black);
        }

        private static void RenderAxis(Vector3 axis, float radius, float length, Color color, bool arrow = true)
        {
            GL.PushMatrix();

            var rotMat = GLMath.RotationFromTo(Vector3.UnitY, axis);

            GL.Translate(axis * length * 0.5f);
            GL.MultMatrix(ref rotMat);

            DrawCylinder(radius, length, color, true);

            GL.PopMatrix();

            if (arrow)
            {
                GL.PushMatrix();

                GL.Translate(axis * length);
                GL.MultMatrix(ref rotMat);

                DrawCone(radius * 2f, radius * 6f, color);

                GL.PopMatrix();
            }
        }

        public static void DrawBox(Color color, BoundingBox box, float lineThickness = 1f)
        {
            GL.PushAttrib(AttribMask.LineBit);
            GL.LineWidth(lineThickness);

            GL.Color4(color);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            DrawQuad(new Vector3(box.Right, box.Top, box.Front),
                new Vector3(box.Left, box.Top, box.Front),
                new Vector3(box.Left, box.Bottom, box.Front),
                new Vector3(box.Right, box.Bottom, box.Front));

            DrawQuad(new Vector3(box.Left, box.Top, box.Back),
                new Vector3(box.Right, box.Top, box.Back),
                new Vector3(box.Right, box.Bottom, box.Back),
                new Vector3(box.Left, box.Bottom, box.Back));

            DrawQuad(new Vector3(box.Left, box.Top, box.Front),
                new Vector3(box.Left, box.Top, box.Back),
                new Vector3(box.Left, box.Bottom, box.Back),
                new Vector3(box.Left, box.Bottom, box.Front));

            DrawQuad(new Vector3(box.Right, box.Top, box.Back),
                new Vector3(box.Right, box.Top, box.Front),
                new Vector3(box.Right, box.Bottom, box.Front),
                new Vector3(box.Right, box.Bottom, box.Back));

            GL.PopAttrib();
        }

        private static void DrawQuad(Vector3 pt1, Vector3 pt2, Vector3 pt3, Vector3 pt4)
        {
            using (GLDraw.Begin(BeginMode.Lines))
            {
                GL.Vertex3(pt1); GL.Vertex3(pt2);
                GL.Vertex3(pt2); GL.Vertex3(pt3);
                GL.Vertex3(pt3); GL.Vertex3(pt4);
                GL.Vertex3(pt4); GL.Vertex3(pt1);
            }
        }

        public static void DrawCylinder(float radius, float length, Color color, bool capped)
        {
            const int resolution = 32;
            var cylAxis = Vector3.UnitY;
            var stepAngle = Angle.FromDegrees(360f / (float)resolution);
            var curAngle = Matrix4.Identity;
            var basePoint = new Vector3(0, 0, radius);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Color4(color);
            GL.Begin(BeginMode.Triangles);
            var top = cylAxis * (length * 0.5f);
            var bottom = cylAxis * (length * -0.5f);
            for (int i = 1; i <= resolution; i++)
            {
                var nextAngle = Matrix4.CreateFromAxisAngle(cylAxis, stepAngle.Radians * i);

                var pt1 = Vector3.Transform(basePoint, curAngle);
                var pt2 = Vector3.Transform(basePoint, nextAngle);
                var normal1 = pt1.Normalized();
                var normal2 = pt2.Normalized();

                //triangle 1
                GL.Normal3(normal1);
                GL.Vertex3(pt1 + bottom);
                GL.Vertex3(pt1 + top);
                GL.Normal3(normal2);
                GL.Vertex3(pt2 + top);

                //triangle 2
                GL.Normal3(normal1);
                GL.Vertex3(pt1 + bottom);
                GL.Normal3(normal2);
                GL.Vertex3(pt2 + top);
                GL.Vertex3(pt2 + bottom);

                if (capped)
                {
                    //cap 1
                    GL.Normal3(cylAxis);
                    GL.Vertex3(pt1 + top);
                    GL.Vertex3(top);
                    GL.Vertex3(pt2 + top);
                    //cap 2
                    GL.Normal3(cylAxis * -1f);
                    GL.Vertex3(bottom);
                    GL.Vertex3(pt1 + bottom);
                    GL.Vertex3(pt2 + bottom);
                }

                curAngle = nextAngle;
            }

            GL.End();
        }
        
        public static void DrawCone(float radius, float length, Color color)
        {
            const int resolution = 32;
            var cylAxis = Vector3.UnitY;
            var stepAngle = Angle.FromDegrees(360f / (float)resolution);
            var curAngle = Matrix4.Identity;
            var basePoint = new Vector3(0, 0, radius);

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Begin(BeginMode.Triangles);
            
            GL.Color4(color);

            for (int i = 1; i <= resolution; i++)
            {
                var nextAngle = Matrix4.CreateFromAxisAngle(cylAxis, stepAngle.Radians * i);

                var pt1 = cylAxis * length;
                var pt2 = Vector3.Transform(basePoint, curAngle);
                var pt3 = Vector3.Transform(basePoint, nextAngle);

                var u = pt2 - pt1;
                var v = pt3 - pt1;
                var normal = new Vector3((u.Y * v.Z) - (u.Z * v.Y), (u.Z * v.X) - (u.X * v.Z), (u.X * v.Y) - (u.Y * v.X));
                normal.Normalize();

                //side (cone)
                GL.Normal3(normal);
                GL.Vertex3(pt1);
                GL.Normal3(pt2.Normalized());
                GL.Vertex3(pt2);
                GL.Normal3(pt3.Normalized());
                GL.Vertex3(pt3);
                //bottom
                GL.Normal3(cylAxis * -1f);
                GL.Vertex3(pt2);
                GL.Vertex3(Vector3.Zero);
                GL.Vertex3(pt3);

                curAngle = nextAngle;
            }
            GL.End();
        }

        public static void FillPolygon(Color color, Vector3 normal, int sides, float radius)
        {
            FillPolygon(color, normal, 0f, sides, radius);
        }

        public static void FillPolygon(Color color, Vector3 normal, float distFromCenter, int sides, float radius)
        {
            FillPolygon(color, normal, distFromCenter, sides, radius, Angle.Zero);
        }

        public static void FillPolygon(Color color, Vector3 normal, float distFromCenter, int sides, float radius, Angle startAngle)
        {
            if (sides < 3)
                return;

            var stepAngle = Angle.FromDegrees(360f / (float)sides);
            var axisRot = Rotation.FromDirection(normal.Normalized());
            var up = Vector3.Transform(Vector3.UnitY, axisRot.Quaternion);
            var transformPoint = up * radius;
            var basePoint = normal * distFromCenter;
            
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Color4(color);
            GL.Begin(BeginMode.Triangles);

            var pt1 = basePoint + Vector3.Transform(transformPoint, Matrix4.CreateFromAxisAngle(normal, startAngle.Radians));
            var pt2 = basePoint + Vector3.Transform(transformPoint, Matrix4.CreateFromAxisAngle(normal, startAngle.Radians + stepAngle.Radians));
            var pt3 = basePoint + Vector3.Transform(transformPoint, Matrix4.CreateFromAxisAngle(normal, startAngle.Radians + stepAngle.Radians * 2));

            GL.Normal3(normal);
            GL.Vertex3(pt1);
            GL.Vertex3(pt2);
            GL.Vertex3(pt3);

            for (int i = 0; i < sides - 3; i++)
            {
                pt2 = pt3;
                pt3 = basePoint + Vector3.Transform(transformPoint, Matrix4.CreateFromAxisAngle(normal, startAngle.Radians + stepAngle.Radians * (3 + i)));
                GL.Normal3(normal);
                GL.Vertex3(pt1);
                GL.Vertex3(pt2);
                GL.Vertex3(pt3);
            }

            GL.End();
        }

        public static void DrawPolygon(Color color, Vector3 normal, int sides, float radius)
        {
            DrawPolygon(color, normal, 0f, sides, radius);
        }

        public static void DrawPolygon(Color color, Vector3 normal, float distFromCenter, int sides, float radius)
        {
            DrawPolygon(color, normal, distFromCenter, sides, radius, Angle.Zero);
        }

        public static void DrawPolygon(Color color, Vector3 normal, float distFromCenter, int sides, float radius, Angle startAngle)
        {
            if (sides < 3)
                return;

            var stepAngle = Angle.FromDegrees(360f / (float)sides);
            var axisRot = Rotation.FromDirection(normal.Normalized());
            var up = Vector3.Transform(Vector3.UnitY, axisRot.Quaternion);
            var transformPoint = up * radius;
            var basePoint = normal * distFromCenter;

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Color4(color);
            GL.Begin(BeginMode.Lines);

            var pt1 = basePoint + Vector3.Transform(transformPoint, Matrix4.CreateFromAxisAngle(normal, startAngle.Radians));

            for (int i = 1; i <= sides; i++)
            {
                var pt2 = basePoint + Vector3.Transform(transformPoint, Matrix4.CreateFromAxisAngle(normal, startAngle.Radians + stepAngle.Radians * i));

                GL.Vertex3(pt1);
                GL.Vertex3(pt2);

                pt1 = pt2;
            }

            GL.End();
        }

        public static void DrawDodecahedron(Color color, float edgeLength)
        {
            var outerRadius = (float)Math.Sqrt(50 + 10 * Math.Sqrt(5)) * edgeLength * 0.1f;
            var innerRadius = (float)Math.Sqrt(25 + 10 * Math.Sqrt(5)) * edgeLength * 0.1f;
            var faceDist = (outerRadius + 2 * innerRadius) / 2f;
            var axis1 = Vector3.Transform(Vector3.UnitY, Matrix4.CreateFromAxisAngle(Vector3.UnitX, Angle.ToRadians(180f - 116.57f)));
            var axis2 = Vector3.Transform(Vector3.UnitY, Matrix4.CreateFromAxisAngle(Vector3.UnitX, Angle.ToRadians(360f - 116.57f)));
            
            FillPolygon(color, Vector3.UnitY, faceDist, 5, outerRadius);
            //DrawPolygon(Color.Black, Vector3.UnitY, faceDist, 5, outerRadius);

            for (int i = 0; i < 5; i++)
            {
                var faceAxis = Vector3.Transform(axis1, Matrix4.CreateFromAxisAngle(Vector3.UnitY, Angle.ToRadians(72f) * i));
                faceAxis.Normalize();
                FillPolygon(color, faceAxis, faceDist, 5, outerRadius, 36f);
                DrawPolygon(Color.Black, faceAxis, faceDist, 5, outerRadius, 36f);
            }

            for (int i = 0; i < 5; i++)
            {
                var faceAxis = Vector3.Transform(axis2, Matrix4.CreateFromAxisAngle(Vector3.UnitY, Angle.ToRadians(72f) * i));
                faceAxis.Normalize();
                FillPolygon(color, faceAxis, faceDist, 5, outerRadius);
                DrawPolygon(Color.Black, faceAxis, faceDist, 5, outerRadius);
            }

            FillPolygon(color, Vector3.UnitY * -1f, faceDist, 5, outerRadius);
            //DrawPolygon(Color.Black, Vector3.UnitY * -1f, faceDist, 5, outerRadius);
        }

        public static void DrawMesh(Color color, Mesh mesh)
        {
            DrawMesh(color, mesh, Vector3.One);
        }

        public static void DrawMesh(Color color, Mesh mesh, Vector3 normalScale)
        {
            var triangles = mesh.Faces.OfType<FaceTriangle>();
            if (triangles.Any())
            {
                GL.Color4(color);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Begin(BeginMode.Triangles);
                foreach (var triangle in triangles)
                {
                    var textured = triangle.IsTextured;
                    foreach (var vert in triangle.Vertices)
                    {
                        GL.Normal3(Vector3.Multiply(vert.Normal, normalScale));
                        if (textured)
                            GL.TexCoord2(vert.UV.Value);
                        GL.Vertex3(vert.Position);
                    }
                }
                GL.End();
            }
        }
        
        public static void DrawWireMesh(Color color, Mesh mesh, float lineThickness = 1f)
        {
            var triangles = mesh.Faces.OfType<FaceTriangle>();

            GL.PushAttrib(AttribMask.LineBit);
            GL.LineWidth(lineThickness);

            if (triangles.Any())
            {
                GL.Color4(color);

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.Begin(BeginMode.Triangles);

                foreach (var triangle in triangles)
                {
                    foreach (var vert in triangle.Vertices)
                        GL.Vertex3(vert.Position + vert.Normal * (0.005f));
                }

                GL.End();
            }
            GL.PopAttrib();
        }

        public static void DrawLine(Color color, Vector3 pt1, Vector3 pt2, float lineThickness = 1f)
        {
            GL.PushAttrib(AttribMask.LineBit);
            GL.LineWidth(lineThickness);
            GL.Color4(color);
            using (GLDraw.Begin(BeginMode.Lines))
            {
                GL.Vertex3(pt1);
                GL.Vertex3(pt2);
            }
            GL.PopAttrib();
        }
    }
}
