using OpenTK;
using Poly3D.Engine;
using Poly3D.Engine.Meshes;
using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poly3D.Test
{
    class ConsoleProgram
    {
        static void Main(string[] args)
        {
            //var angle = Angle.FromDegrees(45);
            //var testRot = new Rotation() { Pitch = 45, Yaw = 0 };
            //var eulerAngles = new Vector3(testRot.Yaw.Radians, testRot.Pitch.Radians, testRot.Roll.Radians);
            //var test1 = GLMath.QuaternionFromEulerAngles(eulerAngles);
            //var test2 = GLMath.EulerAnglesFromQuaternion(test1);

            //Trace.WriteLine(string.Format("{0} == {1} ?", eulerAngles, test2));

            //Trace.WriteLine("Pitch:");
            //var myRot = new Rotation() { Pitch = 45 };
            //var myRot2 = (Rotation)myRot.Quaternion;
            //Trace.WriteLine(myRot);
            //Trace.WriteLine(myRot2);
            //Trace.WriteLine("Roll:");
            //myRot = new Rotation() { Roll = 45 };
            //myRot2 = (Rotation)myRot.Quaternion;
            //Trace.WriteLine(myRot);
            //Trace.WriteLine(myRot2);

            //var rotMat = Matrix4.CreateFromQuaternion(myRot.Quaternion);
            //Trace.WriteLine(Vector3.Transform(new Vector3(5f, 0f, 0f), rotMat));
            //rotMat = Matrix4.CreateFromQuaternion(myRot2.Quaternion);
            //Trace.WriteLine(Vector3.Transform(new Vector3(5f, 0f, 0f), rotMat));
            
            //Trace.WriteLine("Yaw:");
            //myRot = new Rotation() { Yaw = 45 };
            //myRot2 = (Rotation)myRot.Quaternion;
            //Trace.WriteLine(myRot);
            //Trace.WriteLine(myRot2);
            //var rotMat = Matrix4.CreateFromQuaternion(myRot.Quaternion);
            //Trace.WriteLine(Vector3.Transform(new Vector3(0f, 0f, 5f), rotMat));
            //var myRot2 = (Rotation)myRot.Quaternion;


            
            var testObj = new SceneObject();
            testObj.Transform.Position = new Vector3(0, 3, 6);
            testObj.Transform.LookAt(Vector3.Zero);

            var testPlane = new Plane(Vector3.UnitY, 0);
            var testRay = Ray.FromPoints(testObj.Transform.Position, Vector3.Zero);
            float hitDist = 0;
            if (testPlane.Raycast(testRay, out hitDist))
            {
                var hitPos = testRay.GetPoint(hitDist);
            }

            //testObj.Transform.Scale = new OpenTK.Vector3(.5f, .5f, .5f);

            Trace.WriteLine(" world pos:" + testObj.Transform.WorldPosition.Round());
            Trace.WriteLine(" local rot:" + testObj.Transform.Rotation);
            Trace.WriteLine(" world rot:" + testObj.Transform.WorldRotation);

            var testObj2 = new SceneObject();
            testObj2.Parent = testObj;
            testObj2.Transform.Position = Vector3.UnitZ * 5;
            testObj2.Transform.Rotation = new Vector3(-90, 0, 0);

            Trace.WriteLine(" world pos:" + testObj2.Transform.WorldPosition.Round());
            Trace.WriteLine(" local rot:" + testObj2.Transform.Rotation);
            Trace.WriteLine(" world rot:" + testObj2.Transform.WorldRotation);

            var testObj3 = new SceneObject();
            testObj3.Parent = testObj2;
            testObj3.Transform.Position = Vector3.UnitZ * 5;
            //testObj2.Transform.Rotation = new Vector3(0, 90, 0);

            Trace.WriteLine(" world pos:" + testObj3.Transform.WorldPosition.Round());
            Trace.WriteLine(" local rot:" + testObj3.Transform.Rotation);
            Trace.WriteLine(" world rot:" + testObj3.Transform.WorldRotation);

        }
    }
}
