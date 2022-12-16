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

        public float r;
        public float b;
        public float g;
        public float a;

        public Color(float r, float b, float g, float a)
        {
            this.r = r;
            this.b = b;
            this.g = g;
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
#endif
    }
}
