using Poly3D.Maths;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Graphics
{
    public struct ColorHSV : IEquatable<ColorHSV>
    {
        #region Fields

        private float _A;
        private float _V;
        private float _S;
        private float _H;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the alpha component value of this color.
        /// </summary>
        public float A
        {
            get { return _A; }
            set
            {
                _A = value.Clamp();
            }
        }

        /// <summary>
        /// Gets or sets the hue component value of this color.
        /// </summary>
        public float H
        {
            get { return _H; }
            set
            {
                _H = value.Clamp();
            }
        }

        /// <summary>
        /// Gets or sets the saturation component value of this color.
        /// </summary>
        public float S
        {
            get { return _S; }
            set
            {
                _S = value.Clamp();
            }
        }

        /// <summary>
        /// Gets or sets the luminosity component value of this color.
        /// </summary>
        public float V
        {
            get { return _V; }
            set
            {
                _V = value.Clamp();
            }
        }

        /// <summary>
        /// Gets or sets the hue component value, in degrees, of this color.
        /// </summary>
        public float Hue
        {
            get { return _H * 360f; }
            set
            {
                var clamped = (float)Angle.ClampDegrees(value);
                if (clamped == 0 && value >= 360)
                    clamped = 360f;
                _H = clamped / 360f;
            }
        }
        public bool IsEmpty
        {
            get { return object.ReferenceEquals(this, Empty); }
        }

        #endregion

        #region Static constructor

        public static ColorHSV FromAhsv(float h, float s, float v)
        {
            return new ColorHSV()
            {
                A = 1,
                H = h,
                S = s,
                V = v
            };
        }

        public static ColorHSV FromAhsv(float a, float h, float s, float v)
        {
            return new ColorHSV()
            {
                A = a,
                H = h,
                S = s,
                V = v
            };
        }

        #endregion

        public int ToAhsv()
        {
            return (int)((uint)(A * 255f) << 24 | (uint)(H * 360f) << 16 | (uint)(S * 100f) << 8 | (uint)(V * 100f));
        }

        #region Operators

        public override int GetHashCode()
        {
            if (IsEmpty)
                return -1;
            return ToAhsv();
        }

        public override bool Equals(object obj)
        {
            return obj is ColorHSV && Equals((ColorHSV)obj);
        }

        public bool Equals(ColorHSV other)
        {
            if (IsEmpty || other.IsEmpty)
                return IsEmpty && other.IsEmpty;

            return H == other.H && S == other.S && V == other.V && A == other.A;
        }

        public static bool operator ==(ColorHSV left, ColorHSV right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(ColorHSV left, ColorHSV right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Convertion operators

        public static implicit operator ColorHSV(System.Drawing.Color color)
        {
            return ColorConverter.RgbToHsv(color);
        }

        public static implicit operator ColorHSV(Color color)
        {
            return ColorConverter.RgbToHsv(color);
        }

        public static explicit operator System.Drawing.Color(ColorHSV color)
        {
            return (System.Drawing.Color)ColorConverter.HsvToRgb(color);
        }

        #endregion

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            stringBuilder.Append(base.GetType().Name);
            stringBuilder.Append(" [");
            if (this != Empty)
            {
                stringBuilder.Append("A=");
                stringBuilder.Append(A);
                stringBuilder.Append(", H=");
                stringBuilder.Append(H);
                stringBuilder.Append(", S=");
                stringBuilder.Append(S);
                stringBuilder.Append(", V=");
                stringBuilder.Append(V);
            }
            else
            {
                stringBuilder.Append("Empty");
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        public static readonly ColorHSV Empty = default(ColorHSV);
    }
}
