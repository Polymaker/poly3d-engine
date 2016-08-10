using OpenTK;
using OpenTK.Input;
using Poly3D.Engine;
using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Poly3D.Test
{
    public class PanOrbitCamera : ObjectBehaviour
    {
        private MouseState lastMouse;
        private Vector2 lastMousePos = Vector2.Zero;
        private Vector3 CameraTarget;

        public Camera Camera
        {
            get { return (Camera)EngineObject; }
        }

        protected override void OnInitialize()
        {
            lastMouse = Mouse.GetState();
            lastMousePos = GetMousePosition();

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

            if (mouseState.RightButton == ButtonState.Released && lastMouse.RightButton == ButtonState.Pressed)
            {
                CameraTarget = Vector3.Zero;
                Camera.Transform.LookAt(CameraTarget, false);
                var rotAngles = Camera.Transform.Rotation.EulerAngles;
                rotAngles.Z = 0;//reset roll
                Camera.Transform.Rotation = rotAngles;

                //var sceneObj = EngineObject.Scene.GetObjectByName("WheelHub") as SceneObject;
                //if (sceneObj != null)
                //{
                //    var rot1 = sceneObj.Transform.WorldRotation;
                //    var rot2 = sceneObj.Transform.WorldRotation2;
                //    Trace.WriteLine(rot1);
                //    Trace.WriteLine(rot2);
                //}
            }

            if (mouseState.MiddleButton == ButtonState.Pressed && Math.Abs(mouseDelta.Length) > 1)
            {
                var mouseViewDelta = Vector2.Divide(mouseDelta, Camera.DisplayRectangle.Size);

                //Pan camera
                if (keyState.IsKeyDown(Key.LShift) || keyState.IsKeyDown(Key.RShift))
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
                        newPos = Vector3.Transform(newPos, Matrix4.CreateFromAxisAngle(Vector3.UnitY, -yawAngle.Radians));
                        Camera.Transform.WorldPosition = newPos + CameraTarget;
                        Camera.Transform.Rotate(new Vector3(0, yawAngle.Degrees, 0), Space.Self);
                    }
                }
            }

            if (mouseState.Wheel != lastMouse.Wheel)
            {
                var scrollAmount = mouseState.WheelPrecise - lastMouse.WheelPrecise;
                
                if (Camera.Projection == ProjectionType.Perspective)
                {
                    var cameraTarget = Vector3.Zero;
                    var cameraDist = Camera.Transform.WorldPosition - cameraTarget;
                    scrollAmount *= cameraDist.Length / 10f;
                    Camera.Transform.Translate(new Vector3(0, 0, scrollAmount), Space.Self);
                }
                else
                {
                    scrollAmount *= Camera.OrthographicSize / 10f;
                    Camera.OrthographicSize -= scrollAmount;
                }
            }

            if(Math.Abs(mouseDelta.Length) > 1)
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
