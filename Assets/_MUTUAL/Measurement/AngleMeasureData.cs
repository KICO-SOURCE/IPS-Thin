#region Usings

using System;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Measurement
{
    /// <summary>
    /// Angle measurement data class
    /// </summary>
    public class AngleMeasureData : MeasureData
    {
        #region Public Fields

        /// <summary>
        /// Gets or sets the first line vector part of the angle
        /// </summary>
        public Vector3 Line1
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the second line vector part of the angle
        /// </summary>
        public Vector3 Line2
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of angle measurement data.
        /// </summary>
        public AngleMeasureData()
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
            if (measurement.type != DataModelsTypes.Measurements.eType.angleMeasure)
            {
                throw new InvalidOperationException("Measurement type and data does not match");
            }

            base.PrepareData(measurement);
            if (CanCalculate)
            {
                // use dummy values for now.
                if (useDummy)
                {
                    Line1 = Vector3.right;
                    Line2 = Vector3.forward;
                }
                else
                {
                    // Iterate the patient landmarks list and find the type of landmarks mentioned "measurement.points"
                }
            }
        }

        #endregion
    }
}