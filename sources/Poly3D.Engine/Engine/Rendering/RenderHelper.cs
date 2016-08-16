using System;
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
        internal static readonly Color UNIT_X_COLOR = Color.Red;
        internal static readonly Color UNIT_Y_COLOR = Color.LawnGreen;
        internal static readonly Color UNIT_Z_COLOR = Color.FromArgb(0x00, 0x66, 0xFF);

        #region Shapes

        public static void DrawLine(Color color, Vector3 pt1, Vector3 pt2, float lineThickness = 1f)
        {
            GL.Color4(color);
            DrawLine(pt1, pt2, lineThickness);
        }

        public static void DrawLine(Vector3 pt1, Vector3 pt2, float lineThickness = 1f)
        {
            GL.PushAttrib(AttribMask.LineBit);
            GL.LineWidth(lineThickness);

            using (GLDraw.Begin(BeginMode.Lines))
            {
                GL.Vertex3(pt1);
                GL.Vertex3(pt2);
            }

            GL.PopAttrib();
        }

        public static void DrawPolygon(Vector3 normal, float distFromCenter, int sides, float radius, Angle startAngle)
        {
            if (sides < 3)
                return;

            var stepAngle = GLMath.PI * 2f / (float)sides;
            var axisRot = Rotation.FromDirection(normal.Normalized());
            var up = Vector3.Transform(Vector3.UnitY, axisRot.Quaternion);
            var transformPoint = up * radius;
            var basePoint = normal * distFromCenter;

            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.Begin(BeginMode.TriangleFan);

            GL.Normal3(normal);
            for (int i = 0; i <= sides; i++)
            {
                var pt1 = basePoint + Vector3.Transform(transformPoint, Matrix4.CreateFromAxisAngle(normal, startAngle.Radians + stepAngle * i));
                //var pt1 = basePoint + GetCirclePoint(Angle.FromRadians(startAngle.Radians + stepAngle * i), radius, normal);
                GL.Vertex3(pt1);
            }

            GL.End();
        }

        public static void DrawCircle(float radius, float lineThickness = 1f)//stand on the Z axis
        {
            float stepAngle = (float)Math.PI * 2f / (float)CURVE_RES;

            for (int i = 0; i < CURVE_RES; i++)
            {
                var p1 = new Vector3((float)Math.Cos(stepAngle * i), (float)Math.Sin(stepAngle * i), 0f) * radius;
                var p2 = new Vector3((float)Math.Cos(stepAngle * (i + 1)), (float)Math.Sin(stepAngle * (i + 1)), 0f) * radius;
                DrawLine(p1, p2, lineThickness);
            }
        }

        public static void DrawCircle(Vector3 normal, float radius, float lineThickness = 1f)//stand on the Z axis
        {
            float stepAngle = (float)360f / (float)CURVE_RES;

            for (int i = 0; i < CURVE_RES; i++)
            {
                var p1 = GetCirclePoint(stepAngle * i, radius, normal);
                var p2 = GetCirclePoint(stepAngle * (i+1), radius, normal);
                DrawLine(p1, p2, lineThickness);
            }
        }

        private static void OutlineQuad(Vector3 pt1, Vector3 pt2, Vector3 pt3, Vector3 pt4)
        {
            using (GLDraw.Begin(BeginMode.Lines))
            {
                GL.Vertex3(pt1); GL.Vertex3(pt2);
                GL.Vertex3(pt2); GL.Vertex3(pt3);
                GL.Vertex3(pt3); GL.Vertex3(pt4);
                GL.Vertex3(pt4); GL.Vertex3(pt1);
            }
        }

        private static void FillQuad(Vector3 pt1, Vector3 pt2, Vector3 pt3, Vector3 pt4)
        {
            using (GLDraw.Begin(BeginMode.Triangles))
            {
                GL.Vertex3(pt1); GL.Vertex3(pt2); GL.Vertex3(pt3);
                GL.Vertex3(pt1); GL.Vertex3(pt3); GL.Vertex3(pt4);
            }
        }

        #endregion

        #region Primitives

        readonly static int CURVE_RES = 30;

        public static void DrawCube(Vector3 size)
        {
            DrawCube(new BoundingBox(Vector3.Zero, size / 2f));
        }

        public static void DrawCube(BoundingBox box)
        {
            //front
            GL.Normal3(Vector3.UnitZ);
            FillQuad(new Vector3(box.Right, box.Top, box.Front),
                new Vector3(box.Left, box.Top, box.Front),
                new Vector3(box.Left, box.Bottom, box.Front),
                new Vector3(box.Right, box.Bottom, box.Front));
            //back
            GL.Normal3(Vector3.UnitZ * -1f);
            FillQuad(new Vector3(box.Left, box.Top, box.Back),
                new Vector3(box.Right, box.Top, box.Back),
                new Vector3(box.Right, box.Bottom, box.Back),
                new Vector3(box.Left, box.Bottom, box.Back));
            //right
            GL.Normal3(Vector3.UnitX);
            FillQuad(new Vector3(box.Right, box.Top, box.Back),
                new Vector3(box.Right, box.Top, box.Front),
                new Vector3(box.Right, box.Bottom, box.Front),
                new Vector3(box.Right, box.Bottom, box.Back));
            //left
            GL.Normal3(Vector3.UnitX * -1f);
            FillQuad(new Vector3(box.Left, box.Top, box.Front),
                new Vector3(box.Left, box.Top, box.Back),
                new Vector3(box.Left, box.Bottom, box.Back),
                new Vector3(box.Left, box.Bottom, box.Front));
            //top
            GL.Normal3(Vector3.UnitY);
            FillQuad(new Vector3(box.Right, box.Top, box.Back),
                new Vector3(box.Left, box.Top, box.Back),
                new Vector3(box.Left, box.Top, box.Front),
                new Vector3(box.Right, box.Top, box.Front));
            //bottom
            GL.Normal3(Vector3.UnitY * -1f);
            FillQuad(new Vector3(box.Right, box.Bottom, box.Front),
                new Vector3(box.Left, box.Bottom, box.Front),
                new Vector3(box.Left, box.Bottom, box.Back),
                new Vector3(box.Right, box.Bottom, box.Back));
        }

        public static void OutlineCube(Vector3 size)
        {
            OutlineCube(new BoundingBox(Vector3.Zero, size / 2f));
        }

        public static void OutlineCube(Color color, Vector3 size)
        {
            GL.Color4(color);
            OutlineCube(size);
        }

        public static void OutlineCube(Color color, BoundingBox box)
        {
            GL.Color4(color);
            OutlineCube(box);
        }

        public static void OutlineCube(BoundingBox box)
        {
            //front
            GL.Normal3(Vector3.UnitZ);
            OutlineQuad(new Vector3(box.Right, box.Top, box.Front),
                new Vector3(box.Left, box.Top, box.Front),
                new Vector3(box.Left, box.Bottom, box.Front),
                new Vector3(box.Right, box.Bottom, box.Front));
            //back
            GL.Normal3(Vector3.UnitZ * -1f);
            OutlineQuad(new Vector3(box.Left, box.Top, box.Back),
                new Vector3(box.Right, box.Top, box.Back),
                new Vector3(box.Right, box.Bottom, box.Back),
                new Vector3(box.Left, box.Bottom, box.Back));
            //right
            GL.Normal3(Vector3.UnitX);
            OutlineQuad(new Vector3(box.Right, box.Top, box.Back),
                new Vector3(box.Right, box.Top, box.Front),
                new Vector3(box.Right, box.Bottom, box.Front),
                new Vector3(box.Right, box.Bottom, box.Back));
            //left
            GL.Normal3(Vector3.UnitX * -1f);
            OutlineQuad(new Vector3(box.Left, box.Top, box.Front),
                new Vector3(box.Left, box.Top, box.Back),
                new Vector3(box.Left, box.Bottom, box.Back),
                new Vector3(box.Left, box.Bottom, box.Front));
            //top
            GL.Normal3(Vector3.UnitY);
            OutlineQuad(new Vector3(box.Right, box.Top, box.Back),
                new Vector3(box.Left, box.Top, box.Back),
                new Vector3(box.Left, box.Top, box.Front),
                new Vector3(box.Right, box.Top, box.Front));
            //bottom
            GL.Normal3(Vector3.UnitY * -1f);
            OutlineQuad(new Vector3(box.Right, box.Bottom, box.Front),
                new Vector3(box.Left, box.Bottom, box.Front),
                new Vector3(box.Left, box.Bottom, box.Back),
                new Vector3(box.Right, box.Bottom, box.Back));
        }

        public static void DrawCone(float radius, float height)
        {
            float stepAngle = (float)Math.PI * 2f / (float)CURVE_RES;
            float normalAngle = GLMath.PI /2f - (float)Math.Atan(height / radius);

            var nY = (float)Math.Sin(normalAngle);
            var nXZ = (float)Math.Cos(normalAngle);

            GL.Begin(BeginMode.Triangles);
            var hSize = height / 2f;
            var top = Vector3.UnitY * hSize;
            var bottom = Vector3.UnitY * -hSize;

            for (int i = 0; i < CURVE_RES; i++)
            {
                var p1 = GetCirclePoint(Angle.FromRadians(stepAngle * i), radius, Vector3.UnitY);
                var p2 = GetCirclePoint(Angle.FromRadians(stepAngle * (i + 1)), radius, Vector3.UnitY);
                
                GL.Normal3(Vector3.UnitY * -1f);
                GL.Vertex3(p1 + bottom);
                GL.Vertex3(p2 + bottom);
                GL.Vertex3(bottom);

                var norm1 = p1.Normalized() * nXZ + new Vector3(0, nY, 0);
                var norm2 = p2.Normalized() * nXZ + new Vector3(0, nY, 0);
                var norm3 = Vector3.Lerp(p1,p2,0.5f).Normalized() * nXZ + new Vector3(0, nY, 0);

                GL.Normal3(norm1);
                GL.Vertex3(p1 + bottom);
                GL.Normal3(norm3);
                GL.Vertex3(top);
                GL.Normal3(norm2);
                GL.Vertex3(p2 + bottom);
            }

            GL.End();
        }

        public static void DrawCylinder(float radius, float height, bool capped = true)
        {
            float stepAngle = (float)Math.PI * 2f / (float)CURVE_RES;
            
            var hSize = height / 2f;
            var top = Vector3.UnitY * hSize;
            var bottom = Vector3.UnitY * -hSize;

            GL.Begin(BeginMode.Triangles);

            for (int i = 0; i < CURVE_RES; i++)
            {
                var p1 = GetCirclePoint(Angle.FromRadians(stepAngle * i), radius, Vector3.UnitY);
                var p2 = GetCirclePoint(Angle.FromRadians(stepAngle * (i + 1)), radius, Vector3.UnitY);
                var norm1 = p1.Normalized();
                var norm2 = p2.Normalized();

                GL.Normal3(norm2);
                GL.Vertex3(p2 + bottom);
                GL.Normal3(norm1);
                GL.Vertex3(p1 + top);
                GL.Vertex3(p1 + bottom);

                GL.Normal3(norm2);
                GL.Vertex3(p2 + bottom);
                GL.Vertex3(p2 + top);
                GL.Normal3(norm1);
                GL.Vertex3(p1 + top);

                if (capped)
                {
                    GL.Normal3(Vector3.UnitY * 1f);
                    GL.Vertex3(top);
                    GL.Vertex3(p1 + top);
                    GL.Vertex3(p2 + top);

                    GL.Normal3(Vector3.UnitY * -1f);
                    GL.Vertex3(bottom);
                    GL.Vertex3(p2 + bottom);
                    GL.Vertex3(p1 + bottom);
                }
            }
            
            GL.End();
        }

        public static void DrawDodecahedron(float edgeLength)
        {
            var outerRadius = (float)Math.Sqrt(50 + 10 * Math.Sqrt(5)) * edgeLength * 0.1f;
            var innerRadius = (float)Math.Sqrt(25 + 10 * Math.Sqrt(5)) * edgeLength * 0.1f;

            var faceDist = (outerRadius + 2 * innerRadius) / 2f;

            var axis1 = Vector3.Transform(Vector3.UnitY, Matrix4.CreateFromAxisAngle(Vector3.UnitX, Angle.ToRadians(63.4395f)));
            var axis2 = Vector3.Transform(Vector3.UnitY, Matrix4.CreateFromAxisAngle(Vector3.UnitX, Angle.ToRadians(243.4395f)));


            DrawPolygon(Vector3.UnitY, faceDist, 5, outerRadius, 0f);

            for (int i = 0; i < 5; i++)
            {
                var faceAxis = Vector3.Transform(axis1, Matrix4.CreateFromAxisAngle(Vector3.UnitY, Angle.ToRadians(72f) * i));
                faceAxis.Normalize();
                DrawPolygon(faceAxis, faceDist, 5, outerRadius, 36f);
            }

            for (int i = 0; i < 5; i++)
            {
                var faceAxis = Vector3.Transform(axis2, Matrix4.CreateFromAxisAngle(Vector3.UnitY, Angle.ToRadians(72f) * i));
                faceAxis.Normalize();
                DrawPolygon(faceAxis, faceDist, 5, outerRadius, 0f);
            }

            DrawPolygon(Vector3.UnitY * -1f, faceDist, 5, outerRadius, 0f);
        }

        #endregion

        #region Mesh/Models

        public static void DrawMesh(Color color, Mesh mesh)
        {
            DrawMesh(color, mesh, Vector3.One);
        }

        public static void DrawMesh(Color color, Mesh mesh, Vector3 normalScale)
        {
            var triangles = mesh.Faces.OfType<FaceTriangle>();
            var quads = mesh.Faces.OfType<FaceQuad>();
            if (triangles.Any() || quads.Any())
            {
                GL.Color4(color);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                GL.Begin(BeginMode.Triangles);

                foreach (var triangle in triangles)
                {
                    var textured = triangle.IsTextured;
                    foreach (var vert in triangle.Vertices)
                    {
                        GLVertex(vert, normalScale);
                    }
                }

                foreach (var face in quads)
                {
                    GLVertex(face.Vertex0, normalScale); GLVertex(face.Vertex1, normalScale); GLVertex(face.Vertex2, normalScale);
                    GLVertex(face.Vertex0, normalScale); GLVertex(face.Vertex2, normalScale); GLVertex(face.Vertex3, normalScale);
                }

                GL.End();
            }
        }

        private static void GLVertex(Vertex vert, Vector3 normalScale)
        {
            GL.Normal3(Vector3.Multiply(vert.Normal, normalScale));
            if (vert.IsTextured)
                GL.TexCoord2(vert.UV.Value);
            GL.Vertex3(vert.Position);
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

        #endregion

        #region Manipulators

        public static void RenderManipulator(Camera camera, Vector3 position, TransformType manipulatorType)
        {
            GL.PushAttrib(AttribMask.LightingBit | AttribMask.DepthBufferBit);
            //GL.Clear(ClearBufferMask.DepthBufferBit);

            var distFromCam = camera.GetDistanceFromCamera(position);
            var viewSize = camera.GetViewSize(distFromCam);
            
            var maniScale = Manipulator.SCREEN_SIZE * viewSize.Y / camera.DisplayRectangle.Height;

            DrawManipulatorAxis(Vector3.UnitX, UNIT_X_COLOR, manipulatorType, maniScale, false);

            DrawManipulatorAxis(Vector3.UnitY, UNIT_Y_COLOR, manipulatorType, maniScale, false);

            DrawManipulatorAxis(Vector3.UnitZ, UNIT_Z_COLOR, manipulatorType, maniScale, false);

            GL.PopAttrib();
        }

        private static void DrawManipulatorAxis(Vector3 axis, Color color, TransformType manipulatorType, float length, bool selected)
        {
            GL.PushMatrix();
            GL.Disable(EnableCap.Lighting);
            if (manipulatorType == TransformType.Rotation)
            {
                var rotMat = GLMath.RotationFromTo(Vector3.UnitZ, axis);
                GL.MultMatrix(ref rotMat);
                GL.Color4(color);
                DrawCircle(length, selected ? 3 : 1.5f);
            }
            else
            {
                var endSize = length / 8f;
                var axisLength = length - endSize;

                DrawLine(color, Vector3.Zero, axis * axisLength, selected ? 2.5f : 1.5f);

                //GL.Enable(EnableCap.Lighting);
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

                if (manipulatorType == TransformType.Translation)
                {
                    var rotMat = GLMath.RotationFromTo(Vector3.UnitY, axis);
                    GL.Translate(axis * (axisLength + endSize / 2f));
                    GL.MultMatrix(ref rotMat);
                    endSize *= selected ? 2f : 1.7f;
                    DrawCone(endSize / 3f, endSize);
                }
                else
                {
                    GL.Translate(axis * (axisLength + endSize / 2f));
                    endSize *= selected ? 1.25f : 1f;
                    DrawCube(Vector3.One * endSize);
                }
            }

            GL.PopMatrix();
        }

        #endregion

        private static Vector3 GetCirclePoint(Angle angle, float radius, Vector3 axis)
        {
            var point = new Vector3((float)Math.Cos(angle.Radians), 0f, (float)Math.Sin(angle.Radians)) * radius;
            if (axis == Vector3.UnitY)
                return point;
            //else if (axis == Vector3.UnitZ)
            //    return point.Xzy;
            // else if (axis == Vector3.UnitX)
            //    return point.Yxz;
            var rotMat = GLMath.RotationFromTo(Vector3.UnitY, axis);
            return Vector3.Transform(point, rotMat);
        }
    }
}
