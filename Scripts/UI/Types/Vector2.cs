using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    [AType]
    public struct Vector2
    {
        public static Vector2 Zero = new Vector2(0, 0);

        public static Vector2 One = new Vector2(1, 1);

        [AProperty]
        public float x { get; set; }

        [AProperty]
        public float y { get; set; }

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }


        public static Vector2 operator *(Vector2 v, float num)
        {
            return new Vector2(v.x * num, v.y * num);
        }

        public static Vector2 operator -(Vector2 v)
        {
            return new Vector2(-v.x, -v.y);
        }

        public static Vector2 operator -(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x-b.x, a.y-b.y);
        }
        public static Vector2 operator +(Vector2 a, Vector2 b)
        {
            return new Vector2(a.x + b.x, a.y + b.y);
        }

        public static bool operator ==(Vector2 a, Vector2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Vector2 a, Vector2 b)
        {
            return !(a == b);
        }

#if UNITY
        public static explicit operator Vector2(UnityEngine.Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static implicit operator UnityEngine.Vector2(Vector2 v)
        {
            return new UnityEngine.Vector2(v.x, v.y);
        }
#elif GODOT
        public static explicit operator Vector2(Godot.Vector2 v)
        {
            return new Vector2(v.x, v.y);
        }

        public static implicit operator Godot.Vector2(Vector2 v)
        {
            return new Godot.Vector2(v.x, v.y);
        }
        public static explicit operator Vector2(Godot.Vector2i v)
        {
            return new Vector2(v.x, v.y);
        }

        public static implicit operator Godot.Vector2i(Vector2 v)
        {
            return new Godot.Vector2i((int)v.x, (int)v.y);
        }
#endif

        public static Vector2 Scale (Vector2 a, Vector2 b)
        {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
    }
}
