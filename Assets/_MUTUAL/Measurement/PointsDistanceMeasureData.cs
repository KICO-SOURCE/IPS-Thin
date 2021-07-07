#region Usings

using System;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Measurement
{
    /// <summary>
    /// Distance measurement data class
    /// </summary>
    public class PointsDistanceMeasureData : MeasureData
    {
        #region Public Fields

        /// <summary>
        /// Gets or sets the first point.
        /// </summary>
        public Vector3 Point1
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the second point.
        /// </summary>
        public Vector3 Point2
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of PointsDistanceMeasureData.
        /// </summary>
        public PointsDistanceMeasureData()
        {
            // Expect the patient landmarks collection through dependency injection once the classes are implemented.
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Prepare the measurement data from the data model.
        /// </summary>
        /// <param name="measurement">Measurement data model</param>
        public override void PrepareData(DataModelsTypes.Measurements measurement)
        {
            if (measurement.type != DataModelsTypes.Measurements.eType.pointToPointDistance)
            {
                throw new InvalidOperationException("Measurement type and data does not match");
            }

            base.PrepareData(measurement);
            if (CanCalculate)
            {
                // use dummy values for now.
                if(useDummy)
                {
                    Point1 = Vector3.zero;
                    Point2 = Vector3.zero;
                }
                else
                {
                    // Iterate the patient landmarks list and find the type of landmarks mentioned "measurement.points"
                }
            }
        }

        /// <summary>
        /// Calculate the measurment using the measurement data by calling corresponding calculate function in measurement manager.
        /// </summary>
        /// <returns>Returs the calculation result.</returns>
        public override float CalculateMeasure()
        {
            var result = MeasurementManager.CalculateDistance(Point1, Point2);
            return result;
        }

        #endregion
    }
}
