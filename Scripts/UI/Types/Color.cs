using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public class Color
    {
        public static Color white => new Color(1, 1, 1, 1);
        public static Color black => new Color(0, 0, 0, 1);
        public static Color blue => new Color(66.0f/255.0f, 133.0f/ 255.0f, 244.0f/ 255.0f, 1);

        public float r;
        public float b;
        public float g;
        public float a;

        public Color(float r, float g, float b, float a)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

#if UNITY
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
