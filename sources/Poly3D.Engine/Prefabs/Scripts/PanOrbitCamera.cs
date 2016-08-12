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
    public sealed class PanOrbitCamera : ObjectBehaviour
    {
        private float _TargetDistance;
        private MouseState lastMouse;
        private Vector2 lastMousePos = Vector2.Zero;
        
        public Vector3 CameraTarget
        {
            get
            {
                return Camera.Transform.WorldPosition + Camera.Transform.Forward * TargetDistance;
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
            lastMousePos = GetMousePosition();
            RotationButton = MouseButton.Middle;

            var groundPlane = new Plane(Vector3.UnitY, 0);
            var camRay = new Ray(Camera.Transform.WorldPosition, Camera.Transform.Forward);
            float distFromGround;
            Vector3 camTarget = Vector3.Zero;
            if (groundPlane.Raycast(camRay, out distFromGround))
            {
                camTarget = camRay.GetPoint(distFromGround);
            }
            else
                camTarget = Vector3.Zero;
            TargetDistance = (camTarget - Camera.Transform.WorldPosition).Length;
        }

        protected override void OnUpdate(float deltaTime)
        {
            var mouseState = Mouse.GetState();
            var keyState = Keyboard.GetState();
            var mousePos = GetMousePosition();
            var mouseDelta = new Vector2(mousePos.X - lastMousePos.X, mousePos.Y - lastMousePos.Y);

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
                    var rollAmount = mouseViewDelta.X * 360f;
                    Camera.Transform.Rotate(new Rotation(0, 0, rollAmount), Space.Self);
                }
                //Pan camera
                else if (keyState.IsKeyDown(Key.LShift) || keyState.IsKeyDown(Key.RShift))
                {

                    var viewSize = Camera.GetViewSize(Camera.GetDistanceFromCamera(CameraTarget));

                    var panTranslate = Camera.Transform.Up * mouseViewDelta.Y * viewSize.Y;
                    panTranslate += Camera.Transform.Right * mouseViewDelta.X * viewSize.X;

                    Camera.Transform.Translate(panTranslate, Space.World);
                }
                //Orbit camera
                else
                {
                    
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
                        Camera.Transform.Rotate(new Vector3(0, yawAngle.Degrees, 0), Space.Parent);
                    }
                }
            }

            if (mouseState.Wheel != lastMouse.Wheel && 
                Scene.Display.GetDisplayBounds().Contains((int)mousePos.X, (int)mousePos.Y))
            {
                var scrollAmount = mouseState.WheelPrecise - lastMouse.WheelPrecise;

                if (Camera.Projection == ProjectionType.Perspective)
                {
                    var camPivot = CameraTarget;
                    scrollAmount *= TargetDistance / 10f;
                    Camera.Transform.Translate(new Vector3(0, 0, scrollAmount), Space.Self);
                    AdjustTargetDist(camPivot);
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

        private void AdjustTargetDist(Vector3 newTarget)
        {
            TargetDistance = (newTarget - Camera.Transform.WorldPosition).Length;
        }

        private static Vector2 GetMousePosition()
        {
            var curPos = System.Windows.Forms.Cursor.Position;
            return new Vector2(curPos.X, curPos.Y);
        }
    }
}
