using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PangordsEngine
{
    public static partial class Mathf
    {
        /// <summary>
        /// Creates a frustrum projection matrix.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top.</param>
        /// <param name="nearVal">The near val.</param>
        /// <param name="farVal">The far val.</param>
        /// <returns></returns>
        public static Matrix4x4 Frustum(float left, float right, float bottom, float top, float nearVal, float farVal)
        {
            var result = Matrix4x4.identity();
            result[0, 0] = (2.0f * nearVal) / (right - left);
            result[1, 1] = (2.0f * nearVal) / (top - bottom);
            result[2, 0] = (right + left) / (right - left);
            result[2, 1] = (top + bottom) / (top - bottom);
            result[2, 2] = -(farVal + nearVal) / (farVal - nearVal);
            result[2, 3] = -1.0f;
            result[3, 2] = -(2.0f * farVal * nearVal) / (farVal - nearVal);
            return result;
        }

        /// <summary>
        /// Creates a matrix for a symmetric perspective-view frustum with far plane at infinite.
        /// </summary>
        /// <param name="fovy">The fovy.</param>
        /// <param name="aspect">The aspect.</param>
        /// <param name="zNear">The z near.</param>
        /// <returns></returns>
        public static Matrix4x4 InfinitePerspective(float fovy, float aspect, float zNear)
        {

            float range = Tan(fovy / (2f)) * zNear;

            float left = -range * aspect;
            float right = range * aspect;
            float bottom = -range;
            float top = range;

            var result = new Matrix4x4(0);
            result[0, 0] = ((2f) * zNear) / (right - left);
            result[1, 1] = ((2f) * zNear) / (top - bottom);
            result[2, 2] = -(1f);
            result[2, 3] = -(1f);
            result[3, 2] = -(2f) * zNear;
            return result;
        }

        /// <summary>
        /// Build a look at view matrix.
        /// </summary>
        /// <param name="eye">The eye.</param>
        /// <param name="center">The center.</param>
        /// <param name="up">Up.</param>
        /// <returns></returns>
        public static Matrix4x4 LookAt(Vector3 eye, Vector3 center, Vector3 up)
        {
            Vector3 f = new Vector3(Normalize(center - eye));
            Vector3 s = new Vector3(Normalize(Cross(f, up)));
            Vector3 u = new Vector3(Cross(s, f));

            Matrix4x4 Result = new Matrix4x4(1);
            Result[0, 0] = s.x;
            Result[1, 0] = s.y;
            Result[2, 0] = s.z;
            Result[0, 1] = u.x;
            Result[1, 1] = u.y;
            Result[2, 1] = u.z;
            Result[0, 2] = -f.x;
            Result[1, 2] = -f.y;
            Result[2, 2] = -f.z;
            Result[3, 0] = -Dot(s, eye);
            Result[3, 1] = -Dot(u, eye);
            Result[3, 2] = Dot(f, eye);
            return Result;
        }

        /// <summary>
        /// Creates a matrix for an orthographic parallel viewing volume.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top.</param>
        /// <param name="zNear">The z near.</param>
        /// <param name="zFar">The z far.</param>
        /// <returns></returns>
        public static Matrix4x4 Ortho(float left, float right, float bottom, float top, float zNear, float zFar)
        {
            var result = Matrix4x4.identity();
            result[0, 0] = (2f) / (right - left);
            result[1, 1] = (2f) / (top - bottom);
            result[2, 2] = -(2f) / (zFar - zNear);
            result[3, 0] = -(right + left) / (right - left);
            result[3, 1] = -(top + bottom) / (top - bottom);
            result[3, 2] = -(zFar + zNear) / (zFar - zNear);
            return result;
        }

        /// <summary>
        /// Creates a matrix for projecting two-dimensional coordinates onto the screen.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="bottom">The bottom.</param>
        /// <param name="top">The top.</param>
        /// <returns></returns>
        public static Matrix4x4 Ortho(float left, float right, float bottom, float top)
        {
            var result = Matrix4x4.identity();
            result[0, 0] = (2f) / (right - left);
            result[1, 1] = (2f) / (top - bottom);
            result[2, 2] = -(1f);
            result[3, 0] = -(right + left) / (right - left);
            result[3, 1] = -(top + bottom) / (top - bottom);
            return result;
        }

        /// <summary>
        /// Creates a perspective transformation matrix.
        /// </summary>
        /// <param name="fovy">The field of view angle, in radians.</param>
        /// <param name="aspect">The aspect ratio.</param>
        /// <param name="zNear">The near depth clipping plane.</param>
        /// <param name="zFar">The far depth clipping plane.</param>
        /// <returns>A <see cref="Matrix4x4"/> that contains the projection matrix for the perspective transformation.</returns>
        public static Matrix4x4 Perspective(float fovy, float aspect, float zNear, float zFar)
        {
            var tanHalfFovy = (float)Math.Tan(fovy / 2.0f);

            var result = Matrix4x4.identity();
            result[0, 0] = 1.0f / (aspect * tanHalfFovy);
            result[1, 1] = 1.0f / (tanHalfFovy);
            result[2, 2] = -(zFar + zNear) / (zFar - zNear);
            result[2, 3] = -1.0f;
            result[3, 2] = -(2.0f * zFar * zNear) / (zFar - zNear);
            result[3, 3] = 0.0f;
            return result;
        }

        /// <summary>
        /// Builds a perspective projection matrix based on a field of view.
        /// </summary>
        /// <param name="fov">The fov (in radians).</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="zNear">The z near.</param>
        /// <param name="zFar">The z far.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static Matrix4x4 PerspectiveFov(float fov, float width, float height, float zNear, float zFar)
        {
            if (width <= 0 || height <= 0 || fov <= 0)
                throw new ArgumentOutOfRangeException();

            var rad = fov;

            var h = Mathf.Cos((0.5f) * rad) / Mathf.Sin((0.5f) * rad);
            var w = h * height / width;

            var result = new Matrix4x4(0);
            result[0, 0] = w;
            result[1, 1] = h;
            result[2, 2] = -(zFar + zNear) / (zFar - zNear);
            result[2, 3] = -(1f);
            result[3, 2] = -((2f) * zFar * zNear) / (zFar - zNear);
            return result;
        }

        /// <summary>
        /// Define a picking region.
        /// </summary>
        /// <param name="center">The center.</param>
        /// <param name="delta">The delta.</param>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public static Matrix4x4 PickMatrix(Vector2 center, Vector2 delta, Vector4 viewport)
        {
            if (delta.x <= 0 || delta.y <= 0)
                throw new ArgumentOutOfRangeException();
            var Result = new Matrix4x4(1.0f);

            if (!(delta.x > (0f) && delta.y > (0f)))
                return Result; // Error

            Vector3 Temp = new Vector3(
                ((viewport[2]) - (2f) * (center.x - (viewport[0]))) / delta.x,
                ((viewport[3]) - (2f) * (center.y - (viewport[1]))) / delta.y,
                (0f));

            // Translate and scale the picked region to the entire window
            Result = Translate(Result, Temp);
            return Scale(Result, new Vector3((viewport[2]) / delta.x, (viewport[3]) / delta.y, (1)));
        }

        /// <summary>
        /// Map the specified object coordinates (obj.x, obj.y, obj.z) into window coordinates.
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <param name="model">The model.</param>
        /// <param name="proj">The proj.</param>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        public static Vector3 Project(Vector3 obj, Matrix4x4 model, Matrix4x4 proj, Vector4 viewport)

        {
            Vector4 tmp = new Vector4(obj, (1f));
            tmp = model * tmp;
            tmp = proj * tmp;

            tmp /= tmp.w;
            tmp = tmp * 0.5f + 0.5f;
            tmp[0] = tmp[0] * viewport[2] + viewport[0];
            tmp[1] = tmp[1] * viewport[3] + viewport[1];

            return new Vector3(tmp.x, tmp.y, tmp.z);
        }

        /// <summary>
        /// Builds a rotation 4 * 4 matrix created from an axis vector and an angle.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <param name="angle">The angle.</param>
        /// <param name="v">The v.</param>
        /// <returns></returns>
        public static Matrix4x4 Rotate(Matrix4x4 m, float angle, Vector3 v)
        {
            float c = Cos(angle);
            float s = Sin(angle);

            Vector3 axis = Normalize(v);
            Vector3 temp = (1.0f - c) * axis;

            Matrix4x4 rotate = Matrix4x4.identity();
            rotate[0, 0] = c + temp[0] * axis[0];
            rotate[0, 1] = 0 + temp[0] * axis[1] + s * axis[2];
            rotate[0, 2] = 0 + temp[0] * axis[2] - s * axis[1];

            rotate[1, 0] = 0 + temp[1] * axis[0] - s * axis[2];
            rotate[1, 1] = c + temp[1] * axis[1];
            rotate[1, 2] = 0 + temp[1] * axis[2] + s * axis[0];

            rotate[2, 0] = 0 + temp[2] * axis[0] + s * axis[1];
            rotate[2, 1] = 0 + temp[2] * axis[1] - s * axis[0];
            rotate[2, 2] = c + temp[2] * axis[2];

            Matrix4x4 result = Matrix4x4.identity();
            result[0] = m[0] * rotate[0][0] + m[1] * rotate[0][1] + m[2] * rotate[0][2];
            result[1] = m[0] * rotate[1][0] + m[1] * rotate[1][1] + m[2] * rotate[1][2];
            result[2] = m[0] * rotate[2][0] + m[1] * rotate[2][1] + m[2] * rotate[2][2];
            result[3] = m[3];
            return result;
        }


        //  TODO: this is actually defined as an extension, put in the right file.
        public static Matrix4x4 Rotate(float angle, Vector3 v)
        {
            return Rotate(Matrix4x4.identity(), angle, v);
        }


        /// <summary>
        /// Applies a scale transformation to matrix <paramref name="m"/> by vector <paramref name="v"/>.
        /// </summary>
        /// <param name="m">The matrix to transform.</param>
        /// <param name="v">The vector to scale by.</param>
        /// <returns><paramref name="m"/> scaled by <paramref name="v"/>.</returns>
        public static Matrix4x4 Scale(Matrix4x4 m, Vector3 v)
        {
            Matrix4x4 result = m;
            result[0] = m[0] * v[0];
            result[1] = m[1] * v[1];
            result[2] = m[2] * v[2];
            result[3] = m[3];
            return result;
        }

        /// <summary>
        /// Applies a translation transformation to matrix <paramref name="m"/> by vector <paramref name="v"/>.
        /// </summary>
        /// <param name="m">The matrix to transform.</param>
        /// <param name="v">The vector to translate by.</param>
        /// <returns><paramref name="m"/> translated by <paramref name="v"/>.</returns>
        public static Matrix4x4 Translate(Matrix4x4 m, Vector3 v)
        {
            Matrix4x4 result = m;
            result[3] = m[0] * v[0] + m[1] * v[1] + m[2] * v[2] + m[3];
            return result;
        }

        /// <summary>
        /// Creates a matrix for a symmetric perspective-view frustum with far plane 
        /// at infinite for graphics hardware that doesn't support depth clamping.
        /// </summary>
        /// <param name="fovy">The fovy.</param>
        /// <param name="aspect">The aspect.</param>
        /// <param name="zNear">The z near.</param>
        /// <returns></returns>
        public static Matrix4x4 TweakedInfinitePerspective(float fovy, float aspect, float zNear)
        {
            float range = Tan(fovy / (2)) * zNear;
            float left = -range * aspect;
            float right = range * aspect;
            float bottom = -range;
            float top = range;

            Matrix4x4 Result = new Matrix4x4((0f));
            Result[0, 0] = ((2) * zNear) / (right - left);
            Result[1, 1] = ((2) * zNear) / (top - bottom);
            Result[2, 2] = (0.0001f) - (1f);
            Result[2, 3] = (-1);
            Result[3, 2] = -((0.0001f) - (2)) * zNear;
            return Result;
        }

        /// <summary>
        /// Map the specified window coordinates (win.x, win.y, win.z) into object coordinates.
        /// </summary>
        /// <param name="win">The win.</param>
        /// <param name="model">The model.</param>
        /// <param name="proj">The proj.</param>
        /// <param name="viewport">The viewport.</param>
        /// <returns></returns>
        public static Vector3 UnProject(Vector3 win, Matrix4x4 model, Matrix4x4 proj, Vector4 viewport)
        {
            Matrix4x4 Inverse = Mathf.Inverse(proj * model);

            Vector4 tmp = new Vector4(win, (1f));
            tmp.x = (tmp.x - (viewport[0])) / (viewport[2]);
            tmp.y = (tmp.y - (viewport[1])) / (viewport[3]);
            tmp = tmp * (2f) - (1f);

            Vector4 obj = Inverse * tmp;
            obj /= obj.w;

            return new Vector3(obj);
        }
    }
}
