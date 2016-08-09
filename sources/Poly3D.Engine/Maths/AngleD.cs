using System;

namespace Poly3D.Maths
{
    public struct AngleD
    {
        public static readonly AngleD Zero = new AngleD();
        public static AngleUnit DefaultConvertionUnit = AngleUnit.Degrees;

        private double _Degrees;

        public double Degrees
        {
            get { return _Degrees; }
            set
            {
                if (ReferenceEquals(this, Zero))
                    return;
                _Degrees = value;
            }
        }

        public double Radians
        {
            get { return ToRadians(Degrees); }
            set
            {
                if (ReferenceEquals(this, Zero))
                    return;
                _Degrees = ToDegrees(value);
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
            return _Degrees.GetHashCode();
        }

        #endregion

        /// <summary>
        /// Clamps the angle between 0-360
        /// </summary>
        public void Clamp()
        {
            _Degrees = ClampDegrees(_Degrees);
        }

        public AngleD Clamped()
        {
            return AngleD.FromDegrees(ClampDegrees(_Degrees));
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