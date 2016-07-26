using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
using Poly3D.Maths;
using OpenTK;
using NUnit.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assert = NUnit.Framework.Assert;
using System.Collections.Generic;
using System.Diagnostics;
namespace Poly3d.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestYaw()
        {
            //var rotation = new Rotation(0, 90, 0);

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

        private bool TestConvertion(Rotation rotation)
        {
            var rotation2 = new Rotation(rotation.Quaternion);
            var point = new Vector3(1f, 1f, 1f);
            rotation2 = new Rotation(rotation2.EulerAngles);
            var result = Vector3.Transform(point, rotation2.Quaternion);
            var expectedResult = Vector3.Transform(point, rotation.Quaternion);
            try
            {
                AssertVectors(result, expectedResult);
            }
            catch
            {
                Trace.WriteLine(string.Format("Rotation ({0}) failed to convert. (result = {1})", result, expectedResult));
                return false;
            }
            return true;
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

    }
}
