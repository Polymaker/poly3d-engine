using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public static class NumberExtensions
    {
        #region Clamping

        /// <summary>
        /// Clamps a value between 0.0 and 1.0.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>A value between 0.0 and 1.0</returns>
        public static double Clamp(this double value)
        {
            return Clamp(value, 0, 1);
        }

        /// <summary>
        /// Clamps a value to the specified range.
        /// </summary>
        /// <param name="value">The value</param>
        /// <param name="min">The minimum value</param>
        /// <param name="max">The maximim value</param>
        /// <returns>A value between <see cref="min"/> and <see cref="max"/></returns>
        public static double Clamp(this double value, double min, double max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        public static double ClampMod(this double value)
        {
            return ClampMod(value, 0, 1);
        }

        public static double ClampMod(this double value, double min, double max)
        {
            return (value - min) % (max - min + 1) + min;
        }

        public static float Clamp(this float value)
        {
            return Clamp(value, 0, 1);
        }

        public static float Clamp(this float value, float min, float max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        public static float ClampMod(this float value)
        {
            return ClampMod(value, 0, 1);
        }

        public static float ClampMod(this float value, float min, float max)
        {
            return (value - min) % (max - min + 1) + min;
        }

        #endregion
    }
}
