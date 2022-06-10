using System;

namespace PangordsEngine
{
    public static partial class Mathf
    {
        public static Vector3 Cross(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.y * rhs.z - rhs.y * lhs.z,
                lhs.z * rhs.x - rhs.z * lhs.x,
                lhs.x * rhs.y - rhs.x * lhs.y);
        }

        public static float Dot(Vector2 x, Vector2 y)
        {
            Vector2 tmp = new Vector2(x * y);
            return tmp.x + tmp.y;
        }

        public static float Dot(Vector3 x, Vector3 y)
        {
            Vector3 tmp = new Vector3(x * y);
            return tmp.x + tmp.y + tmp.z;
        }

        public static float Dot(Vector4 x, Vector4 y)
        {
            Vector4 tmp = new Vector4(x * y);
            return (tmp.x + tmp.y) + (tmp.z + tmp.w);
        }

        public static Vector2 Normalize(Vector2 v)
        {
            float sqr = v.x * v.x + v.y * v.y;
            return v * (1.0f / (float)Math.Sqrt(sqr));
        }

        public static Vector3 Normalize(Vector3 v)
        {
            float sqr = v.x * v.x + v.y * v.y + v.z * v.z;
            return v * (1.0f / (float)Math.Sqrt(sqr));
        }

        public static Vector4 Normalize(Vector4 v)
        {
            float sqr = v.x * v.x + v.y * v.y + v.z * v.z + v.w * v.w;
            return v * (1.0f / (float)Math.Sqrt(sqr));
        }
    }
}