using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public struct Vector4
    {
        public static Vector4 One = new Vector4(1, 1, 1, 1);

        public static Vector4 Zero = new Vector4(0, 0, 0, 0);


        public float x;
        public float y;
        public float z;
        public float w;

        public Vector4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }
    }
}
