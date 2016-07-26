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
            var rotation = new Rotation(0, 90, 0);

            var point = new Vector3(1f, 0f, 1f);

            var result = Vector3.Transform(point, rotation.Quaternion);

            var expectedResult = new Vector3(-1f, 0f, 1f);

            Assert.That(result.X, Is.EqualTo(expectedResult.X).Within(0.00001));
            Assert.That(result.Y, Is.EqualTo(expectedResult.Y).Within(0.00001));
            Assert.That(result.Z, Is.EqualTo(expectedResult.Z).Within(0.00001));
        }

        [TestMethod]
        public void TestPitch()
        {
            var rotation = new Rotation(45, 0, 0);

            var point = new Vector3(0f, 0f, 1f);

            var result = Vector3.Transform(point, rotation.Quaternion);

            var expectedResult = new Vector3(0f, 0.7071f, 0.7071f);

            Assert.That(result.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            Assert.That(result.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            Assert.That(result.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));
        }

        [TestMethod]
        public void TestRoll()
        {
            var rotation = new Rotation(0, 0, 90);

            var point = new Vector3(0f, 1f, 0f);

            var result = Vector3.Transform(point, rotation.Quaternion);

            var expectedResult = new Vector3(1f, 0f, 0f);

            Assert.That(result.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            Assert.That(result.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            Assert.That(result.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));
        }

        [TestMethod]
        public void TestYawPitch()
        {
            var rotation = new Rotation(315, 180, 0);

            var point = new Vector3(0f, 0f, 1f);

            var result = Vector3.Transform(point, rotation.Quaternion);

            var expectedResult = new Vector3(0f, -0.7071f, -0.7071f);

            Assert.That(result.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            Assert.That(result.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            Assert.That(result.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));
        }

        [TestMethod]
        public void TestConvertion()
        {
            var rotations = new Rotation[]
            {
                new Rotation(0,0,0),

                new Rotation(45,0,0),
                new Rotation(0,45,0),
                new Rotation(0,0,45),

                new Rotation(90,0,0),
                new Rotation(0,90,0),
                new Rotation(0,0,90),

                new Rotation(180,0,0),
                new Rotation(0,180,0),
                new Rotation(0,0,180),

                new Rotation(45,45,0),
                new Rotation(0,45,45),
                new Rotation(45,0,45),

                new Rotation(90,90,0),
                new Rotation(0,90,90),
                new Rotation(90,0,90),

                new Rotation(180,180,0),
                new Rotation(0,180,180),
                new Rotation(180,0,180),
            };

            //var quaternions = new Quaternion[rotations.Length];
            var passed = true;
            for (int i = 0; i < rotations.Length; i++)
            {
                passed = passed & TestConvertion(rotations[i]);
                //var testRot = new Rotation(rotations[i].Quaternion);
                //AssertVectors(testRot.EulerAngles, rotations[i].EulerAngles);
            }

            Assert.IsTrue(passed);
            //var rotation = new Rotation(315, 180, 0);
            //var rotation2 = new Rotation(rotation.Quaternion);
            //var rotation3 = new Rotation(rotation2.EulerAngles);

            //var point = new Vector3(0f, 0f, 1f);


            //var result1 = Vector3.Transform(point, rotation2.Quaternion);
            //var result2 = Vector3.Transform(point, rotation3.Quaternion);
            //var expectedResult = Vector3.Transform(point, rotation.Quaternion);

            //Assert.That(result1.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            //Assert.That(result1.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            //Assert.That(result1.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));

            //Assert.That(result2.X, Is.EqualTo(expectedResult.X).Within(0.0001));
            //Assert.That(result2.Y, Is.EqualTo(expectedResult.Y).Within(0.0001));
            //Assert.That(result2.Z, Is.EqualTo(expectedResult.Z).Within(0.0001));
        }

        private bool TestConvertion(Rotation rotation)
        {
            var result = new Rotation(rotation.Quaternion);
            result = new Rotation(result.EulerAngles);
            try
            {
                AssertVectors(result.EulerAngles, rotation.EulerAngles);
            }
            catch
            {
                Trace.WriteLine(string.Format("Rotation ({0}) failed to convert. (result = {1})", rotation, result));
                return false;
            }
            return true;
        }

        private static void AssertVectors(Vector3 v1, Vector3 v2)
        {

            Assert.That(v1.X, Is.EqualTo(v2.X).Within(0.0001));
            Assert.That(v1.Y, Is.EqualTo(v2.Y).Within(0.0001));
            Assert.That(v1.Z, Is.EqualTo(v2.Z).Within(0.0001));
        }

    }
}
