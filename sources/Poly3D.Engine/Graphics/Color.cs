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

        public static implicit operator OpenTK.Graphics.Color4(Color color)
        {
            return System.Drawing.Color.FromArgb((int)(color.A * 255f), (int)(color.R * 255f), (int)(color.G * 255f), (int)(color.B * 255f));
        }

        public static implicit operator Color(OpenTK.Graphics.Color4 color)
        {
            return new Color(color.A, color.R, color.G, color.B);
        }

        public static explicit operator OpenTK.Vector3(Color color)
        {
            return new OpenTK.Vector3(color.R, color.G, color.B);
        }

        public static explicit operator OpenTK.Vector4(Color color)
        {
            return new OpenTK.Vector4(color.A, color.R, color.G, color.B);
        }

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

        private static Color GetKnownColor(System.Drawing.KnownColor color)
        {
            return (Color)System.Drawing.Color.FromKnownColor(color);
        }

        public static Color AliceBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.AliceBlue); }
        }

        public static Color AntiqueWhite
        {
            get { return GetKnownColor(System.Drawing.KnownColor.AntiqueWhite); }
        }

        public static Color Aqua
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Aqua); }
        }

        public static Color Aquamarine
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Aquamarine); }
        }

        public static Color Azure
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Azure); }
        }

        public static Color Beige
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Beige); }
        }

        public static Color Bisque
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Bisque); }
        }

        public static Color Black
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Black); }
        }

        public static Color BlanchedAlmond
        {
            get { return GetKnownColor(System.Drawing.KnownColor.BlanchedAlmond); }
        }

        public static Color Blue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Blue); }
        }

        public static Color BlueViolet
        {
            get { return GetKnownColor(System.Drawing.KnownColor.BlueViolet); }
        }

        public static Color Brown
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Brown); }
        }

        public static Color BurlyWood
        {
            get { return GetKnownColor(System.Drawing.KnownColor.BurlyWood); }
        }

        public static Color CadetBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.CadetBlue); }
        }

        public static Color Chartreuse
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Chartreuse); }
        }

        public static Color Chocolate
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Chocolate); }
        }

        public static Color Coral
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Coral); }
        }

        public static Color CornflowerBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.CornflowerBlue); }
        }

        public static Color Cornsilk
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Cornsilk); }
        }

        public static Color Crimson
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Crimson); }
        }

        public static Color Cyan
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Cyan); }
        }

        public static Color DarkBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkBlue); }
        }

        public static Color DarkCyan
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkCyan); }
        }

        public static Color DarkGoldenrod
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkGoldenrod); }
        }

        public static Color DarkGray
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkGray); }
        }

        public static Color DarkGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkGreen); }
        }

        public static Color DarkKhaki
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkKhaki); }
        }

        public static Color DarkMagenta
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkMagenta); }
        }

        public static Color DarkOliveGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkOliveGreen); }
        }

        public static Color DarkOrange
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkOrange); }
        }

        public static Color DarkOrchid
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkOrchid); }
        }

        public static Color DarkRed
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkRed); }
        }

        public static Color DarkSalmon
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkSalmon); }
        }

        public static Color DarkSeaGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkSeaGreen); }
        }

        public static Color DarkSlateBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkSlateBlue); }
        }

        public static Color DarkSlateGray
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkSlateGray); }
        }

        public static Color DarkTurquoise
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkTurquoise); }
        }

        public static Color DarkViolet
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DarkViolet); }
        }

        public static Color DeepPink
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DeepPink); }
        }

        public static Color DeepSkyBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DeepSkyBlue); }
        }

        public static Color DimGray
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DimGray); }
        }

        public static Color DodgerBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.DodgerBlue); }
        }

        public static Color Firebrick
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Firebrick); }
        }

        public static Color FloralWhite
        {
            get { return GetKnownColor(System.Drawing.KnownColor.FloralWhite); }
        }

        public static Color ForestGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.ForestGreen); }
        }

        public static Color Fuchsia
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Fuchsia); }
        }

        public static Color Gainsboro
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Gainsboro); }
        }

        public static Color GhostWhite
        {
            get { return GetKnownColor(System.Drawing.KnownColor.GhostWhite); }
        }

        public static Color Gold
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Gold); }
        }

        public static Color Goldenrod
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Goldenrod); }
        }

        public static Color Gray
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Gray); }
        }

        public static Color Green
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Green); }
        }

        public static Color GreenYellow
        {
            get { return GetKnownColor(System.Drawing.KnownColor.GreenYellow); }
        }

        public static Color Honeydew
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Honeydew); }
        }

        public static Color HotPink
        {
            get { return GetKnownColor(System.Drawing.KnownColor.HotPink); }
        }

        public static Color IndianRed
        {
            get { return GetKnownColor(System.Drawing.KnownColor.IndianRed); }
        }

        public static Color Indigo
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Indigo); }
        }

        public static Color Ivory
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Ivory); }
        }

        public static Color Khaki
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Khaki); }
        }

        public static Color Lavender
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Lavender); }
        }

        public static Color LavenderBlush
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LavenderBlush); }
        }

        public static Color LawnGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LawnGreen); }
        }

        public static Color LemonChiffon
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LemonChiffon); }
        }

        public static Color LightBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightBlue); }
        }

        public static Color LightCoral
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightCoral); }
        }

        public static Color LightCyan
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightCyan); }
        }

        public static Color LightGoldenrodYellow
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightGoldenrodYellow); }
        }

        public static Color LightGray
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightGray); }
        }

        public static Color LightGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightGreen); }
        }

        public static Color LightPink
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightPink); }
        }

        public static Color LightSalmon
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightSalmon); }
        }

        public static Color LightSeaGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightSeaGreen); }
        }

        public static Color LightSkyBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightSkyBlue); }
        }

        public static Color LightSlateGray
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightSlateGray); }
        }

        public static Color LightSteelBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightSteelBlue); }
        }

        public static Color LightYellow
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LightYellow); }
        }

        public static Color Lime
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Lime); }
        }

        public static Color LimeGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.LimeGreen); }
        }

        public static Color Linen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Linen); }
        }

        public static Color Magenta
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Magenta); }
        }

        public static Color Maroon
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Maroon); }
        }

        public static Color MediumAquamarine
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumAquamarine); }
        }

        public static Color MediumBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumBlue); }
        }

        public static Color MediumOrchid
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumOrchid); }
        }

        public static Color MediumPurple
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumPurple); }
        }

        public static Color MediumSeaGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumSeaGreen); }
        }

        public static Color MediumSlateBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumSlateBlue); }
        }

        public static Color MediumSpringGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumSpringGreen); }
        }

        public static Color MediumTurquoise
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumTurquoise); }
        }

        public static Color MediumVioletRed
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MediumVioletRed); }
        }

        public static Color MidnightBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MidnightBlue); }
        }

        public static Color MintCream
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MintCream); }
        }

        public static Color MistyRose
        {
            get { return GetKnownColor(System.Drawing.KnownColor.MistyRose); }
        }

        public static Color Moccasin
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Moccasin); }
        }

        public static Color NavajoWhite
        {
            get { return GetKnownColor(System.Drawing.KnownColor.NavajoWhite); }
        }

        public static Color Navy
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Navy); }
        }

        public static Color OldLace
        {
            get { return GetKnownColor(System.Drawing.KnownColor.OldLace); }
        }

        public static Color Olive
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Olive); }
        }

        public static Color OliveDrab
        {
            get { return GetKnownColor(System.Drawing.KnownColor.OliveDrab); }
        }

        public static Color Orange
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Orange); }
        }

        public static Color OrangeRed
        {
            get { return GetKnownColor(System.Drawing.KnownColor.OrangeRed); }
        }

        public static Color Orchid
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Orchid); }
        }

        public static Color PaleGoldenrod
        {
            get { return GetKnownColor(System.Drawing.KnownColor.PaleGoldenrod); }
        }

        public static Color PaleGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.PaleGreen); }
        }

        public static Color PaleTurquoise
        {
            get { return GetKnownColor(System.Drawing.KnownColor.PaleTurquoise); }
        }

        public static Color PaleVioletRed
        {
            get { return GetKnownColor(System.Drawing.KnownColor.PaleVioletRed); }
        }

        public static Color PapayaWhip
        {
            get { return GetKnownColor(System.Drawing.KnownColor.PapayaWhip); }
        }

        public static Color PeachPuff
        {
            get { return GetKnownColor(System.Drawing.KnownColor.PeachPuff); }
        }

        public static Color Peru
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Peru); }
        }

        public static Color Pink
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Pink); }
        }

        public static Color Plum
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Plum); }
        }

        public static Color PowderBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.PowderBlue); }
        }

        public static Color Purple
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Purple); }
        }

        public static Color Red
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Red); }
        }

        public static Color RosyBrown
        {
            get { return GetKnownColor(System.Drawing.KnownColor.RosyBrown); }
        }

        public static Color RoyalBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.RoyalBlue); }
        }

        public static Color SaddleBrown
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SaddleBrown); }
        }

        public static Color Salmon
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Salmon); }
        }

        public static Color SandyBrown
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SandyBrown); }
        }

        public static Color SeaGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SeaGreen); }
        }

        public static Color SeaShell
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SeaShell); }
        }

        public static Color Sienna
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Sienna); }
        }

        public static Color Silver
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Silver); }
        }

        public static Color SkyBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SkyBlue); }
        }

        public static Color SlateBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SlateBlue); }
        }

        public static Color SlateGray
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SlateGray); }
        }

        public static Color Snow
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Snow); }
        }

        public static Color SpringGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SpringGreen); }
        }

        public static Color SteelBlue
        {
            get { return GetKnownColor(System.Drawing.KnownColor.SteelBlue); }
        }

        public static Color Tan
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Tan); }
        }

        public static Color Teal
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Teal); }
        }

        public static Color Thistle
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Thistle); }
        }

        public static Color Tomato
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Tomato); }
        }

        public static Color Turquoise
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Turquoise); }
        }

        public static Color Violet
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Violet); }
        }

        public static Color Wheat
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Wheat); }
        }

        public static Color White
        {
            get { return GetKnownColor(System.Drawing.KnownColor.White); }
        }

        public static Color WhiteSmoke
        {
            get { return GetKnownColor(System.Drawing.KnownColor.WhiteSmoke); }
        }

        public static Color Yellow
        {
            get { return GetKnownColor(System.Drawing.KnownColor.Yellow); }
        }

        public static Color YellowGreen
        {
            get { return GetKnownColor(System.Drawing.KnownColor.YellowGreen); }
        }

        #endregion
    }
}
