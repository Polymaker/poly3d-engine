using Poly3D.Engine.Meshes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poly3D.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = new Stopwatch();
            timer.Start();
            var objMesh = WavefrontMeshLoader.LoadWavefrontObj(@"C:\Users\jturner\Documents\32496.obj");
            timer.Stop();
            Trace.WriteLine("Mesh loaded in " + timer.Elapsed);
            timer.Restart();
            objMesh.ComputeSurfaces();
            timer.Stop();
            Trace.WriteLine("Surfaces computed in " + timer.Elapsed);
            timer.Restart();
            foreach (var surf in objMesh.Surfaces)
                surf.ComputeBoundingEdges();
            timer.Stop();
            Trace.WriteLine("Surfaces' bounding edges computed in " + timer.Elapsed);
        }
    }
}
