using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poly3D.Maths;
using OpenTK;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using System.Collections.Generic;
using System.Diagnostics;
using Poly3D.Engine.Physics;

namespace Poly3D.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestYaw()
        {
            var point = new Vector3(1f, 0f, 1f);

            Vector3 eulerAngles = new Vector3(0, 90, 0).ToRadians();
            var rotQuat = GLMath.EulerToQuat(eulerAngles);
            var result = Vector3.Transform(point, rotQuat);

            var expectedResult = new Vector3(-1f, 0f, 1f);

            Assert.That(result.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            Assert.That(result.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            Assert.That(result.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));
        }

        [TestMethod]
        public void TestPitch()
        {

            var point = new Vector3(0f, 0f, 1f);

            Vector3 eulerAngles = new Vector3(45, 0, 0).ToRadians();
            var rotQuat = GLMath.EulerToQuat(eulerAngles);
            var result = Vector3.Transform(point, rotQuat);

            var expectedResult = new Vector3(0f, 0.7071f, 0.7071f);

            Assert.That(result.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            Assert.That(result.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            Assert.That(result.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));
        }

        [TestMethod]
        public void TestRoll()
        {

            var point = new Vector3(0f, 1f, 0f);

            Vector3 eulerAngles = new Vector3(0, 0, 90).ToRadians();
            var rotQuat = GLMath.EulerToQuat(eulerAngles);
            var result = Vector3.Transform(point, rotQuat);

            var expectedResult = new Vector3(1f, 0f, 0f);

            Assert.That(result.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            Assert.That(result.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            Assert.That(result.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));
        }

        [TestMethod]
        public void TestYawPitch()
        {
            var point = new Vector3(0f, 0f, 1f);

            Vector3 eulerAngles = new Vector3(315, 180, 0).ToRadians();
            var rotQuat = GLMath.EulerToQuat(eulerAngles);
            var result = Vector3.Transform(point, rotQuat);

            var expectedResult = new Vector3(0f, -0.7071f, -0.7071f);

            Assert.That(result.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            Assert.That(result.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            Assert.That(result.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));
        }

        [TestMethod]
        public void TestConvertion()
        {
            var rotations = new Vector3[]
            {
                new Vector3(0,0,0),
                new Vector3(45,0,0),
                new Vector3(0,45,0),
                new Vector3(0,0,45),
                new Vector3(90,0,0),
                new Vector3(0,90,0),
                new Vector3(0,0,90),
                new Vector3(180,0,0),
                new Vector3(0,180,0),
                new Vector3(0,0,180),
                new Vector3(45,45,0),
                new Vector3(0,45,45),
                new Vector3(45,0,45),
                new Vector3(90,90,0),
                new Vector3(0,90,90),
                new Vector3(90,0,90),
                new Vector3(180,180,0),
                new Vector3(0,180,180),
                new Vector3(180,0,180),
            };

            var passed = true;
            for (int i = 0; i < rotations.Length; i++)
            {
                passed = passed & TestConvertion(rotations[i]);
            }

            Assert.IsTrue(passed);

        }


        private bool TestConvertion(Vector3 eulerAngles)
        {
            var quat1 = GLMath.EulerToQuat(eulerAngles.ToRadians());
            var euler2 = GLMath.QuatToEuler(quat1).ToDegrees();
            var quat2 = GLMath.EulerToQuat(euler2.ToRadians());
            var point = new Vector3(1f, 1f, 1f);
            var result = Vector3.Transform(point, quat1);
            var expectedResult = Vector3.Transform(point, quat2);
            try
            {
                AssertVectors(result, expectedResult);
            }
            catch
            {
                Trace.WriteLine(string.Format("Rotation ({0}) failed to convert. (result = {1})", eulerAngles, euler2));
                Trace.WriteLine(string.Format("Point ({0}) failed to convert. (result = {1})", expectedResult, result));
                return false;
            }
            return true;
        }

        private static void AssertVectors(Vector3 v1, Vector3 v2)
        {

            Assert.That(v1.X, Is.EqualTo(v2.X).Within(0.001));
            Assert.That(v1.Y, Is.EqualTo(v2.Y).Within(0.001));
            Assert.That(v1.Z, Is.EqualTo(v2.Z).Within(0.001));
        }

        [TestMethod]
        public void TestIntersection()
        {
            var sphere = new BoundingSphere(Vector3.Zero, 4f);
            var ray = new Ray(new Vector3(4, 0, 10), new Vector3(0, 0, -1));
            float hitDist;
            if (PhysicsHelper.RayIntersectsSphere(ray, sphere, out hitDist))
            {
                var hitLoc = ray.GetPoint(hitDist);
            }
        }

    }
}
