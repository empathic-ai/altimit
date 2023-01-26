using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public static class ColorExtensions
    {
        public static Color SetAlpha(this Color c, float num)
        {
            return new Color(c.r, c.g, c.b, num);
        }

        public static Color SetBrightness(this Color c, float num)
        {
            return new Color(c.r*num, c.g*num, c.b*num, c.a);
        }
    }

    public class Color
    {
        public static Color White => new Color(1, 1, 1, 1);
        public static Color Black => new Color(0, 0, 0, 1);
        public static Color grey => new Color(.5f, .5f, .5f, 1);
        public static Color blue => new Color(66.0f/255.0f, 133.0f/ 255.0f, 244.0f/ 255.0f, 1);

        public float r;
        public float b;
        public float g;
        public float a;

        public Color(float r, float g, float b, float a = 1)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public Color(string hexCode)
        {
            if (hexCode.StartsWith("#"))
                hexCode = hexCode.Substring(1);

            this.r = int.Parse(hexCode.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
            this.g = int.Parse(hexCode.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
            this.b = int.Parse(hexCode.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255.0f;
            this.a = 1;
        }


#if UNITY_64
        public static explicit operator Color(UnityEngine.Color c)
        {
            return new Color(c.r, c.g, c.b, c.a);
        }

        public static implicit operator UnityEngine.Color(Color c)
        {
            return new UnityEngine.Color(c.r, c.g, c.b, c.a);
        }
#elif GODOT
        public static explicit operator Color(Godot.Color c)
        {
            return new Color(c.r, c.g, c.b, c.a);
        }

        public static implicit operator Godot.Color(Color c)
        {
            return new Godot.Color(c.r, c.g, c.b, c.a);
        }
#endif
    }
}
