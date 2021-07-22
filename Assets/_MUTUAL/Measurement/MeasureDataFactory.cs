namespace Assets._MUTUAL.Measurement
{
    /// <summary>
    /// A factory class to create measurement data based on the measurement type from data model
    /// </summary>
    public class MeasureDataFactory
    {
        /// <summary>
        /// Create measurement data for the given measurement type.
        /// </summary>
        /// <param name="measurement">Measurement data model</param>
        /// <returns>Measurement data object</returns>
        public static IMeasureData CreateMeasureData(DataModelsTypes.Measurements measurement)
        {
            IMeasureData data;
            //  Create measurement data based on type.
            switch (measurement.type)
            {
                case DataModelsTypes.Measurements.eType.pointToPointDistance:
                    data = new PointsDistanceMeasureData();
                    break;
                case DataModelsTypes.Measurements.eType.pointToPlaneDistance:
                    data = new PointToPlaneDistanceMeasureData();
                    break;
                case DataModelsTypes.Measurements.eType.angleMeasure:
                    data = new AngleMeasureData();
                    break;
                default:
                    data = new MeasureData();
                    break;
            }
            // Prepare data from data model.
            data.PrepareData(measurement);
            return data;
        }
    }
}
