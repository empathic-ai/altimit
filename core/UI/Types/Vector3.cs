using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    [AType]
    public struct Vector3
    {
        public static Vector3 One = new Vector3(1, 1, 1);

        public static Vector3 Zero = new Vector3(0, 0, 0);

        public static Vector3 Up = new Vector3(0, 1, 0);

        [AProperty]
        public float x { get; set; }
        [AProperty]
        public float y { get; set; }
        [AProperty]
        public float z { get; set; }

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

        public static Vector3 operator +(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x+b.x, a.y + b.y, a.z + b.z);
        }
        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }
        
        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z;
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !(a == b);
        }

        public static Vector3 Cross(Vector3 a, Vector3 b)
        {
            return new Vector3(a.y * b.z - a.z * b.y, a.z * b.x - a.x * b.z, a.x * b.y - a.y * b.x);
        }

#if GODOT
        public static explicit operator Godot.Vector3(Vector3 v)
        {
            return new Godot.Vector3(v.x, v.y, v.z);
        }

        public static explicit operator Vector3(Godot.Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }
#elif UNITY
        public static explicit operator Vector3(UnityEngine.Vector3 v)
        {
            return new Vector3(v.x, v.y, v.z);
        }

        public static implicit operator UnityEngine.Vector3(Vector3 v)
        {
            return new UnityEngine.Vector3(v.x, v.y, v.z);
        }
#endif
    }
}
