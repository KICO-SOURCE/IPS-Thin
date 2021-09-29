#region Usings

using System;
using UnityEngine;

#endregion

namespace Assets.Utils
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

        /// <summary>
        /// Gets the mid point.
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static Vector3 GetMidPoint(Vector3 p1, Vector3 p2)
        {
            return new Vector3(
                (p1.x + p2.x) * 0.5f,
                (p1.y + p2.y) * 0.5f,
                (p1.z + p2.z) * 0.5f);
        }

        #endregion
    }
}
