using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PangordsEngine
{
    public static partial class Mathf
    {
        public static float ArcCos(float x)
        {
            return (float)Math.Acos(x);
        }

        public static float ArcCosH(float x)
        {

            if (x < (1f))
                return (0f);
            return (float)Math.Log(x + Math.Sqrt(x * x - (1f)));
        }

        public static float ArcSin(float x)
        {
            return (float)Math.Asin(x);
        }

        public static float ArcSinH(float x)
        {
            return (float)(x < 0f ? -1f : (x > 0f ? 1f : 0f)) * (float)Math.Log(Math.Abs(x) + Math.Sqrt(1f + x * x));
        }

        public static float ArcTan(float y, float x)
        {
            return (float)Math.Atan2(y, x);
        }

        public static float ArcTan(float y_over_x)
        {
            return (float)Math.Atan(y_over_x);
        }

        public static float ArcTanH(float x)
        {
            if (Math.Abs(x) >= 1f)
                return 0;
            return (0.5f) * (float)Math.Log((1f + x) / (1f - x));
        }

        public static float Cos(float angle)
        {
            return (float)Math.Cos(angle);
        }

        public static float CosH(float angle)
        {
            return (float)Math.Cosh(angle);
        }

        public static float RadiansToDegrees(float radians)
        {
            return radians * (57.295779513082320876798154814105f);
        }

        public static float DegreesToRadians(float degrees)
        {
            return degrees * (0.01745329251994329576923690768489f);
        }

        public static float Sin(float angle)
        {
            return (float)Math.Sin(angle);
        }

        public static float SinH(float angle)
        {
            return (float)Math.Sinh(angle);
        }

        public static float Tan(float angle)
        {
            return (float)Math.Tan(angle);
        }

        public static float TanH(float angle)
        {
            return (float)Math.Tanh(angle);
        }
    }
}
