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
            var testObj = new SceneObject();
            testObj.Transform.Position = Vector3.UnitZ * 5;
            testObj.Transform.Rotation = new Vector3(45, 0, 0);

            Trace.WriteLine("obj1 local pos: " + testObj.Transform.Position);
            Trace.WriteLine("obj1 world pos: " + testObj.Transform.WorldPosition);
            Trace.WriteLine("obj1 local rot: " + testObj.Transform.Rotation);
            Trace.WriteLine("obj1 world rot: " + testObj.Transform.WorldRotation);

            var testObj2 = new SceneObject();
            testObj2.Parent = testObj;
            testObj2.Transform.Position = Vector3.UnitZ * 5;
            testObj2.Transform.Rotation = new Vector3(45, 0, 0);

            Trace.WriteLine("obj2 local pos: " + testObj2.Transform.Position);
            Trace.WriteLine("obj2 world pos: " + testObj2.Transform.WorldPosition);
            Trace.WriteLine("obj2 local rot: " + testObj2.Transform.Rotation);
            Trace.WriteLine("obj2 world rot: " + testObj2.Transform.WorldRotation);

            var testObj3 = new SceneObject();
            testObj3.Parent = testObj2;
            //testObj3.Transform.Rotation = new Vector3(-90, 0, 0);
            testObj3.Transform.Rotate(new Vector3(0, 45, 0), RelativeSpace.World);
            //testObj3.Transform.Translate(Vector3.UnitZ * 5f, Space.Self);
            //testObj3.Transform = new Transform(new Vector3(0, 0, 5), Rotation.Identity, Vector3.One);
            //testObj3.Transform.WorldRotation = new Vector3(0, 0, 0);
            //testObj3.Transform.WorldPosition += testObj3.Transform.Forward * 5;

            Trace.WriteLine("obj3 local pos: " + testObj3.Transform.Position);
            Trace.WriteLine("obj3 world pos: " + testObj3.Transform.WorldPosition);
            Trace.WriteLine("obj3 local rot: " + testObj3.Transform.Rotation);
            Trace.WriteLine("obj3 world rot: " + testObj3.Transform.WorldRotation);
        }
    }
}
