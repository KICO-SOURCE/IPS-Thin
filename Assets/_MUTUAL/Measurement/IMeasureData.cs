namespace Assets._MUTUAL.Measurement
{
    /// <summary>
    /// Interface for the measure data object.
    /// </summary>
    public interface IMeasureData
    {
        #region Fields

        /// <summary>
        /// Gets whether the measurement can be calculated using this data
        /// </summary>
        bool CanCalculate
        {
            get;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the measurement data from the data model.
        /// </summary>
        /// <param name="measurement">Measurement data model</param>
        void PrepareData(DataModelsTypes.Measurements measurement);

        /// <summary>
        /// Calculate the measurement using the measurement data model.
        /// </summary>
        /// <returns>Returs the calculation result.</returns>
        float CalculateMeasure();

        #endregion
    }
}
