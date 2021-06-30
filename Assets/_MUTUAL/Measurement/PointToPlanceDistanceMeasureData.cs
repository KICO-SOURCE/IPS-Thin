#region Usings

using System;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Measurement
{
    /// <summary>
    /// Distance to plane measurement data class
    /// </summary>
    public class PointToPlanceDistanceMeasureData : MeasureData
    {
        #region Public Fields

        /// <summary>
        /// Gets or sets the plane instance.
        /// </summary>
        public Plane Plane
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the point.
        /// </summary>
        public Vector3 Point
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of PointToPlanceDistanceMeasureData.
        /// </summary>
        public PointToPlanceDistanceMeasureData()
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
            if (measurement.type != DataModelsTypes.Measurements.eType.pointToPlaneDistance)
            {
                throw new InvalidOperationException("Measurement type and data does not match");
            }

            base.PrepareData(measurement);
            if (CanCalculate)
            {
                // use dummy values for now.
                if (useDummy)
                {
                    Point = Vector3.zero;
                    Plane = new Plane();
                }
                else
                {
                    // Iterate the patient landmarks list and find the type of landmarks mentioned "measurement.points"
                    // Create Plane using first 3 points.
                }
            }
        }

        #endregion
    }
}