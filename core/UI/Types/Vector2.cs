using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Altimit;

namespace Altimit.UI
{
    public static class Vector2Extensions
    {
        public static float GetLength(this Vector2 vector)
        {
            return Mathf.Sqrt(vector.x * vector.x + vector.y * vector.y);
        }
    }

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

        public static Vector2 operator /(Vector2 v, float num)
        {
            return new Vector2(v.x / num, v.y / num);
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

        public static Vector2 Transform(Vector2 position, Matrix3x2 matrix)
        {
 
            return new Vector2(
                (position.x * matrix[0,0]) + (position.y * matrix[1, 0]) + matrix[2,0],
                (position.x * matrix[0,1]) + (position.y * matrix[1,1]) + matrix[2,1]);
        }

        public static float Distance(Vector2 a, Vector2 b)
        {
            float dx = a.x - b.x;
            float dy = a.y - b.y;
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            return a + (b - a) * t;
        }

        public static float SignedAngle(Vector2 a, Vector2 b)
        {
            float sign = Mathf.Sign(a.x * b.y - a.y * b.x);
            return Angle(a, b) * sign;
        }

        public static float Angle(Vector2 from, Vector2 to)
        {
            float dot = Vector2.Dot(from, to);
            float angle = Mathf.Acos(dot / (Vector2.Magnitude(from) * Vector2.Magnitude(to)));
            return angle;
        }


        public static float Magnitude(Vector2 pos)
        {
            return Mathf.Sqrt(pos.x * pos.x + pos.y * pos.y);
        }


        public static float Dot(Vector2 a, Vector2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

#if UNITY_64
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

        public static float GetDistance(Vector2 v1, Vector2 v2)
        {
            float xDiff = v1.x - v2.x;
            float yDiff = v1.y - v2.y;
            return (float)Math.Sqrt(xDiff * xDiff + yDiff * yDiff);
        }
    }
}
