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
        private MouseState lastMouse;
        private Vector2 lastMousePos = Vector2.Zero;
        private Vector3 CameraTarget;

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
            if (groundPlane.Raycast(camRay, out distFromGround))
            {
                CameraTarget = camRay.GetPoint(distFromGround);
            }
            else
                CameraTarget = Vector3.Zero;
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
                CameraTarget = Vector3.Zero;
                Camera.Transform.LookAt(CameraTarget, false);
                Camera.Transform.SetRotation(RotationComponent.Roll, 0f);//reset roll
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
                    CameraTarget += panTranslate;
                    Camera.Transform.Translate(panTranslate, Space.World);
                }
                //Orbit camera
                else
                {
                    var cameraDist = Camera.Transform.WorldPosition - CameraTarget;

                    var pitchAngle = Angle.FromDegrees(360f * mouseViewDelta.Y);

                    var newPos = Vector3.Transform(cameraDist, Matrix4.CreateFromAxisAngle(Camera.Transform.Right, pitchAngle.Radians));

                    Camera.Transform.WorldPosition = newPos + CameraTarget;
                    Camera.Transform.LookAt(CameraTarget, false);

                    if (Camera.Transform.Up != Vector3.UnitY)
                    {
                        var yawAngle = Angle.FromDegrees(360f * mouseViewDelta.X);
                        if (Camera.Transform.Up.Y < 0)
                            yawAngle = -(float)yawAngle;
                        newPos = Vector3.Transform(newPos, Matrix4.CreateFromAxisAngle(Vector3.UnitY, -yawAngle.Radians));
                        Camera.Transform.WorldPosition = newPos + CameraTarget;
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
                    var cameraTarget = Vector3.Zero;
                    var cameraDist = Camera.Transform.WorldPosition - cameraTarget;
                    scrollAmount *= cameraDist.Length / 10f;
                    Camera.Transform.Translate(new Vector3(0, 0, scrollAmount), Space.Self);
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

        private static Vector2 GetMousePosition()
        {
            var curPos = System.Windows.Forms.Cursor.Position;
            return new Vector2(curPos.X, curPos.Y);
        }
    }
}
