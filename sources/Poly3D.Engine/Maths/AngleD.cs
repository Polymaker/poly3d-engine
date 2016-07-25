using System;

namespace Poly3D.Maths
{
    public struct AngleD
    {
        public static readonly AngleD Zero = new AngleD();
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

        public static AngleD FromDegrees(double degrees)
        {
            return new AngleD { Degrees = degrees };
        }

        public static AngleD FromRadians(double radians)
        {
            return new AngleD { Radians = radians };
        }

        #endregion

        #region Operators

        public static explicit operator double(AngleD angle)
        {
            return DefaultConvertionUnit == AngleUnit.Degrees ? angle.Degrees : angle.Radians;
        }

        public static explicit operator AngleD(double angle)
        {
            return DefaultConvertionUnit == AngleUnit.Degrees ? FromDegrees(angle) : FromRadians(angle);
        }

        public static AngleD operator +(AngleD a1, AngleD a2)
        {
            return FromDegrees(a1.Degrees + a2.Degrees);
        }

        public static AngleD operator -(AngleD a1, AngleD a2)
        {
            return FromDegrees(a1.Degrees - a2.Degrees);
        }

        public static AngleD operator *(AngleD a1, double value)
        {
            return FromDegrees(a1.Degrees * value);
        }

        public static AngleD operator /(AngleD a1, double value)
        {
            return FromDegrees(a1.Degrees / value);
        }

        public static AngleD operator %(AngleD a1, double value)
        {
            return FromDegrees(a1.Degrees % value);
        }

        public static bool operator ==(AngleD a1, AngleD a2)
        {
            return a1.Degrees == a2.Degrees;
        }

        public static bool operator !=(AngleD a1, AngleD a2)
        {
            return a1.Degrees != a2.Degrees;
        }

        public static bool operator >(AngleD a1, AngleD a2)
        {
            return a1.Degrees > a2.Degrees;
        }

        public static bool operator <(AngleD a1, AngleD a2)
        {
            return a1.Degrees < a2.Degrees;
        }

        public static bool operator >=(AngleD a1, AngleD a2)
        {
            return a1.Degrees >= a2.Degrees;
        }

        public static bool operator <=(AngleD a1, AngleD a2)
        {
            return a1.Degrees <= a2.Degrees;
        }

        public override bool Equals(object obj)
        {
            if (obj is AngleD)
                return Degrees.Equals(((AngleD)obj).Degrees);
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

        public AngleD Clamped()
        {
            return AngleD.FromDegrees(ClampDegrees(angleDeg));
        }

        //assuming this is the minimum
        public AngleD Diff(AngleD other)//clockwise
        {
            var angle1 = ClampDegrees(Degrees);
            var angle2 = ClampDegrees(other.Degrees);
            if (angle2 > angle1)
            {
                return AngleD.FromDegrees(angle2 - angle1);
            }
            return AngleD.FromDegrees((360d - angle1) + angle2);
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

        public override string ToString()
        {
            if (DefaultConvertionUnit == AngleUnit.Degrees)
                return string.Format("{0}°", Degrees);
            else
                return string.Format("{0}ᶜ", Radians);
        }
    }
}