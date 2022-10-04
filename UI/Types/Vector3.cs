using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public struct Vector3
    {
        public static Vector3 up = new Vector3(0, 1, 0);

        public float x;
        public float y;
        public float z;

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator *(Vector3 v, float num)
        {
            return new Vector3(v.x * num, v.y * num, v.z * num);
        }

    }
}
