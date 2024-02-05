using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.Engine
{
    public struct Vector2I
    {
        public int X;
        public int Y;
        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"{{X:{X}, Y:{Y}}}";
        }

        public static Vector2I operator +(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2I operator -(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static implicit operator Vector2(Vector2I v) => new Vector2(v.X, v.Y);
        public static explicit operator Vector2I(Vector2 v) => new Vector2I((int)v.X, (int)v.Y);
    }

    internal static class MathExtensions
    {
        public static Vector3 ToXNA(this BulletSharp.Math.Vector3 v)
        {
            return new Vector3((float)v.X, (float)v.Y, (float)v.Z);
        }

        public static BulletSharp.Math.Vector3 ToBullet(this Vector3 v)
        {
            return new BulletSharp.Math.Vector3(v.X, v.Y, v.Z);
        }

        public static Matrix ToXNA(this BulletSharp.Math.Matrix v)
        {
            return new Matrix(
                (float)v.M11, (float)v.M12, (float)v.M13, (float)v.M14,
                (float)v.M21, (float)v.M22, (float)v.M23, (float)v.M24,
                (float)v.M31, (float)v.M32, (float)v.M33, (float)v.M34,
                (float)v.M41, (float)v.M42, (float)v.M43, (float)v.M44
            );
        }
        

        public static BulletSharp.Math.Matrix ToBullet(this Matrix v)
        {
            return new BulletSharp.Math.Matrix(
                v.M11, v.M12, v.M13, v.M14,
                v.M21, v.M22, v.M23, v.M24,
                v.M31, v.M32, v.M33, v.M34,

                v.M41, v.M42, v.M43, v.M44
            );
        }

        public static System.Numerics.Vector3 ToNumerics(this Vector3 v)
        {
            return new System.Numerics.Vector3(v.X, v.Y, v.Z);
        }

        public static System.Numerics.Quaternion ToNumerics(this Quaternion v)
        {
            return new System.Numerics.Quaternion(v.X, v.Y, v.Z, v.W);
        }

        public static Vector3 HalfExtents(this Matrix matrix)
        {
            return new Vector3(matrix.Right.Length(), matrix.Up.Length(), matrix.Forward.Length()) / 2;
        }

        public static Matrix CrossMatrix(this Vector3 v)
        {
            return new Matrix(0, v.Z, -v.Y, 0, 
                              -v.Z, 0, v.X, 0, 
                              v.Y, -v.X, 0, 0, 
                              0,   0,   0,  0);
        }

        public static float GetIndex(this Vector3 v, int i)
        {
            switch(i)
            {
                case 0:
                    return v.X;
                case 1:
                    return v.Y;
                case 2:
                    return v.Z;
            }
            throw new Exception($"{i} is outside vector bounds");
        }

        public static void SetIndex(this ref Vector3 v, int i, float value)
        {
            switch (i)
            {
                case 0:
                    v.X = value;
                    return;
                case 1:
                    v.Y = value;
                    return;
                case 2:
                    v.Z = value;
                    return;
            }
            throw new Exception($"{i} is outside vector bounds");
        }

    }
}
