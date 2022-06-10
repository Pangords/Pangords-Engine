namespace PangordsEngine
{
    public static partial class Mathf
    {
        public static Matrix2x2 Inverse(Matrix2x2 m)
        {

            float OneOverDeterminant = (1f) / (
                +m[0][0] * m[1][1]
                - m[1][0] * m[0][1]);

            Matrix2x2 Inverse = new Matrix2x2(
                +m[1][1] * OneOverDeterminant,
                -m[0][1] * OneOverDeterminant,
                -m[1][0] * OneOverDeterminant,
                +m[0][0] * OneOverDeterminant);

            return Inverse;
        }

        public static Matrix3x3 Inverse(Matrix3x3 m)
        {
            float OneOverDeterminant = (1f) / (
                +m[0][0] * (m[1][1] * m[2][2] - m[2][1] * m[1][2])
                - m[1][0] * (m[0][1] * m[2][2] - m[2][1] * m[0][2])
                + m[2][0] * (m[0][1] * m[1][2] - m[1][1] * m[0][2]));

            Matrix3x3 Inverse = new Matrix3x3(0);
            Inverse[0, 0] = +(m[1][1] * m[2][2] - m[2][1] * m[1][2]) * OneOverDeterminant;
            Inverse[1, 0] = -(m[1][0] * m[2][2] - m[2][0] * m[1][2]) * OneOverDeterminant;
            Inverse[2, 0] = +(m[1][0] * m[2][1] - m[2][0] * m[1][1]) * OneOverDeterminant;
            Inverse[0, 1] = -(m[0][1] * m[2][2] - m[2][1] * m[0][2]) * OneOverDeterminant;
            Inverse[1, 1] = +(m[0][0] * m[2][2] - m[2][0] * m[0][2]) * OneOverDeterminant;
            Inverse[2, 1] = -(m[0][0] * m[2][1] - m[2][0] * m[0][1]) * OneOverDeterminant;
            Inverse[0, 2] = +(m[0][1] * m[1][2] - m[1][1] * m[0][2]) * OneOverDeterminant;
            Inverse[1, 2] = -(m[0][0] * m[1][2] - m[1][0] * m[0][2]) * OneOverDeterminant;
            Inverse[2, 2] = +(m[0][0] * m[1][1] - m[1][0] * m[0][1]) * OneOverDeterminant;

            return Inverse;

        }
        public static Matrix4x4 Inverse(Matrix4x4 m)
        {
            float Coef00 = m[2][2] * m[3][3] - m[3][2] * m[2][3];
            float Coef02 = m[1][2] * m[3][3] - m[3][2] * m[1][3];
            float Coef03 = m[1][2] * m[2][3] - m[2][2] * m[1][3];

            float Coef04 = m[2][1] * m[3][3] - m[3][1] * m[2][3];
            float Coef06 = m[1][1] * m[3][3] - m[3][1] * m[1][3];
            float Coef07 = m[1][1] * m[2][3] - m[2][1] * m[1][3];

            float Coef08 = m[2][1] * m[3][2] - m[3][1] * m[2][2];
            float Coef10 = m[1][1] * m[3][2] - m[3][1] * m[1][2];
            float Coef11 = m[1][1] * m[2][2] - m[2][1] * m[1][2];

            float Coef12 = m[2][0] * m[3][3] - m[3][0] * m[2][3];
            float Coef14 = m[1][0] * m[3][3] - m[3][0] * m[1][3];
            float Coef15 = m[1][0] * m[2][3] - m[2][0] * m[1][3];

            float Coef16 = m[2][0] * m[3][2] - m[3][0] * m[2][2];
            float Coef18 = m[1][0] * m[3][2] - m[3][0] * m[1][2];
            float Coef19 = m[1][0] * m[2][2] - m[2][0] * m[1][2];

            float Coef20 = m[2][0] * m[3][1] - m[3][0] * m[2][1];
            float Coef22 = m[1][0] * m[3][1] - m[3][0] * m[1][1];
            float Coef23 = m[1][0] * m[2][1] - m[2][0] * m[1][1];

            Vector4 Fac0 = new Vector4(Coef00, Coef00, Coef02, Coef03);
            Vector4 Fac1 = new Vector4(Coef04, Coef04, Coef06, Coef07);
            Vector4 Fac2 = new Vector4(Coef08, Coef08, Coef10, Coef11);
            Vector4 Fac3 = new Vector4(Coef12, Coef12, Coef14, Coef15);
            Vector4 Fac4 = new Vector4(Coef16, Coef16, Coef18, Coef19);
            Vector4 Fac5 = new Vector4(Coef20, Coef20, Coef22, Coef23);

            Vector4 Vec0 = new Vector4(m[1][0], m[0][0], m[0][0], m[0][0]);
            Vector4 Vec1 = new Vector4(m[1][1], m[0][1], m[0][1], m[0][1]);
            Vector4 Vec2 = new Vector4(m[1][2], m[0][2], m[0][2], m[0][2]);
            Vector4 Vec3 = new Vector4(m[1][3], m[0][3], m[0][3], m[0][3]);

            Vector4 Inv0 = new Vector4(Vec1 * Fac0 - Vec2 * Fac1 + Vec3 * Fac2);
            Vector4 Inv1 = new Vector4(Vec0 * Fac0 - Vec2 * Fac3 + Vec3 * Fac4);
            Vector4 Inv2 = new Vector4(Vec0 * Fac1 - Vec1 * Fac3 + Vec3 * Fac5);
            Vector4 Inv3 = new Vector4(Vec0 * Fac2 - Vec1 * Fac4 + Vec2 * Fac5);

            Vector4 SignA = new Vector4(+1, -1, +1, -1);
            Vector4 SignB = new Vector4(-1, +1, -1, +1);
            Matrix4x4 Inverse = new Matrix4x4(Inv0 * SignA, Inv1 * SignB, Inv2 * SignA, Inv3 * SignB);

            Vector4 Row0 = new Vector4(Inverse[0][0], Inverse[1][0], Inverse[2][0], Inverse[3][0]);

            Vector4 Dot0 = new Vector4(m[0] * Row0);
            float Dot1 = (Dot0.x + Dot0.y) + (Dot0.z + Dot0.w);

            float OneOverDeterminant = (1f) / Dot1;

            return Inverse * OneOverDeterminant;
        }
    }
}