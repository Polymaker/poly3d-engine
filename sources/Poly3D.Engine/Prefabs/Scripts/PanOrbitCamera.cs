using OpenTK;
using OpenTK.Input;
using Poly3D.Engine;
using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Poly3D.Prefabs.Scripts
{
    public sealed class PanOrbitCamera : ObjectBehavior
    {
        private float _TargetDistance;
        private MouseState lastMouse;
        private Vector2 lastMousePos = Vector2.Zero;
        private CameraMovement CurrentMovement;
        //private Vector3 _CameraTarget;
        private Transform _Target;

        public Vector3 CameraTarget
        {
            get
            {
                return Camera.Transform.WorldPosition + Camera.Transform.Forward * TargetDistance;
            }
            set
            {
                Angle currentRoll = Camera.Transform.Rotation.Roll;
                Camera.Transform.LookAt(value);
                Camera.Transform.SetRotation(RotationComponent.Roll, currentRoll);
                _TargetDistance = (value - Camera.Transform.WorldPosition).Length;
            }
        }
        
        public float TargetDistance
        {
            get { return _TargetDistance; }
            private set
            {
                _TargetDistance = value;
            }
        }

        public Camera Camera
        {
            get { return (Camera)EngineObject; }
        }

        public MouseButton RotationButton { get; set; }

        public bool AllowPan { get; set; }

        protected override void OnInitialize()
        {
            lastMouse = Mouse.GetState();
            lastMousePos = Scene.Display.GetMousePosition();
            RotationButton = MouseButton.Middle;

            var groundPlane = new Plane(Vector3.UnitY, 0);
            var camRay = new Ray(Camera.Transform.WorldPosition, Camera.Transform.Forward);
            float distFromGround;

            Vector3 camTarget = Vector3.Zero;
            if (groundPlane.Raycast(camRay, out distFromGround))
                camTarget = camRay.GetPoint(distFromGround);

            //_Target = new Transform(camTarget, Rotation.Identity, Vector3.One);

            TargetDistance = (camTarget - Camera.Transform.WorldPosition).Length;
        }

        enum CameraMovement
        {
            None,
            Orbit,
            Pan,
            Roll
        }

        protected override void OnUpdate(float deltaTime)
        {
            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();
            var mousePos = Scene.Display.GetMousePosition();
            var mouseDelta = new Vector2(mousePos.X - lastMousePos.X, mousePos.Y - lastMousePos.Y);

            //AdjustTarget();

            if (!Scene.Display.Focused)
            {
                if (Math.Abs(mouseDelta.Length) > 1)
                    lastMousePos = mousePos;
                lastMouse = mouseState;
                return;
            }
            
            if (mouseState.RightButton == ButtonState.Released && 
                lastMouse.RightButton == ButtonState.Pressed)
            {
                Camera.Transform.LookAt(Vector3.Zero, false);
                Camera.Transform.SetRotation(RotationComponent.Roll, 0f);//reset roll
                AdjustTargetDist(Vector3.Zero);
            }

            
            if (mouseState.IsButtonDown(RotationButton) && Math.Abs(mouseDelta.Length) > 1)
            {
                var mouseViewDelta = Vector2.Divide(mouseDelta, Camera.DisplayRectangle.Size);

                //Roll camera
                if (keyState.IsKeyDown(Key.ControlLeft) || keyState.IsKeyDown(Key.ControlRight))
                {
                    CurrentMovement = CameraMovement.Roll;
                    var rollAmount = mouseViewDelta.X * 360f;
                    Camera.Transform.Rotate(new Rotation(0, 0, rollAmount), RelativeSpace.Self);
                }
                //Pan camera
                else if (keyState.IsKeyDown(Key.LShift) || keyState.IsKeyDown(Key.RShift))
                {
                    CurrentMovement = CameraMovement.Pan;
                    var viewSize = Camera.GetViewSize(Camera.GetDistanceFromCamera(CameraTarget));

                    var panTranslate = Camera.Transform.Up * mouseViewDelta.Y * viewSize.Y;
                    panTranslate += Camera.Transform.Right * mouseViewDelta.X * viewSize.X;

                    Camera.Transform.Translate(panTranslate, RelativeSpace.World);
                }
                //Orbit camera
                else
                {
                    CurrentMovement = CameraMovement.Orbit;

                    var cameraPivot = CameraTarget;
                    var cameraOffset = Camera.Transform.WorldPosition - cameraPivot;

                    var pitchAngle = Angle.FromDegrees(360f * mouseViewDelta.Y);

                    var newPos = Vector3.Transform(cameraOffset, Matrix4.CreateFromAxisAngle(Camera.Transform.Right, pitchAngle.Radians));

                    Camera.Transform.WorldPosition = newPos + cameraPivot;
                    Camera.Transform.LookAt(cameraPivot, false);

                    if (Camera.Transform.Up != Vector3.UnitY)
                    {
                        var yawAngle = Angle.FromDegrees(360f * mouseViewDelta.X);
                        if (Camera.Transform.Up.Y < 0)
                            yawAngle = -(float)yawAngle;
                        newPos = Vector3.Transform(newPos, Matrix4.CreateFromAxisAngle(Vector3.UnitY, -yawAngle.Radians));
                        Camera.Transform.WorldPosition = newPos + cameraPivot;
                        Camera.Transform.Rotate(new Vector3(0, yawAngle.Degrees, 0), RelativeSpace.Parent);
                    }

                    AdjustTargetDist(cameraPivot);
                }
            }
            else if (!mouseState.IsButtonDown(RotationButton))
            {
                CurrentMovement = CameraMovement.None;
            }

            if (mouseState.Wheel != lastMouse.Wheel && 
                Scene.Display.GetDisplayBounds().Contains((int)mousePos.X, (int)mousePos.Y))
            {
                var scrollAmount = mouseState.WheelPrecise - lastMouse.WheelPrecise;

                if (Camera.Projection == ProjectionType.Perspective)
                {
                    var cameraPivot = CameraTarget;
                    scrollAmount *= TargetDistance / 10f;
                    Camera.Transform.Translate(new Vector3(0, 0, scrollAmount), RelativeSpace.Self);
                    AdjustTargetDist(cameraPivot);
                }
                else if (Camera.Projection == ProjectionType.Orthographic)
                {
                    scrollAmount *= Camera.OrthographicSize / 10f;
                    Camera.OrthographicSize -= scrollAmount;
                }
            }

            if (Math.Abs(mouseDelta.Length) > 1)
                lastMousePos = mousePos;

            lastMouse = mouseState;
        }

        protected override void OnRender(Camera camera)
        {
            base.OnRender(camera);
            if (CurrentMovement == CameraMovement.Orbit)
            {
                OpenTK.Graphics.OpenGL.GL.PushMatrix();
                var invertMat = EngineObject.Transform.LocalToWorldMatrix.Inverted();
                OpenTK.Graphics.OpenGL.GL.MultMatrix(ref invertMat);
                var targetMat = new ComplexTransform(CameraTarget, Rotation.Identity, Vector3.One).TransformMatrix;
                OpenTK.Graphics.OpenGL.GL.MultMatrix(ref targetMat);
                RenderHelper.RenderUIScaledManipulator(camera, CameraTarget, 40, TransformType.Rotation);
                OpenTK.Graphics.OpenGL.GL.PopMatrix();
            }
        }

        //private void AdjustTarget()
        //{
        //    var calcTarget = Camera.Transform.WorldPosition + Camera.Transform.Forward * TargetDistance;
        //    if ((calcTarget - CameraTarget).Length > float.Epsilon)
        //    {
        //        _CameraTarget = Camera.Transform.WorldPosition + Camera.Transform.Forward * TargetDistance;
        //    }
        //}

        public void SetDistanceFromTarget(float distance)
        {
            var finalPos = CameraTarget + (Camera.Transform.Forward * -1 * distance);
            TargetDistance = distance;
            Camera.Transform.WorldPosition = finalPos;
        }

        private void AdjustTargetDist(Vector3 newTarget)
        {
            TargetDistance = (newTarget - Camera.Transform.WorldPosition).Length;
        }
    }
}
