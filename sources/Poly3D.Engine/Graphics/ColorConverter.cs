using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Poly3D.Graphics
{
    public static class ColorConverter
    {

        #region HSV <-> RGB

        public static ColorHSV RgbToHsv(Color color)
        {
            if (color == Color.Empty)
                return ColorHSV.Empty;
            var var_R = color.R;
            var var_G = color.G;
            var var_B = color.B;

            var var_Min = Math.Min(var_R, Math.Min(var_G, var_B)); //Min. value of RGB
            var var_Max = Math.Max(var_R, Math.Max(var_G, var_B)); //Max. value of RGB
            var del_Max = var_Max - var_Min; //Delta RGB value 

            var V = var_Max;
            float H = 0, S = 0;

            if (del_Max == 0) //This is a gray, no chroma...
            {
                H = 0; //HSV results from 0 to 1
                S = 0;
            }
            else //Chromatic data...
            {
                S = del_Max / var_Max;
                var del_R = (((var_Max - var_R) / 6f) + (del_Max / 2f)) / del_Max;
                var del_G = (((var_Max - var_G) / 6f) + (del_Max / 2f)) / del_Max;
                var del_B = (((var_Max - var_B) / 6f) + (del_Max / 2f)) / del_Max;

                if (var_R == var_Max) H = del_B - del_G;
                else if (var_G == var_Max) H = (1f / 3f) + del_R - del_B;
                else if (var_B == var_Max) H = (2f / 3f) + del_G - del_R;

                if (H < 0) H += 1;
                if (H > 1) H -= 1;
            }
            return ColorHSV.FromAhsv(color.A, H, S, V);
        }

        public static Color HsvToRgb(ColorHSV color)
        {
            if (color == ColorHSV.Empty)
                return Color.Empty;

            if (color.V == 0)
            {
                return Color.FromArgb(color.A, color.V, color.V, color.V);
            }

            var var_h = (color.H * 6f) % 6f;
            var var_i = (int)Math.Floor(var_h);             //Or ... var_i = floor( var_h )
            var var_1 = color.V * (1f - color.S);
            var var_2 = color.V * (1f - color.S * (var_h - var_i));
            var var_3 = color.V * (1f - color.S * (1f - (var_h - var_i)));
            float var_r = 0, var_g = 0, var_b = 0;
            if (var_i == 0) { var_r = color.V; var_g = var_3; var_b = var_1; }
            else if (var_i == 1) { var_r = var_2; var_g = color.V; var_b = var_1; }
            else if (var_i == 2) { var_r = var_1; var_g = color.V; var_b = var_3; }
            else if (var_i == 3) { var_r = var_1; var_g = var_2; var_b = color.V; }
            else if (var_i == 4) { var_r = var_3; var_g = var_1; var_b = color.V; }
            else { var_r = color.V; var_g = var_1; var_b = var_2; }

            return Color.FromArgb(color.A, var_r, var_g, var_b);
        }

        #endregion
    }
}
