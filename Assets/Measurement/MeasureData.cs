namespace Assets.Measurement
{
    /// <summary>
    /// Measurement data base class
    /// </summary>
    public class MeasureData : IMeasureData
    {
        #region Public Fields

        /// <summary>
        /// Indicates whether dummy data need to be used if data is not available
        /// TODO : Remove after patient data integration
        /// </summary>
        protected bool useDummy = true;

        /// <summary>
        /// Gets whether the measurement can be calculated using this data
        /// </summary>
        public bool CanCalculate
        {
            get;
            private set;
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of measurement data.
        /// </summary>
        /// 
        public MeasureData()
        {
            // Expect the patient landmarks collection through dependency injection once the classes are implemented.
        }

        #endregion

        #region  Public Methods

        /// <summary>
        /// Prepare the measurement data from the data model.
        /// </summary>
        /// <param name="measurement">Measurement data model</param>
        public virtual void PrepareData(DataModelsTypes.Measurements measurement)
        {
            // set the calculate flag to false. This needs to set to true when the landmarks specified in the measurement for
            // this calculation is avialable in the patient landmarks list.
            CanCalculate = false;
            ValidateData(measurement);
        }

        /// <summary>
        /// Calculate the measurment using the measurement data by calling corresponding calculate function in measurement manager.
        /// </summary>
        /// <returns>Returs the calculation result.</returns>
        public virtual float CalculateMeasure()
        {
            // Child class implement their specific calculation logic.
            return 0;
        }

        #endregion

        #region  Protected Methods

        /// <summary>
        /// Validate whether enough data is available for the calculation of the provided meaurement.
        /// </summary>
        /// <param name="measurement">Measurement model</param>
        protected void ValidateData(DataModelsTypes.Measurements measurement)
        {
            if (measurement.points != null && measurement.points.Length > 0)
            {
                // Iterate the patient landmarks list and find the type of landmarks mentioned "measurement.points"

                // use dummy values for now.
                if (useDummy)
                {
                    CanCalculate = true;
                }
            }
        }

        #endregion
    }
}
