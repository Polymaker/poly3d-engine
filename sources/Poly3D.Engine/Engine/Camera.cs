using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Poly3D.Maths;
using Poly3D.Graphics;
using OpenTK.Graphics;

namespace Poly3D.Engine
{
	public class Camera : SceneObject
    {
        // Fields...
        private Color _BackColor;
        private bool _Active;
        private Rect _ViewRectangle;
        private float _OrthographicSize;
        private Matrix4 _ProjectionMatrix;
        private float _FarClipDistance;
        private float _NearClipDistance;
        private ProjectionType _Projection;
        private Angle _FieldOfView;
        private bool isPMatrixDirty = false;

        /// <summary>
        /// Gets or sets the vertical field of view of the camera; the Horizontal FOV varies depending on the viewport's aspect ratio.
        /// </summary>
        /// <remarks>Field of view is ignored when camera projection is orthographic.</remarks>
        public Angle FieldOfView
        {
            get { return _FieldOfView; }
            set
            {
                _FieldOfView = value;
                if(Projection == ProjectionType.Perspective)
                    isPMatrixDirty = true;
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

                isPMatrixDirty = true;
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

        public ProjectionType Projection
        {
            get { return _Projection; }
            set
            {
                if (_Projection == value)
                    return;
                _Projection = value;
                isPMatrixDirty = true;
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
                isPMatrixDirty = true;
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
                isPMatrixDirty = true;
            }
        }

        public Matrix4 ProjectionMatrix
        {
            get
            {
                if (isPMatrixDirty)
                    ComputeProjectionMatrix();
                return _ProjectionMatrix;
            }
        }

        public float OrthographicSize
        {
            get { return _OrthographicSize; }
            set
            {
                value = Math.Abs(value);
                if (_OrthographicSize == value)
                    return;
                _OrthographicSize = value;
                if (Projection == ProjectionType.Orthographic)
                    isPMatrixDirty = true;
            }
        }

        public bool Active
        {
            get { return _Active; }
            set
            {
                _Active = value;
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
            isPMatrixDirty = true;
        }

        protected void ComputeProjectionMatrix()
        {
            if (ViewRectangle.Min == Vector2.Zero && ViewRectangle.Max == Vector2.One)
            {
                if (Projection == ProjectionType.Perspective)
                {
                    _ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)FieldOfView.Radians, AspectRatio, NearClipDistance, FarClipDistance);
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
                    _ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView((float)FieldOfView.Radians, AspectRatio, NearClipDistance, FarClipDistance);
                }
                else
                {
                    //TODO
                    _ProjectionMatrix = Matrix4.CreateOrthographic(OrthographicSize * AspectRatio, OrthographicSize, NearClipDistance, FarClipDistance);
                }
            }
            //lastViewPort = new Vector2(Scene.Viewport.Width, Scene.Viewport.Height);
            isPMatrixDirty = false;
        }

        private Vector2 lastViewPort = Vector2.Zero;

        internal void Render()
        {
            var curViewport = new Vector2(Scene.Viewport.Width, Scene.Viewport.Height);
            if (curViewport != lastViewPort)
            {

                var viewRect = DisplayRectangle;
                GL.Viewport((int)viewRect.X, (int)viewRect.Y, (int)viewRect.Width, (int)viewRect.Height);
                lastViewPort = curViewport;
                isPMatrixDirty = true;
            }

            GL.ClearColor((Color4)BackColor);
            GL.Clear(ClearBufferMask.DepthBufferBit | ClearBufferMask.ColorBufferBit);

            GL.Enable(EnableCap.DepthTest);

            GL.MatrixMode(MatrixMode.Projection);
            
            if (isPMatrixDirty)
                ComputeProjectionMatrix();

            GL.LoadMatrix(ref _ProjectionMatrix);
            
            GL.MatrixMode(MatrixMode.Modelview);

            var viewMatrix = Transform.GetFinalMatrix();

            viewMatrix.Invert();
            GL.LoadMatrix(ref viewMatrix);

            //GL.Rotate(0.5f, Vector3.UnitY);
            GL.Translate(0, 0, -2);
            DrawPyramid();
            GL.LoadMatrix(ref viewMatrix);
            GL.Translate(2f, 0, 0);
            GL.Rotate(45f, Vector3.UnitY);
            DrawPyramid();

            GL.LoadMatrix(ref viewMatrix);

            GL.Translate(-2f, 0, 0);
            GL.Rotate(-45f, Vector3.UnitY);
            DrawPyramid();
            //GL.Color3(Color.Black);
            //GL.Begin(BeginMode.Lines);
            //GL.Vertex3(Transform.WorldPosition);
            //GL.Vertex3(Transform.WorldPosition + Transform.Right * 4);
            //GL.End();
        }

        private void DrawPyramid()
        {
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            GL.Begin(BeginMode.Triangles);
            GL.Color3(Color.Blue);
            GL.Vertex3(0f, -0.5f, -1f);
            GL.Vertex3(0.866f, -0.5f, 0.5f);
            GL.Vertex3(0f, 1f, 0f);

            GL.Color3(Color.Red);
            GL.Vertex3(0.866f, -0.5f, 0.5f);
            GL.Vertex3(-0.866f, -0.5f, 0.5f);
            GL.Vertex3(0f, 1f, 0f);

            GL.Color3(Color.Yellow);
            GL.Vertex3(-0.866f, -0.5f, 0.5f);
            GL.Vertex3(0f, -0.5f, -1f);
            GL.Vertex3(0f, 1f, 0f);

            GL.End();
        }
    }
}
