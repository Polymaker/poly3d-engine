using OpenTK;
using System;
using System.Diagnostics;

namespace Poly3D.Maths
{
    public static class GLMath
    {

        public const float PI = 3.1415926535897931f;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="angles">A Vector3 containing pitch, yaw and roll in radians.</param>
        /// <returns></returns>
        public static Quaternion EulerToQuat(Vector3 angles)
        {
            //I multiply the axises by -1 because the default rotation seems counter-clockwise
            var roll = Quaternion.FromAxisAngle(Vector3.UnitZ * -1f, angles.Z);
            var pitch = Quaternion.FromAxisAngle(Vector3.UnitX * -1f, angles.X);
            var yaw = Quaternion.FromAxisAngle(Vector3.UnitY * -1f, angles.Y);

            return yaw * pitch * roll;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="quat"></param>
        /// <returns>Returns a Vector3 containing pitch, yaw and roll in radians.</returns>
        public static Vector3 QuatToEuler(Quaternion quat)
        {
            Vector3 pitchYawRoll = new Vector3();
            //don't ask me why, but after a million tries, doing this will return the result I expected, and can convert back and forth from euler to quat
            quat = new Quaternion(-quat.Z, -quat.Y, -quat.X, quat.W);

            double sqw = quat.W * quat.W;
            double sqx = quat.X * quat.X;
            double sqy = quat.Y * quat.Y;
            double sqz = quat.Z * quat.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = quat.X * quat.Y + quat.Z * quat.W;

            if (test > 0.499f * unit)
            {
                // Singularity at north pole
                pitchYawRoll.Y = 2f * (float)Math.Atan2(quat.X, quat.W);  // Yaw
                pitchYawRoll.X = PI * 0.5f;                         // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else if (test < -0.499f * unit)
            {
                // Singularity at south pole
                pitchYawRoll.Y = -2f * (float)Math.Atan2(quat.X, quat.W); // Yaw
                pitchYawRoll.X = -PI * 0.5f;                        // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }

            pitchYawRoll.Y = (float)Math.Atan2(2 * quat.Y * quat.W - 2 * quat.X * quat.Z, sqx - sqy - sqz + sqw);       // Yaw
            pitchYawRoll.X = (float)Math.Asin(2 * test / unit);                                             // Pitch
            pitchYawRoll.Z = (float)Math.Atan2(2 * quat.X * quat.W - 2 * quat.Y * quat.Z, -sqx + sqy - sqz + sqw);      // Roll

            return pitchYawRoll;
        }

        #region Vector3 Extensions

        public static Vector3 ToRadians(this Vector3 v)
        {
            return new Vector3(Angle.ToRadians(v.X), Angle.ToRadians(v.Y), Angle.ToRadians(v.Z));
        }

        public static Vector3 ToDegrees(this Vector3 v)
        {
            return new Vector3(Angle.ToDegrees(v.X), Angle.ToDegrees(v.Y), Angle.ToDegrees(v.Z));
        }

        public static Matrix4 RotationFromTo(this Vector3 v1, Vector3 v2)
        {
            if (v1 == v2)
                return Matrix4.Identity;
            var axis = Vector3.Cross(v1, v2);

            float d = Vector3.Dot(v1, v2);
            var angle = (float)Math.Acos(d);

            return Matrix4.CreateFromAxisAngle(axis, angle);
        }

        #endregion

        public static Vector3 ComputeNormal(params Vector3[] vertices)
        {
            var u = vertices[1] - vertices[0];
            var v = vertices[2] - vertices[0];
            return new Vector3((u.Y * v.Z) - (u.Z * v.Y), (u.Z * v.X) - (u.X * v.Z), (u.X * v.Y) - (u.Y * v.X)).Normalized();
        }
    }
}
