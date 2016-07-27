using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Poly3D.Maths;
using Poly3D.Graphics;
using System.Diagnostics;

namespace Poly3D.Engine
{
	public class Camera : SceneObject
    {
        // Fields...
        private Color _BackColor;
        private Rect _ViewRectangle;
        private float _OrthographicSize;
        private Matrix4 _ProjectionMatrix;
        private float _FarClipDistance;
        private float _NearClipDistance;
        private ProjectionType _Projection;
        private Angle _FieldOfView;
        private bool isMatrixDirty;
        private bool isViewportDirty;

        /// <summary>
        /// Gets or sets the vertical field of view of the camera; the Horizontal FOV varies depending on the viewport's aspect ratio.
        /// </summary>
        /// <remarks>Field of view is ignored when camera projection is orthographic.</remarks>
        public Angle FieldOfView
        {
            get { return _FieldOfView; }
            set
            {
                if (_FieldOfView == value)
                    return;
                _FieldOfView = value;
                if(Projection == ProjectionType.Perspective)
                    isMatrixDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets a rectangle that indicate where on the screen this camera view will be drawn. Measured in Screen Coordinates (values 0–1).
        /// </summary>
        public Rect ViewRectangle
        {
            get { return _ViewRectangle; }
            set
            {
                if (_ViewRectangle == value)
                    return;
                //if (_ViewRectangle.Left < 0 || _ViewRectangle.Right > 1 || _ViewRectangle.Top < 0 || _ViewRectangle.Bottom > 1)
                //    throw new Exception("");
                _ViewRectangle = value;

                isMatrixDirty = true;
                isViewportDirty = true;
            }
        }

        /// <summary>
        /// Get the rectangle that indicate wehere on the screen the camera is rendered in pixel coordinates.
        /// </summary>
        public Rect DisplayRectangle
        {
            get
            {
                if(Scene == null || Scene.Viewport == null)
                    return new Rect(0, 0, 1, 1);
                
                var displaySize = new Vector2(Scene.Viewport.Width, Scene.Viewport.Height);
                return new Rect(
                    displaySize.X * ViewRectangle.X, 
                    displaySize.Y * ViewRectangle.Y, 
                    displaySize.X * ViewRectangle.Width, 
                    displaySize.Y * ViewRectangle.Height);
            }
        }

        public float AspectRatio
        {
            get
            {
                var dispRect = DisplayRectangle;
                return dispRect.Width / dispRect.Height;
            }
        }

        /// <summary>
        /// Gets or sets the camera's projection type.
        /// </summary>
        public ProjectionType Projection
        {
            get { return _Projection; }
            set
            {
                if (_Projection == value)
                    return;
                _Projection = value;
                isMatrixDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the near clipping plane distance.
        /// </summary>
        public float NearClipDistance
        {
            get { return _NearClipDistance; }
            set
            {
                value = Math.Max(0.0001f, Math.Min(FarClipDistance, value));
                if (_NearClipDistance == value)
                    return;
                _NearClipDistance = value;
                isMatrixDirty = true;
            }
        }

        /// <summary>
        /// Gets or sets the far clipping plane distance.
        /// </summary>
        public float FarClipDistance
        {
            get { return _FarClipDistance; }
            set
            {
                value = Math.Max(NearClipDistance, value);
                if (_FarClipDistance == value)
                    return;
                _FarClipDistance = value;
                isMatrixDirty = true;
            }
        }

        public Matrix4 ProjectionMatrix
        {
            get
            {
                if (isMatrixDirty)
                    ComputeProjectionMatrix();
                return _ProjectionMatrix;
            }
        }

        public float OrthographicSize
        {
            get { return _OrthographicSize; }
            set
            {
                if (value < 0.01f || _OrthographicSize == value)
                    return;
                _OrthographicSize = value;
                if (Projection == ProjectionType.Orthographic)
                    isMatrixDirty = true;
            }
        }

        public Color BackColor
        {
            get { return _BackColor; }
            set
            {
                _BackColor = value;
            }
        }

        public Camera()
        {
            _FieldOfView = Angle.FromDegrees(60);
            _NearClipDistance = 0.3f;
            _FarClipDistance = 1000f;
            _OrthographicSize = 10f;
            _Projection = ProjectionType.Perspective;
            _BackColor = Color.FromArgb(0.5f, 0.5f, 0.5f);
            _ViewRectangle = new Rect(0, 0, 1, 1);
            isMatrixDirty = true;
            isViewportDirty = true;
        }

        protected void ComputeProjectionMatrix()
        {
            if (ViewRectangle.Min == Vector2.Zero && ViewRectangle.Max == Vector2.One)
            {
                if (Projection == ProjectionType.Perspective)
                {
                    _ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FieldOfView.Radians, AspectRatio, NearClipDistance, FarClipDistance);
                }
                else
                {
                    _ProjectionMatrix = Matrix4.CreateOrthographic(OrthographicSize * AspectRatio, OrthographicSize, NearClipDistance, FarClipDistance);
                }
            }
            else//offCenter
            {
                if (Projection == ProjectionType.Perspective)
                {
                    //TODO
                    _ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FieldOfView.Radians, AspectRatio, NearClipDistance, FarClipDistance);
                }
                else
                {
                    //TODO
                    _ProjectionMatrix = Matrix4.CreateOrthographic(OrthographicSize * AspectRatio, OrthographicSize, NearClipDistance, FarClipDistance);
                }
            }

            isMatrixDirty = false;
        }

        private Vector2 lastViewPort = Vector2.Zero;

        private void UpdateViewport()
        {
            var viewRect = DisplayRectangle;
            GL.Viewport((int)viewRect.X, (int)viewRect.Y, (int)viewRect.Width, (int)viewRect.Height);
            isViewportDirty = false;
            ComputeProjectionMatrix();
        }

        public Matrix4 GetModelviewMatrix()
        {
            return Matrix4.LookAt(Transform.WorldPosition, Transform.Forward * 4, Transform.Up);
        }

        internal void Render()
        {
            var curViewport = new Vector2(Scene.Viewport.Width, Scene.Viewport.Height);

            if (curViewport != lastViewPort)
            {
                isViewportDirty = true;
                lastViewPort = curViewport;
                UpdateViewport();
            }

            GL.ClearColor(BackColor);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            GL.Enable(EnableCap.DepthTest);

            GL.MatrixMode(MatrixMode.Projection);
            
            if (isMatrixDirty)
                ComputeProjectionMatrix();

            GL.LoadMatrix(ref _ProjectionMatrix);
            
            GL.MatrixMode(MatrixMode.Modelview);

            var viewMatrix = GetModelviewMatrix();

            GL.LoadMatrix(ref viewMatrix);
            DrawAxeCoord();

            //foreach (var so in Scene.Objects)
            //{
            //    //restore view matrix
            //    //apply object transform
            //    //render object
            //}

            //GL.Rotate(0.5f, Vector3.UnitY);
            //GL.Translate(0, 0, 3);
            //GL.Translate(0, 0, 3);

            //GL.Translate(0, -1f, 0);
            GL.Scale(4f, 4f, 4f);
            GL.Color4(Color.White);
            GL.Begin(BeginMode.Triangles);
            GL.Vertex3(-1f, 0f, 1f);
            GL.Vertex3(1f, 0f, 1f);
            GL.Vertex3(1f, 0f, -1f);

            GL.Vertex3(-1f, 0f, 1f);
            GL.Vertex3(1f, 0f, -1f);
            GL.Vertex3(-1f, 0f, -1f);
            GL.End();

            GL.LoadMatrix(ref viewMatrix);
            DrawPyramid();

            GL.LoadMatrix(ref viewMatrix);
            GL.Translate(2f, 0, 0);
            GL.Rotate(45f, Vector3.UnitY);
            DrawPyramid();

            GL.LoadMatrix(ref viewMatrix);
            GL.Translate(-2f, 0, 0);
            GL.Rotate(-45f, Vector3.UnitY);
            DrawPyramid();
            
        }

        public Ray RaycastFromScreen(Vector2 point)
        {
            var viewRect = DisplayRectangle;
            if (!viewRect.Contains(point))
                return null;

            // Normalize screen poin
            var normalizedPoint = new Vector2((point.X - viewRect.X) / viewRect.Width, (point.Y - viewRect.Y) / viewRect.Height);

            return Raycast(normalizedPoint);
        }

        public Ray Raycast(Vector2 point)
        {
            if (point.X < 0f || point.X > 1f || point.Y < 0f || point.Y > 1f)
                return null;

            var transformMatrix = Matrix4.Mult(GetModelviewMatrix(), ProjectionMatrix).Inverted();

            // Transformation of normalized coordinates (from [0.0, 1.0] to [-1.0, 1.0]).
            var startVector = new Vector4(
                point.X * 2f - 1f,//-1 = left, so 0->1 = -1->+1
                1f - point.Y * 2f,//-1 = bottom, so 0->1 = +1->-1
                -1f,//near
                1f);
            
            var endVector = new Vector4(
                startVector.X,
                startVector.Y,
                1f,//far
                1f);

            var origin = Vector4.Transform(startVector, transformMatrix);
            if (origin.W == 0)
                return null;

            origin.X /= origin.W;
            origin.Y /= origin.W;
            origin.Z /= origin.W;

            var end = Vector4.Transform(endVector, transformMatrix);

            if (end.W == 0)
                return null;

            end.X /= end.W;
            end.Y /= end.W;
            end.Z /= end.W;

            return Ray.FromPoints(origin.Xyz, end.Xyz);
        }

        private void DrawPyramid()
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Begin(BeginMode.Triangles);
            GL.Color4(Color.Blue);
            GL.Vertex3(0f, -0.5f, -1f);
            GL.Vertex3(0.866f, -0.5f, 0.5f);
            GL.Vertex3(0f, 1f, 0f);
            
            GL.Color4(Color.Red);
            GL.Vertex3(0.866f, -0.5f, 0.5f);
            GL.Vertex3(-0.866f, -0.5f, 0.5f);
            GL.Vertex3(0f, 1f, 0f);

            GL.Color4(Color.Yellow);
            GL.Vertex3(-0.866f, -0.5f, 0.5f);
            GL.Vertex3(0f, -0.5f, -1f);
            GL.Vertex3(0f, 1f, 0f);

            GL.End();
        }

        private void DrawAxeCoord()
        {
            GL.LineWidth(3);

            GL.Begin(BeginMode.Lines);

            GL.Color4(Color.Red);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitX * 2);

            GL.Color4(Color.LimeGreen);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitY * 2);

            GL.Color4(Color.Blue);
            GL.Vertex3(Vector3.Zero);
            GL.Vertex3(Vector3.UnitZ * 2);

            GL.End();
        }
    }
}
