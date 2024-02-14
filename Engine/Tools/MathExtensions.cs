using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// Creates an array of smooth points with at a specified resolution
        /// </summary>
        /// <param name="points">control points</param>
        /// <param name="resolution"> how big should each step between the points be</param>
        /// <returns>Retuns a list of points</returns>
        public static Vector3[] CatmullRom(Vector3[] points, float resolution)
        {
            List<Vector3> result = new List<Vector3>();

            for (int i = 0; i < points.Length - 1; i++)
            {
                Vector3 P0, P1, P2, P3;
                if (i == 0) //first segment
                {
                    P0 = points[i];
                    P1 = points[i];
                    P2 = points[i + 1];
                    P3 = (points.Length > 2) ? points[i + 2] : points[i + 1];
                }
                else if (i == points.Length - 2) // last segment
                {
                    P0 = points[i - 1];
                    P1 = points[i];
                    P2 = points[i + 1];
                    P3 = points[i + 1];
                }
                else
                {
                    P0 = points[i - 1];
                    P1 = points[i];
                    P2 = points[i + 1];
                    P3 = points[i + 2];
                }

                float t = 0.0f;
                while (t < 1.0f)
                {
                    Vector3 point = CalculateCatmullRom(P0, P1, P2, P3, t);
                    result.Add(point);
                    t += resolution;
                }
            }
            result.Add(points[points.Length - 1]); // Add the last point separately
            return result.ToArray();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static Vector3 CalculateCatmullRom(Vector3 P0, Vector3 P1, Vector3 P2, Vector3 P3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;
            return 0.5f * ((2.0f * P1) + (-P0 + P2) * t + (2.0f * P0 - 5.0f * P1 + 4f * P2 - P3) * t2
                           + (-P0 + 3.0f * P1 - 3.0f * P2 + P3) * t3);
        }


        public static void BezierQuadratic(Vector3[] arr, float t, out Vector3 pos, out Vector3 normal)
        {
            Vector3 a = Vector3.Lerp(arr[0], arr[1], t);
            Vector3 b = Vector3.Lerp(arr[1], arr[2], t);
            pos = Vector3.Lerp(a, b, t);
            normal = Vector3.Normalize(a - b);
        }

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
