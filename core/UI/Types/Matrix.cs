using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altimit.UI
{
    public class Matrix3x2
    {
        public float this[int row, int col]
        {
            get
            {
                return _matrix[row, col];
            }
            set
            {
                _matrix[row, col] = value;
            }
        }

        /*
        public float M11 { get; set; }
        public float M12 { get; set; }
        public float M21 { get; set; }
        public float M22 { get; set; }
        public float M41 { get; set; }
        public float M42 { get; set; }

        public Matrix(float m11, float m12, float m21, float m22, float m41, float m42)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            M41 = m41;
            M42 = m42;
        }
        */

        private float[,] _matrix;

        public Matrix3x2(float[,] matrix)
        {
            _matrix = matrix;
        }

        public static Matrix3x2 CreateScale(float scale)
        {
            return new Matrix3x2(new float[,] {
            { scale, 0 },
            { 0, scale },
            { 0, 0 },
        });
        }

        public static Matrix3x2 CreateRotationZ(float rotation)
        {
            return new Matrix3x2(new float[,] {
            { (float)Math.Cos(rotation), (float)Math.Sin(rotation) },
            { (float)-Math.Sin(rotation), (float)Math.Cos(rotation) },
            { 0, 0 }
        });
        }

        public static Matrix3x2 operator *(Matrix3x2 m1, Matrix3x2 m2)
        {
            float[,] result = new float[3, 3];

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    result[x, y] = m1._matrix[x, 0] * m2._matrix[0, y] + m1._matrix[x, 1] * m2._matrix[1, y];
                }
            }

            return new Matrix3x2(result);
        }

        public static Matrix3x2 CreateTranslation(float x, float y)
        {
            float[,] values = { { 1, 0, x }, { 0, 1, y }, { 0, 0, 1 } };
            return new Matrix3x2(values);
        }
    }
}
