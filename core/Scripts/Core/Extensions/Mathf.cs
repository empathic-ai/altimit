using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit
{
    public static class Mathf
    {
        public static float Deg2Rad => (float)((Math.PI * 2) / 360);
        public static float Rad2Deg => (float)(360 / (Math.PI*2));

        public static float Sqrt(float f)
        {
            return (float)Math.Sqrt(f);
        }

        public static float Abs(float f)
        {
            return (float)Math.Abs(f);
        }

        public static float Max(float a, float b)
        {
            return (float)Math.Max(a, b);
        }

        public static float Atan2(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }
        public static float Cos(float d)
        {
            return (float)Math.Cos(d);
        }
        public static float Sin(float d)
        {
            return (float)Math.Sin(d);
        }

        public static float Sign(float f)
        {
            return (float)Math.Sign(f);
        }
        public static float Acos(float value)
        {
            return (float)Math.Acos(value);
        }

    }
}
