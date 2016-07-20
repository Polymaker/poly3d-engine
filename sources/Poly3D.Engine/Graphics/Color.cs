using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
namespace Poly3D.Graphics
{
    [Serializable]
    public struct Color : IEquatable<Color>
    {

        private float _A;
        private float _R;
        private float _G;
        private float _B;

        public float A
        {
            get { return _A; }
            set
            {
                _A = value.Clamp();
            }
        }

        public float R
        {
            get { return _R; }
            set
            {
                _R = value.Clamp();
            }
        }

        public float G
        {
            get { return _G; }
            set
            {
                _G = value.Clamp();
            }
        }

        public float B
        {
            get { return _B; }
            set
            {
                _B = value.Clamp();
            }
        }

        public bool IsEmpty
        {
            get { return object.ReferenceEquals(this, Empty); }
        }

        public Color(float a, float r, float g, float b)
        {
            _A = a;
            _R = r;
            _G = g;
            _B = b;
        }

        public Color(byte a, byte r, byte g, byte b)
        {
            _R = (float)r / 255f;
            _G = (float)g / 255f;
            _B = (float)b / 255f;
            _A = (float)a / 255f;
        }

        #region Static constructor


        /// <summary>
        /// Creates a Color structure from the specified float values (RGB).
        /// </summary>
        /// <param name="r">Red</param>
        /// <param name="g">Green</param>
        /// <param name="b">Blue</param>
        /// <returns>A ColorF structure</returns>
        public static Color FromArgb(float r, float g, float b)
        {
            return FromArgb(1f, r, g, b);
        }

        /// <summary>
        /// Creates a Color structure from the from the four ARGB component.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Color FromArgb(float a, float r, float g, float b)
        {
            return new Color(a,r,g,b);
        }

        public static Color FromArgb(byte r, byte g, byte b)
        {
            return FromArgb(255, r, g, b);
        }

        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(
                (float)a / 255f,
                (float)r / 255f,
                (float)g / 255f,
                (float)b / 255f
            );
        }

        //public static ColorF FromArgb(int argb)
        //{
        //    return (ColorF)(System.Drawing.Color.FromArgb(argb));
        //}

        #endregion

        public int ToArgb()
        {
            return (int)((uint)(A * 255f) << 24 | (uint)(R * 255f) << 16 | (uint)(G * 255f) << 8 | (uint)(B * 255f));
        }

        #region Equation operators

        public override int GetHashCode()
        {
            if (IsEmpty)
                return -1;
            return ToArgb();
        }

        public override bool Equals(object obj)
        {
            return obj is Color && Equals((Color)obj);
        }

        public bool Equals(Color other)
        {
            if (IsEmpty || other.IsEmpty)
                return IsEmpty && other.IsEmpty;

            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        public static bool operator ==(Color left, Color right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Color left, Color right)
        {
            return !left.Equals(right);
        }

        #endregion

        #region Convertion operators

        public static implicit operator Color(System.Drawing.Color color)
        {
            return new Color(color.A, color.R, color.G, color.B);
        }

        public static implicit operator Color(ColorHSV color)
        {
            return ColorConverter.HsvToRgb(color);
        }

        public static explicit operator System.Drawing.Color(Color color)
        {
            return System.Drawing.Color.FromArgb((int)(color.A * 255f), (int)(color.R * 255f), (int)(color.G * 255f), (int)(color.B * 255f));
        }

        #endregion

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder(32);
            stringBuilder.Append(base.GetType().Name);
            stringBuilder.Append(" [");
            if (!IsEmpty)
            {
                stringBuilder.Append("A=");
                stringBuilder.Append(A);
                stringBuilder.Append(", R=");
                stringBuilder.Append(R);
                stringBuilder.Append(", G=");
                stringBuilder.Append(G);
                stringBuilder.Append(", B=");
                stringBuilder.Append(B);
            }
            else
            {
                stringBuilder.Append("Empty");
            }
            stringBuilder.Append("]");
            return stringBuilder.ToString();
        }

        #region pre-defined colors

        public static readonly Color Empty = default(Color);

        #endregion
    }
}
