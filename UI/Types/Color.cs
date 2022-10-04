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
    }
}
