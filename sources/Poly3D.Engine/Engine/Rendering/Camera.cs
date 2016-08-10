using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Poly3D.Maths;
using Poly3D.Graphics;
using System.Diagnostics;
using System.Collections;
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
                if(Scene == null || Scene.Display == null)
                    return new Rect(0, 0, 1, 1);

                //var displaySize = new Vector2(Scene.Display.Width, Scene.Display.Height);
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
                if (_BackColor == value)
                    return;
                _BackColor = value;
            }
        }

        public int RenderPriority { get; set; }

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

        #region Matrices calculation

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

        public Matrix4 GetModelviewMatrix()
        {
            //saves us the trouble of inverting the chain of transformations that the camera has
            return Matrix4.LookAt(Transform.WorldPosition, Transform.WorldPosition + Transform.Forward * 4, Transform.Up);
        }

        #endregion

        #region Viewport handling

        private Vector2 lastViewPort = Vector2.Zero;

        internal void UpdateViewport()
        {
            var curViewport = new Vector2(Scene.Viewport.Width, Scene.Viewport.Height);
            //if the GL render context size changed or the view rectangle of the camera changed (isViewportDirty)
            if (curViewport != lastViewPort || isViewportDirty)
            {
                var viewRect = DisplayRectangle;
                GL.Viewport((int)viewRect.X, (int)viewRect.Y, (int)viewRect.Width, (int)viewRect.Height);
                isViewportDirty = false;

                ComputeProjectionMatrix();

                lastViewPort = curViewport;

                //GL.ClearColor(BackColor);
            }
        }

        #endregion
        
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

        public Vector2 PointToScreen(Vector3 point)
        {
            var transformMatrix = Matrix4.Mult(GetModelviewMatrix(), ProjectionMatrix);
            var result = Vector4.Transform(new Vector4(point, 1), transformMatrix);

            var dispRect = DisplayRectangle;
            result.X = ((result.X / result.W) + 1f) / 2f;
            result.Y = (1f - (result.Y / result.W)) / 2f;
            result.X = dispRect.X + result.X * dispRect.Width;
            result.Y = dispRect.Y + result.Y * dispRect.Height;
            return result.Xy;
        }

        public SceneObject RaySelect(Ray ray)
        {
            var hits = new List<Tuple<float, SceneObject>>();
            foreach (var meshObj in Scene.EngineObjects.Where(o => o.IsActive).OfType<ObjectMesh>())
            {
                var worldToLocal = meshObj.Transform.WorldToLocalMatrix;
                var localRay = Ray.Transform(ray, worldToLocal);
                float dist;
                if (meshObj.Mesh.BoundingBox.Intersects(localRay, out dist))
                {
                    var localPt = localRay.GetPoint(dist);
                    var worldPt = Vector3.Transform(localPt, meshObj.Transform.GetTransformMatrix());
                    var worldDist = (worldPt - ray.Origin).Length;
                    hits.Add(new Tuple<float, SceneObject>(worldDist, meshObj));
                }
            }
            
            if (hits.Count > 0)
                return hits.OrderBy(h => h.Item1).First().Item2;

            return null;
        }

        /// <summary>
        /// Gets the visible vertical height of the world at the distance specified. 
        /// </summary>
        /// <param name="distFromCamera"></param>
        /// <returns></returns>
        public float GetViewHeight(float distFromCamera)
        {
            if(Projection == ProjectionType.Perspective)
                return (float)Math.Tan(FieldOfView.Radians / 2f) * distFromCamera * 2f;

            return OrthographicSize;
        }

        /// <summary>
        /// Gets the visible horizontal width of the world at the distance specified. 
        /// </summary>
        /// <param name="distFromCamera"></param>
        /// <returns></returns>
        public float GetViewWidth(float distFromCamera)
        {
            return GetViewHeight(distFromCamera) * AspectRatio;
        }

        public Vector2 GetViewSize(float distFromCamera)
        {
            var vh = GetViewHeight(distFromCamera);
            return new Vector2(vh * AspectRatio, vh);
        }

        public float GetDistanceFromCamera(SceneObject so)
        {
            return GetDistanceFromCamera(so.Transform.WorldPosition);
        }

        public float GetDistanceFromCamera(Vector3 worldPos)
        {
            var posOffset = worldPos - Transform.WorldPosition;

            var angleFromFwrd = Vector3.CalculateAngle(posOffset.Normalized(), Transform.Forward);

            if (float.IsNaN(angleFromFwrd))
                return posOffset.Length;

            //distance from camera is equal to adjacent side on the triangle formed by camera, specified pos and a point along camera foward axis
            return (float)Math.Cos(angleFromFwrd) * posOffset.Length;
        }
    }
}
