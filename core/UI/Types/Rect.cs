using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public struct Rect
    {
        public static Rect One = new Rect(1, 1, 1, 1);

        public static Rect Zero = new Rect(0, 0, 0, 0);

        public float left;
        public float right;
        public float top;
        public float bottom;

        public Rect(float left, float top, float right, float bottom)
        {
            this.left = left;
            this.right = right;
            this.top = top;
            this.bottom = bottom;
        }
    }
}
