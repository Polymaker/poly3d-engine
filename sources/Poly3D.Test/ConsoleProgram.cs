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
            Vector3 eulerAngles = new Vector3(45, 45, 0).ToRadians();
            var rotQuat = GLMath.EulerToQuat(eulerAngles);

            Trace.WriteLine("In Euler: " + eulerAngles.ToDegrees());
            Trace.WriteLine("In Quat: " + rotQuat);

            eulerAngles = GLMath.QuatToEuler(rotQuat);
            rotQuat = GLMath.EulerToQuat(eulerAngles);

            Trace.WriteLine("Out Euler: " + eulerAngles.ToDegrees());
            Trace.WriteLine("Out Quat: " + rotQuat);
            Trace.WriteLine(string.Empty);

            eulerAngles = new Vector3(0, 45, 45).ToRadians();
            rotQuat = GLMath.EulerToQuat(eulerAngles);

            Trace.WriteLine("In Euler: " + eulerAngles.ToDegrees());
            Trace.WriteLine("In Quat: " + rotQuat);

            eulerAngles = GLMath.QuatToEuler(rotQuat);
            rotQuat = GLMath.EulerToQuat(eulerAngles);

            Trace.WriteLine("Out Euler: " + eulerAngles.ToDegrees());
            Trace.WriteLine("Out Quat: " + rotQuat);
            Trace.WriteLine(string.Empty);

            eulerAngles = new Vector3(0, 45, 45).ToRadians();
            rotQuat = GLMath.EulerToQuat(eulerAngles);

            Trace.WriteLine("In Euler: " + eulerAngles.ToDegrees());
            Trace.WriteLine("In Quat: " + rotQuat);

            eulerAngles = GLMath.QuatToEuler(rotQuat);
            rotQuat = GLMath.EulerToQuat(eulerAngles);

            Trace.WriteLine("Out Euler: " + eulerAngles.ToDegrees());
            Trace.WriteLine("Out Quat: " + rotQuat);
            Trace.WriteLine(string.Empty);

            eulerAngles = new Vector3(90, 90, 0).ToRadians();
            rotQuat = GLMath.EulerToQuat(eulerAngles);

            Trace.WriteLine("In Euler: " + eulerAngles.ToDegrees());
            Trace.WriteLine("In Quat: " + rotQuat);

            eulerAngles = GLMath.QuatToEuler(rotQuat);
            rotQuat = GLMath.EulerToQuat(eulerAngles);

            Trace.WriteLine("Out Euler: " + eulerAngles.ToDegrees());
            Trace.WriteLine("Out Quat: " + rotQuat);
            Trace.WriteLine(string.Empty);
        }
    }
}
