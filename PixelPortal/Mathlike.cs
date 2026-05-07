using Microsoft.Xna.Framework;
using System;

namespace PixelPortal
{
    internal static class Mathlike
    {
        public static float ClampF(float v, float min, float max) {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
        public static int ClampI(int v, int min, int max)
        {
            if (v < min) return min;
            if (v > max) return max;
            return v;
        }
        /// <summary>
        /// circular clamp, sets max magnitude
        /// </summary>
        /// <param name="v"></param>
        /// <param name="maxl"></param>
        /// <returns></returns>
        public static Vector2 ClampV(Vector2 v, float maxl)
        {
            float vl = v.Length();
            if (vl > maxl) return v * maxl / vl;
            return v;
        }
        /// <summary>
        /// orthogonal clamp, piecewise between 0 and u (inclusive, inclusive)
        /// </summary>
        /// <param name="v"></param>
        /// <param name="u"></param>
        /// <returns></returns>
        public static Point ClampP(Point v, Point u)
        {
            int x = ClampI(v.X, 0, u.X);
            int y = ClampI(v.Y, 0, u.Y);
            return new Point(x, y);
        }
        public static float ModF(float a, float b)
        {
            return ((a % b) + b) % b;
        }
        public static int ModI(int v, int b)
        {
            return ((v % b) + b) % b;
            //return (v + b * 100) % b;
        }
        public static Vector2 WrapV(Vector2 v, Vector2 u)
        {
            float x = ModF(v.X, u.X);
            float y = ModF(v.Y, u.Y);
            return new Vector2(x, y);
        }
        public static Point WrapP(Point v, Point u)
        {
            int x = ModI(v.X, u.X);
            int y = ModI(v.Y, u.Y);
            return new Point(x, y);
        }
        public static float angleV(Vector2 v)
        {
            return MathF.Atan2(v.Y, v.X);
        }
        public static float angleP(Point v)
        {
            return MathF.Atan2(v.Y, v.X);
        }
        public static float ProjectionFactor(Vector2 v, Vector2 dir)
        {
            float dot = Vector2.Dot(v,dir);
            return dot / dir.LengthSquared();
        }

        public static float TwoDCrossProduct(Vector2 a, Vector2 b)
        {
            return a.X * b.Y - a.Y * b.X;
        }
    }
}
