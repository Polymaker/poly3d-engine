using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

using Poly3D.Maths;
using Poly3D.Graphics;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;

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
                var displaySize = new Vector2(Scene.Display.Width, Scene.Display.Height);
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
            var curViewport = new Vector2(Scene.Display.Width, Scene.Display.Height);
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
            //var normalizedPoint = new Vector2((point.X - viewRect.X) / viewRect.Width, (point.Y - viewRect.Y) / viewRect.Height);

            return Raycast(ScreenPointToViewport(point));
        }

        public Ray Raycast(Vector2 viewportPoint)
        {
            if (viewportPoint.X < 0f || viewportPoint.X > 1f || viewportPoint.Y < 0f || viewportPoint.Y > 1f)
                return null;

            var cameraMatrix = Matrix4.Mult(GetModelviewMatrix(), ProjectionMatrix).Inverted();
            try
            {
                var nearFrustumPoint = ViewportPointToFrustum(viewportPoint, 0);
                var farFrustumPoint = ViewportPointToFrustum(viewportPoint, 1);

                var origin = FrustumPointToWorld(nearFrustumPoint, cameraMatrix);
                var target = FrustumPointToWorld(farFrustumPoint, cameraMatrix);

                return Ray.FromPoints(origin, target);
            }
            catch
            {
                return null;
            }
            
        }

        #region Point convertion from Viewport space

        /// <summary>
        /// Transforms position from viewport space into screen space.
        /// </summary>
        /// <param name="viewportPoint">Viewport point. The bottom-left is (0,0); the top-right is (1,1); front is 0; back is 1</param>
        /// <returns></returns>
        public Vector2 ViewportPointToScreen(Vector3 viewportPoint)
        {
            return ViewportPointToScreen(viewportPoint.Xy); //we discard Z as it doesn't affect screen space, only world space
        }

        /// <summary>
        /// Transforms position from viewport space into screen space.
        /// </summary>
        /// <param name="viewportPoint">Viewport point. The bottom-left is (0,0); the top-right is (1,1)</param>
        /// <returns>Returns a screen space coordinate.</returns>
        public Vector2 ViewportPointToScreen(Vector2 viewportPoint)
        {
            var viewRect = DisplayRectangle;
            return new Vector2(
                (int)(viewRect.X + viewRect.Width * viewportPoint.X),
                (int)(viewRect.Y + viewRect.Height * (1f - viewportPoint.Y)));
        }

        /// <summary>
        /// Converts Viewport point (0.0 to 1.0) to 'Frustum' (3D) point (-1.0 to +1.0)
        /// </summary>
        /// <param name="viewportPoint"></param>
        /// <param name="z"></param>
        /// <returns>Returns a frustum space coordinate.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Vector4 ViewportPointToFrustum(Vector2 viewportPoint, float z = 0f)
        {
            return ViewportPointToFrustum(new Vector3(viewportPoint.X, viewportPoint.Y, z));
        }

        /// <summary>
        /// Converts Viewport point (0.0 to 1.0) to 'Frustum' (3D) point (-1.0 to +1.0)
        /// </summary>
        /// <param name="viewportPoint"></param>
        /// <returns>Returns a frustum space coordinate.</returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Vector4 ViewportPointToFrustum(Vector3 viewportPoint)
        {
            if (viewportPoint.MaxComponent() > 1f || viewportPoint.MinComponent() < 0)
                throw new ArgumentException("Point not in viewport space (0.0 to 1.0).", "viewportPoint");
            return new Vector4(
                viewportPoint.X * 2f - 1f,
                viewportPoint.Y * 2f - 1f,
                viewportPoint.Z * 2f - 1f,
                1f);
        }

        /// <summary>
        /// Transforms position from viewport space into world space.
        /// </summary>
        /// <param name="viewportPoint">Viewport point. The bottom-left is (0,0); the top-right is (1,1)</param>
        /// <returns>Returns a world space coordinate.</returns>
        public Vector3 ViewportPointToWorld(Vector2 viewportPoint)
        {
            return FrustumPointToWorld(ViewportPointToFrustum(viewportPoint));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewportPoint"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Vector3 ViewportPointToWorld(Vector2 viewportPoint, float z)
        {
            return FrustumPointToWorld(ViewportPointToFrustum(viewportPoint, z));
        }

        /// <summary>
        /// Transforms position from viewport space into world space.
        /// </summary>
        /// <param name="viewportPoint">Viewport point. The bottom-left is (0,0); the top-right is (1,1); front is 0; back is 1</param>
        /// <returns></returns>
        public Vector3 ViewportPointToWorld(Vector3 viewportPoint)
        {
            return FrustumPointToWorld(ViewportPointToFrustum(viewportPoint));
        }

        #endregion

        #region Point convertion from Screen space

        /// <summary>
        /// Transforms position from screen space into viewport space.
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public Vector2 ScreenPointToViewport(Vector2 screenPoint)
        {
            var viewRect = DisplayRectangle;
            return new Vector2(
                (screenPoint.X - viewRect.X) / viewRect.Width, //left to right = 0 to 1
                (viewRect.Height - (screenPoint.Y - viewRect.Y)) / viewRect.Height);//up to bottom = 0 to 1 (invert Y)
        }

        /// <summary>
        /// Transforms position from screen space into world space.
        /// </summary>
        /// <param name="screenPoint"></param>
        /// <returns></returns>
        public Vector3 ScreenPointToWorld(Vector2 screenPoint)
        {
            return ViewportPointToWorld(ScreenPointToViewport(screenPoint));
        }

        #endregion

        #region Point convertion from World space

        /// <summary>
        /// Transforms position from world space into viewport space.
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public Vector3 WorldPointToViewport(Vector3 worldPoint)
        {
            var transformMatrix = Matrix4.Mult(GetModelviewMatrix(), ProjectionMatrix);
            var result = Vector4.Transform(new Vector4(worldPoint, 1), transformMatrix);
            //if result.Z < 0, point is behind camera (I think, not tested)
            return FrustumPointToViewPort(result);
        }

        /// <summary>
        /// Transforms position from world space into screen space.
        /// </summary>
        /// <param name="worldPoint"></param>
        /// <returns></returns>
        public Vector2 WorldPointToScreen(Vector3 worldPoint)
        {
            return ViewportPointToScreen(WorldPointToViewport(worldPoint));
        }

        #endregion

        #region Point convertion from Frustum

        private Vector3 FrustumPointToWorld(Vector4 frustumPoint)
        {
            var cameraMatrix = Matrix4.Mult(GetModelviewMatrix(), ProjectionMatrix).Inverted();
            return FrustumPointToWorld(frustumPoint, cameraMatrix);
        }

        private static Vector3 FrustumPointToWorld(Vector4 frustumPoint, Matrix4 cameraMatrix)
        {
            var result = Vector4.Transform(frustumPoint, cameraMatrix);
            if (result.W == 0)
                throw new Exception("Problems");
            return result.Xyz / result.W;
        }

        private static Vector3 FrustumPointToViewPort(Vector4 frustumPoint)
        {
            var pos = frustumPoint.Xyz / frustumPoint.W;
            return new Vector3((pos.X + 1f) / 2f, (pos.Y + 1f) / 2f, (pos.Z + 1f) / 2f);
        }

        #endregion

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
                    var worldPt = Vector3.Transform(localPt, meshObj.Transform.LocalToWorldMatrix);
                    var worldDist = (worldPt - ray.Origin).Length;
                    hits.Add(new Tuple<float, SceneObject>(worldDist, meshObj));
                }
            }
            
            if (hits.Count > 0)
                return hits.OrderBy(h => h.Item1).First().Item2;

            return null;
        }

        #region Visible world space calculations

        /// <summary>
        /// Gets the visible vertical height of the world at the distance specified. 
        /// </summary>
        /// <param name="distFromCamera"></param>
        /// <returns></returns>
        public float GetViewHeight(float distFromCamera)
        {
            if (Projection == ProjectionType.Perspective)
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

        #endregion

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
