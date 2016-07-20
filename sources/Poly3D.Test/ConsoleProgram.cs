using Poly3D.Engine;
using Poly3D.Engine.Meshes;
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
            testObj.Transform.Position = testObj.Transform.Forward * 5;
            testObj.Transform.EulerAngles = new OpenTK.Vector3(0, 90, 0);
            //testObj.Transform.Scale = new OpenTK.Vector3(.5f, .5f, .5f);

            Trace.WriteLine(testObj.Transform.WorldPosition);

            var testObj2 = new SceneObject();
            testObj2.Parent = testObj;
            testObj2.Transform.Position = testObj2.Transform.Forward * 5;
            testObj2.Transform.EulerAngles = new OpenTK.Vector3(0, 90, 0);
            Trace.WriteLine(testObj2.Transform.WorldPosition);

            var testObj3 = new SceneObject();
            testObj3.Parent = testObj2;
            testObj3.Transform.Position = testObj3.Transform.Forward * 5;
            Trace.WriteLine(testObj3.Transform.WorldPosition);


            var testObj4 = new SceneObject();
            testObj4.Parent = testObj3;
            //testObj4.Transform.Position = testObj4.Transform.Forward * 5;
            Trace.WriteLine(testObj4.Transform.WorldPosition);
            //var timer = new Stopwatch();
            //timer.Start();
            //var objMesh = Poly3D.Engine.Data.WavefrontMeshLoader.LoadWavefrontObj(@"C:\Users\jturner\Documents\32496.obj");
            //timer.Stop();
            //Trace.WriteLine("Mesh loaded in " + timer.Elapsed);
            //timer.Restart();
            //objMesh.ComputeSurfaces();
            //timer.Stop();
            //Trace.WriteLine("Surfaces computed in " + timer.Elapsed);
            //timer.Restart();
            //foreach (var surf in objMesh.Surfaces)
            //    surf.ComputeBoundingEdges();
            //timer.Stop();
            //Trace.WriteLine("Surfaces' bounding edges computed in " + timer.Elapsed);
        }
    }
}
