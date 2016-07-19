using System;

namespace Poly3D.Maths
{
    public struct Angle
    {
        public static readonly Poly3D.Maths.Angle Zero = new Poly3D.Maths.Angle();
        public static AngleUnit DefaultConvertionUnit = AngleUnit.Degrees;

        private double angleDeg;

        public double Degrees
        {
            get { return angleDeg; }
            set
            {
                angleDeg = value;
            }
        }

        public double Radians
        {
            get { return ToRadians(Degrees); }
            set
            {
                angleDeg = ToDegrees(value);
            }
        }

        #region Static Ctors

        public static Poly3D.Maths.Angle FromDegrees(double degrees)
        {
            return new Poly3D.Maths.Angle { Degrees = degrees };
        }

        public static Poly3D.Maths.Angle FromRadians(double radians)
        {
            return new Poly3D.Maths.Angle { Radians = radians };
        }

        #endregion

        #region Operators

        public static explicit operator double(Poly3D.Maths.Angle angle)
        {
            return DefaultConvertionUnit == AngleUnit.Degrees ? angle.Degrees : angle.Radians;
        }

        public static explicit operator Poly3D.Maths.Angle(double angle)
        {
            return DefaultConvertionUnit == AngleUnit.Degrees ? FromDegrees(angle) : FromRadians(angle);
        }

        public static Poly3D.Maths.Angle operator +(Poly3D.Maths.Angle a1, Poly3D.Maths.Angle a2)
        {
            return FromDegrees(a1.Degrees + a2.Degrees);
        }

        public static Poly3D.Maths.Angle operator -(Poly3D.Maths.Angle a1, Poly3D.Maths.Angle a2)
        {
            return FromDegrees(a1.Degrees - a2.Degrees);
        }

        public static Poly3D.Maths.Angle operator *(Poly3D.Maths.Angle a1, double value)
        {
            return FromDegrees(a1.Degrees * value);
        }

        public static Poly3D.Maths.Angle operator /(Poly3D.Maths.Angle a1, double value)
        {
            return FromDegrees(a1.Degrees / value);
        }

        public static Poly3D.Maths.Angle operator %(Poly3D.Maths.Angle a1, double value)
        {
            return FromDegrees(a1.Degrees % value);
        }

        public static bool operator ==(Poly3D.Maths.Angle a1, Poly3D.Maths.Angle a2)
        {
            return a1.Degrees == a2.Degrees;
        }

        public static bool operator !=(Poly3D.Maths.Angle a1, Poly3D.Maths.Angle a2)
        {
            return a1.Degrees != a2.Degrees;
        }

        public static bool operator >(Poly3D.Maths.Angle a1, Poly3D.Maths.Angle a2)
        {
            return a1.Degrees > a2.Degrees;
        }

        public static bool operator <(Poly3D.Maths.Angle a1, Poly3D.Maths.Angle a2)
        {
            return a1.Degrees < a2.Degrees;
        }

        public static bool operator >=(Poly3D.Maths.Angle a1, Poly3D.Maths.Angle a2)
        {
            return a1.Degrees >= a2.Degrees;
        }

        public static bool operator <=(Poly3D.Maths.Angle a1, Poly3D.Maths.Angle a2)
        {
            return a1.Degrees <= a2.Degrees;
        }

        public override bool Equals(object obj)
        {
            if (obj is Poly3D.Maths.Angle)
                return Degrees.Equals(((Poly3D.Maths.Angle)obj).Degrees);
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
        public void Clamp()
        {
            angleDeg = ClampDegrees(angleDeg);
        }

        public Poly3D.Maths.Angle Clamped()
        {
            return Poly3D.Maths.Angle.FromDegrees(ClampDegrees(angleDeg));
        }

        //assuming this is the minimum
        public Poly3D.Maths.Angle Diff(Poly3D.Maths.Angle other)//clockwise
        {
            var angle1 = ClampDegrees(Degrees);
            var angle2 = ClampDegrees(other.Degrees);
            if (angle2 > angle1)
            {
                return Poly3D.Maths.Angle.FromDegrees(angle2 - angle1);
            }
            return Poly3D.Maths.Angle.FromDegrees((360d - angle1) + angle2);
        }

        #region Convertion

        public static double ToDegrees(double radians)
        {
            return (radians * 180.0d) / Math.PI;
        }

        public static double ToRadians(double degrees)
        {
            return Math.PI * degrees / 180.0d;
        }

        #endregion

        public static double ClampDegrees(double degrees)
        {
            degrees = degrees % 360d;
            if (degrees < 0)
                degrees += 360d;
            return degrees;
        }
    }
}
