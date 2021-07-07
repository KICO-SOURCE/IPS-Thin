#region Usings

using UnityEngine;

#endregion

namespace Assets._MUTUAL.Measurement
{
    /// <summary>
    /// Measurement manager class resposible for the calculation of different measurements
    /// </summary>
    public class MeasurementManager
    {
        #region Constructors

        /// <summary>
        /// Creates new instance of measurement manager.
        /// </summary>
        public MeasurementManager()
        {
        }

        #endregion

        #region  Public Methods

        /// <summary>
        /// Invokes all calculations specified in the data models measurements configuration sheet.
        /// </summary>
        public void InvokeCalculations()
        {
            // Iterate through the measurements list and prepare measurement data according to the type then calculate measurement.
            for (int i = 0; i < DataModels.measurements.Length; i++)
            {
                var result = 0f;
                var measurement = DataModels.measurements[i];
                var data = MeasureDataFactory.CreateMeasureData(measurement);
                if (data.CanCalculate)
                {
                    result = data.CalculateMeasure();
                }
                // For testing purpose.
                Debug.Log("Result of " + measurement.type + " : " + result);
            }
        }

        /// <summary>
        /// Calculate distance between two points
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">>Second point</param>
        /// <returns></returns>
        public static float CalculateDistance(Vector3 point1, Vector3 point2)
        {
            return Vector3.Distance(point1, point2);
        }

        /// <summary>
        /// Calculate distance to a plance from a point.
        /// </summary>
        /// <param name="plane">Plane data.</param>
        /// <param name="point">Point data.</param>
        /// <returns></returns>
        public static float CalculateDistanceToPlane(Plane plane, Vector3 point)
        {
            var distance = plane.GetDistanceToPoint(point);
            return distance;
        }

        /// <summary>
        /// Calculate angle between two vectors.
        /// </summary>
        /// <param name="line1">First leg of the angle.</param>
        /// <param name="line2">Second leg of the angle.</param>
        /// <returns></returns>
        public static float CalculateAngle(Vector3 line1, Vector3 line2)
        {
            var angle = Vector3.Angle(line1, line2);
            return angle;
        }

        #endregion

        #region  Private Methods

        #endregion
    }
}
