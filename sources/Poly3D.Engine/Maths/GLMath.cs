using OpenTK;
using System;

namespace Poly3D.Maths
{
    public static class GLMath
    {
        #region Quaternion Extensions


        public static Vector3 Round(this Vector3 vector)
        {
            return new Vector3((float)Math.Truncate(vector.X * 10f) / 10f, (float)Math.Round(vector.Y * 10f) / 10f, (float)Math.Round(vector.Z * 10f) / 10f);
        }

        public static Vector3 Mult(this Quaternion rotation, Vector3 point)
        {
            float num = rotation.X * 2f;
            float num2 = rotation.Y * 2f;
            float num3 = rotation.Z * 2f;
            float num4 = rotation.X * num;
            float num5 = rotation.Y * num2;
            float num6 = rotation.Z * num3;
            float num7 = rotation.X * num2;
            float num8 = rotation.X * num3;
            float num9 = rotation.Y * num3;
            float num10 = rotation.W * num;
            float num11 = rotation.W * num2;
            float num12 = rotation.W * num3;
            Vector3 result;
            result.X = (1f - (num5 + num6)) * point.X + (num7 - num12) * point.Y + (num8 + num11) * point.Z;
            result.Y = (num7 + num12) * point.X + (1f - (num4 + num6)) * point.Y + (num9 - num10) * point.Z;
            result.Z = (num8 - num11) * point.X + (num9 + num10) * point.Y + (1f - (num4 + num5)) * point.Z;
            return result;
        }

        public static Vector3 ToEuler(this Quaternion rotation)
        {
            return EulerAnglesFromQuaternion(rotation);
        }

        #endregion

        public const float PI = 3.1415926535897931f;
        public const float TO_RAD = 0.0174532924f;
        public const float TO_DEG = 57.29578f;

        public static Quaternion QuaternionFromEulerAngles(Vector3 angles)
        {
            return QuaternionFromEulerAngles(angles.Y, angles.X, angles.Z);
        }

        /// <summary>
        /// 
        /// </summary
        /// <param name="yaw">Yaw in radians</param>
        /// <param name="pitch">Pitch in radians</param>
        /// <param name="roll">Roll in radians</param>
        /// <returns></returns>
        public static Quaternion QuaternionFromEulerAngles(float yaw, float pitch, float roll)
        {
            float rollOver2 = roll * 0.5f;
            float sinRollOver2 = (float)Math.Sin((double)rollOver2);
            float cosRollOver2 = (float)Math.Cos((double)rollOver2);
            float pitchOver2 = pitch * 0.5f;
            float sinPitchOver2 = (float)Math.Sin((double)pitchOver2);
            float cosPitchOver2 = (float)Math.Cos((double)pitchOver2);
            float yawOver2 = yaw * 0.5f;
            float sinYawOver2 = (float)Math.Sin((double)yawOver2);
            float cosYawOver2 = (float)Math.Cos((double)yawOver2);
            Quaternion result = new Quaternion();
            result.W = cosYawOver2 * cosPitchOver2 * cosRollOver2 + sinYawOver2 * sinPitchOver2 * sinRollOver2;
            result.X = cosYawOver2 * sinPitchOver2 * cosRollOver2 + sinYawOver2 * cosPitchOver2 * sinRollOver2;
            result.Y = sinYawOver2 * cosPitchOver2 * cosRollOver2 - cosYawOver2 * sinPitchOver2 * sinRollOver2;
            result.Z = cosYawOver2 * cosPitchOver2 * sinRollOver2 - sinYawOver2 * sinPitchOver2 * cosRollOver2;

            return result;
            //Quaternion rotateX = Quaternion.FromAxisAngle(Vector3.UnitX, pitch);
            //Quaternion rotateY = Quaternion.FromAxisAngle(Vector3.UnitY, yaw);
            //Quaternion rotateZ = Quaternion.FromAxisAngle(Vector3.UnitZ, roll);
            //Quaternion.Multiply(ref rotateZ, ref rotateY, out rotateY);
            //Quaternion.Multiply(ref rotateX, ref rotateY, out rotateY);
            //return rotateY;
        }

        public static Vector3 EulerAnglesFromQuaternion(Quaternion q)
        {
            Vector3 pitchYawRoll = new Vector3();

            double sqw = q.W * q.W;
            double sqx = q.X * q.X;
            double sqy = q.Y * q.Y;
            double sqz = q.Z * q.Z;

            // If quaternion is normalised the unit is one, otherwise it is the correction factor
            double unit = sqx + sqy + sqz + sqw;
            double test = q.X * q.Y + q.Z * q.W;

            if (test > 0.499f * unit)
            {
                // Singularity at north pole
                pitchYawRoll.Y = 2f * (float)Math.Atan2(q.X, q.W);  // Yaw
                pitchYawRoll.X = PI * 0.5f;                         // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }
            else if (test < -0.499f * unit)
            {
                // Singularity at south pole
                pitchYawRoll.Y = -2f * (float)Math.Atan2(q.X, q.W); // Yaw
                pitchYawRoll.X = -PI * 0.5f;                        // Pitch
                pitchYawRoll.Z = 0f;                                // Roll
                return pitchYawRoll;
            }

            pitchYawRoll.Y = (float)Math.Atan2(2 * q.Y * q.W - 2 * q.X * q.Z, sqx - sqy - sqz + sqw);       // Yaw
            pitchYawRoll.X = (float)Math.Asin(2 * test / unit);                                             // Pitch
            pitchYawRoll.Z = (float)Math.Atan2(2 * q.X * q.W - 2 * q.Y * q.Z, -sqx + sqy - sqz + sqw);      // Roll

            return new Vector3(
                Angle.NormalizeRadians(pitchYawRoll.Z),
                Angle.NormalizeRadians(pitchYawRoll.Y),
                Angle.NormalizeRadians(pitchYawRoll.X));
        }

    }
}
