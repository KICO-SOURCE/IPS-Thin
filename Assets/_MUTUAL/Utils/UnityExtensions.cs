#region Usings

using UnityEngine;

#endregion

namespace Assets._MUTUAL.Utils
{
    /// <summary>
    /// Unity extensions class.
    /// </summary>
    public static class UnityExtensions
    {
        #region Public Static Methods

        /// <summary>
        /// Set transform from right hand euler.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        public static void SetFromRightHandEuler(this Transform transform, float x, float y, float z, float yaw, float pitch, float roll)
        {
            var matrix = new Matrix4x4();
            matrix = matrix.ConvertRightHandEulerToMatrix(x, y, z, yaw, pitch, roll);
            matrix = matrix.ConvertToLefttHandCoordinate();
            transform.SetFromMatrix(matrix);
        }

        /// <summary>
        /// Convert right hand euler to matrix.
        /// </summary>
        /// <param name="m"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="yaw"></param>
        /// <param name="pitch"></param>
        /// <param name="roll"></param>
        /// <returns></returns>
        public static Matrix4x4 ConvertRightHandEulerToMatrix(this Matrix4x4 m, float x, float y, float z, float yaw, float pitch, float roll)
        {
            var xComponent = x;
            var yComponent = y;
            var zComponent = z;
            var xRotComponent = pitch;
            var yRotComponent = yaw;
            var zRotComponent = roll;

            var cosRx = Mathf.Cos(MathUtil.DegreesToRadians(xRotComponent));
            var cosRy = Mathf.Cos(MathUtil.DegreesToRadians(yRotComponent));
            var cosRz = Mathf.Cos(MathUtil.DegreesToRadians(zRotComponent));
            var sinRx = Mathf.Sin(MathUtil.DegreesToRadians(xRotComponent));
            var sinRy = Mathf.Sin(MathUtil.DegreesToRadians(yRotComponent));
            var sinRz = Mathf.Sin(MathUtil.DegreesToRadians(zRotComponent));

            var m00 = cosRx * cosRz;
            var m01 = -(cosRy * sinRz + sinRx * sinRy * cosRz);
            var m02 = sinRy * cosRz + sinRx * cosRy * sinRz;
            var m10 = cosRx * sinRz;
            var m11 = cosRy * cosRz - sinRx * sinRy * sinRz;
            var m12 = sinRy * sinRz - sinRx * cosRy * cosRz;
            var m20 = -cosRx * sinRy;
            var m21 = sinRx;
            var m22 = cosRx * cosRy;

            var matrix = new Matrix4x4();
            matrix.SetRow(0, new Vector4(m00, m01, m02, xComponent));
            matrix.SetRow(1, new Vector4(m10, m11, m12, yComponent));
            matrix.SetRow(2, new Vector4(m20, m21, m22, zComponent));
            matrix.SetRow(3, new Vector4(0, 0, 0, 1));
            return matrix;
        }

        /// <summary>
        /// Convert matrix to left hand coordinate.
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public static Matrix4x4 ConvertToLefttHandCoordinate(this Matrix4x4 m)
        {
            return new Matrix4x4(
                new Vector4(m.m00, -m.m21, m.m10, m.m30),
                new Vector4(-m.m12, m.m22, m.m02, m.m31),
                new Vector4(m.m01, m.m20, m.m11, m.m32),
                new Vector4(-m.m13, m.m23, m.m03, m.m33)
            );
        }

        /// <summary>
        /// Set transform from matrix data.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="matrix"></param>
        public static void SetFromMatrix(this Transform transform, Matrix4x4 matrix)
        {
            // https://answers.unity.com/questions/1134216/how-to-set-transformation-matrices-of-transform.html
            transform.localPosition = matrix.GetColumn(3);
            transform.localScale = new Vector3(matrix.GetColumn(0).magnitude, matrix.GetColumn(1).magnitude, matrix.GetColumn(2).magnitude);
            float w = Mathf.Sqrt(Mathf.Abs(1.0f + matrix.m00 + matrix.m11 + matrix.m22)) / 2.0f;
            if (w != 0)
            {
                // https://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/
                transform.localRotation = new Quaternion((matrix.m21 - matrix.m12) / (4.0f * w),
                    (matrix.m02 - matrix.m20) / (4.0f * w), (matrix.m10 - matrix.m01) / (4.0f * w), w);
            }
            else
            {
                // https://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/ethan.htm
                float tr1 = 1.0f + matrix.m00 - matrix.m11 - matrix.m22;
                float tr2 = 1.0f - matrix.m00 + matrix.m11 - matrix.m22;
                float tr3 = 1.0f - matrix.m00 - matrix.m11 + matrix.m22;
                float qx, qy, qz, qw;
                if ((tr1 > tr2) && (tr1 > tr3))
                {
                    float S = Mathf.Sqrt(tr1) * 2; // S=4*qx 
                    qw = (matrix.m21 - matrix.m12) / S;
                    qx = 0.25f * S;
                    qy = (matrix.m01 + matrix.m10) / S;
                    qz = (matrix.m02 + matrix.m20) / S;
                }
                else if (!(tr2 > tr1) && (tr2 > tr3))
                {
                    float S = Mathf.Sqrt(tr2) * 2; // S=4*qy
                    qw = (matrix.m02 - matrix.m20) / S;
                    qx = (matrix.m01 + matrix.m10) / S;
                    qy = 0.25f * S;
                    qz = (matrix.m12 + matrix.m21) / S;
                }
                else
                {
                    float S = Mathf.Sqrt(tr3) * 2; // S=4*qz
                    qw = (matrix.m10 - matrix.m01) / S;
                    qx = (matrix.m02 + matrix.m20) / S;
                    qy = (matrix.m12 + matrix.m21) / S;
                    qz = 0.25f * S;
                }
                transform.localRotation = new Quaternion(qx, qy, qz, qw);
            }
        }

        #endregion
    }
}
