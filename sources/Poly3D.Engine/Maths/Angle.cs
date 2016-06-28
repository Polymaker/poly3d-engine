using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Engine.Maths
{
    public struct Angle
    {
        public static readonly Angle Zero = new Angle();
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

        public static Angle FromDegrees(double degrees)
        {
            return new Angle { Degrees = degrees };
        }

        public static Angle FromRadians(double radians)
        {
            return new Angle { Radians = radians };
        }

        #endregion

        #region Operators

        public static explicit operator double(Angle angle)
        {
            return DefaultConvertionUnit == AngleUnit.Degrees ? angle.Degrees : angle.Radians;
        }

        public static explicit operator Angle(double angle)
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

        public static Angle operator *(Angle a1, double value)
        {
            return FromDegrees(a1.Degrees * value);
        }

        public static Angle operator /(Angle a1, double value)
        {
            return FromDegrees(a1.Degrees / value);
        }

        public static Angle operator %(Angle a1, double value)
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
        public void Clamp()
        {
            angleDeg = ClampDegrees(angleDeg);
        }

        public Angle Clamped()
        {
            return Angle.FromDegrees(ClampDegrees(angleDeg));
        }

        //assuming this is the minimum
        public Angle Diff(Angle other)//clockwise
        {
            var angle1 = ClampDegrees(Degrees);
            var angle2 = ClampDegrees(other.Degrees);
            if (angle2 > angle1)
            {
                return Angle.FromDegrees(angle2 - angle1);
            }
            return Angle.FromDegrees((360d - angle1) + angle2);
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
