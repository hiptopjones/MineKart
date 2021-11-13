using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SdlEngine
{
    public static class Utilities
    {
        public static double Deg2Rad(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        public static double Clamp(double value, double min, double max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static int Clamp(int value, int min, int max)
        {
            return Math.Min(Math.Max(value, min), max);
        }

        public static bool InRange(double value, double minValue, double maxValue)
        {
            return value >= minValue && value <= maxValue;
        }

        public static bool InRange(int value, int minValue, int maxValue)
        {
            return value >= minValue && value <= maxValue;
        }

        public static double Lerp(double start, double end, double percent)
        {
            return start + (end - start) * percent;
        }

    }
}
