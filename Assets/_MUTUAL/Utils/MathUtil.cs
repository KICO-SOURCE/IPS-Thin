#region Usings

using System;

#endregion

namespace Assets._MUTUAL.Utils
{
    /// <summary>
    /// Maths utils class.
    /// </summary>
    public class MathUtil
    {
        #region Public Static Methods

        /// <summary>
        /// Convert degrees to radians.
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static float DegreesToRadians(float degrees)
        {
            return (float)(degrees * (Math.PI / 180.0));
        }

        #endregion
    }
}
