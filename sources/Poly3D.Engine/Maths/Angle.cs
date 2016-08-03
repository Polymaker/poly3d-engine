using System;

namespace Poly3D.Maths
{
    public struct Angle
    {
        public static readonly Angle Zero = new Angle();
        public static AngleUnit DefaultConvertionUnit = AngleUnit.Degrees;
        public const float PI = 3.1415926535897931f;

        private float angleDeg;
        
        public float Degrees
        {
            get { return angleDeg; }
            set
            {
                angleDeg = value;
            }
        }
        
        public float Radians
        {
            get { return ToRadians(Degrees); }
            set
            {
                angleDeg = ToDegrees(value);
            }
        }

        #region Static Ctors

        public static Angle FromDegrees(float degrees)
        {
            return new Angle { Degrees = degrees };
        }

        public static Angle FromRadians(float radians)
        {
            return new Angle { Radians = radians };
        }

        #endregion

        #region Operators

        public static explicit operator float(Angle angle)
        {
            return DefaultConvertionUnit == AngleUnit.Degrees ? angle.Degrees : angle.Radians;
        }

        public static implicit operator Angle(float angle)
        {
            return DefaultConvertionUnit == AngleUnit.Degrees ? FromDegrees(angle) : FromRadians(angle);
        }

        public static Angle operator +(Angle a1, Angle a2)
        {
            return FromDegrees(a1.Degrees + a2.Degrees);
        }

        public static Angle operator -(Angle a1, Angle a2)
        {
            return FromDegrees(a1.Degrees - a2.Degrees);
        }

        public static Angle operator *(Angle a1, float value)
        {
            return FromDegrees(a1.Degrees * value);
        }

        public static Angle operator /(Angle a1, float value)
        {
            return FromDegrees(a1.Degrees / value);
        }

        public static Angle operator %(Angle a1, float value)
        {
            return FromDegrees(a1.Degrees % value);
        }

        public static bool operator ==(Angle a1, Angle a2)
        {
            return a1.Degrees == a2.Degrees;
        }

        public static bool operator !=(Angle a1, Angle a2)
        {
            return a1.Degrees != a2.Degrees;
        }

        public static bool operator >(Angle a1, Angle a2)
        {
            return a1.Degrees > a2.Degrees;
        }

        public static bool operator <(Angle a1, Angle a2)
        {
            return a1.Degrees < a2.Degrees;
        }

        public static bool operator >=(Angle a1, Angle a2)
        {
            return a1.Degrees >= a2.Degrees;
        }

        public static bool operator <=(Angle a1, Angle a2)
        {
            return a1.Degrees <= a2.Degrees;
        }

        public override bool Equals(object obj)
        {
            if (obj is Angle)
                return Degrees.Equals(((Angle)obj).Degrees);
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return angleDeg.GetHashCode();
        }

        #endregion

        /// <summary>
        /// Clamps the angle between 0-360
        /// </summary>
        public void Normalize()
        {
            angleDeg = NormalizeDegrees(angleDeg);
        }

        public Angle Normalized()
        {
            return Angle.FromDegrees(NormalizeDegrees(angleDeg));
        }

        //assuming this is the minimum
        public Angle Diff(Angle other)//clockwise
        {
            var angle1 = NormalizeDegrees(Degrees);
            var angle2 = NormalizeDegrees(other.Degrees);
            if (angle2 > angle1)
            {
                return Angle.FromDegrees(angle2 - angle1);
            }
            return Angle.FromDegrees((360f - angle1) + angle2);
        }

        #region Convertion

        public static float ToDegrees(float radians)
        {
            return (radians * 180.0f) / (float)Math.PI;
        }

        public static float ToRadians(float degrees)
        {
            return (float)Math.PI * degrees / 180.0f;
        }

        #endregion

        public static float NormalizeDegrees(float degrees)
        {
            degrees = degrees % 360f;
            if (degrees < 0f)
                degrees += 360f;
            return degrees;
        }

        public static float NormalizeRadians(float radians)
        {
            radians = radians % (PI * 2f);
            if (radians < 0f)
                radians += PI * 2f;
            return radians;
        }

        public override string ToString()
        {
            if (DefaultConvertionUnit == AngleUnit.Degrees)
                return string.Format("{0}°", Degrees);
            else
                return string.Format("{0}ᶜ", Radians);
        }
    }
}
