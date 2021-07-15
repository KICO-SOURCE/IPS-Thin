#region Usings

using System.Collections.Generic;
using System.Linq;
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
        /// Invokes the line rendering.
        /// </summary>
        public void RenderLines()
        {
            // Iterate through the lines list and prepare line data for rendering the line.
        }

        /// <summary>
        /// Draw the line with line render data.
        /// </summary>
        /// <param name="lineData">line data model</param>
        public void DrawLine(LineRenderData lineData, GameObject Parent)
        {
            Material lineRenderMaterial = Resources.Load<Material>("Materials/LineRenderer");
            LineRenderer lineRenderer;

            Parent.TryGetComponent(out lineRenderer);
            if (lineRenderer == null)
            {
                Parent.AddComponent<LineRenderer>();
                Parent.TryGetComponent(out lineRenderer);
            }

            lineRenderer.widthMultiplier = lineData.Thickness;
            lineRenderer.SetPositions(lineData.Points.ToArray());
            lineRenderer.material = lineRenderMaterial;
            lineRenderer.material.color = lineData.LineColor;
        }

        /// <summary>
        /// Calculate distance between two points
        /// </summary>
        /// <param name="point1">First point</param>
        /// <param name="point2">>Second point</param>
        /// <returns>Distance measurement</returns>
        public static float CalculateDistance(Vector3 point1, Vector3 point2)
        {
            return Vector3.Distance(point1, point2);
        }

        /// <summary>
        /// Calculate distance to a plance from a point.
        /// </summary>
        /// <param name="plane">Plane data.</param>
        /// <param name="point">Point data.</param>
        /// <returns>Distance measurement</returns>
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
        /// <returns>Angle measurement</returns>
        public static float CalculateAngle(Vector3 line1, Vector3 line2)
        {
            var angle = Vector3.Angle(line1, line2);
            return angle;
        }

        /// <summary>
        /// Calculate angle between two vectors projected on a plane.
        /// </summary>
        /// <param name="line1">First line vector.</param>
        /// <param name="line2">Second line vector.</param>
        /// <returns></returns>
        /// <param name="projectionPlane">Projection plane</param>
        /// <returns>Angle measurement</returns>
        public static float CalculateProjectedAngle(Vector3 line1, Vector3 line2, Plane projectionPlane)
        {
            line1 = Vector3.ProjectOnPlane(line1, projectionPlane.normal);
            line2 = Vector3.ProjectOnPlane(line2, projectionPlane.normal);
            var angle = Vector3.Angle(line1, line2);
            return angle;
        }

        /// <summary>
        /// Generate Line of best fit from list of points.
        /// </summary>
        /// <param name="points">Source points collection</param>
        /// <returns>Returns best fit line points</returns>
        public static List<Vector3> GenerateLinearBestFit(List<Vector3> points)
        {
            // TODO :
            int numPoints = points.Count;

            //Calculate the mean of the x -values and the mean of the y -values.
            double meanX = points.Average(point => point.x);
            double meanY = points.Average(point => point.y);
            double meanZ = points.Average(point => point.z);

            //slope a of the line of best fit:
            double sumXSquared = points.Sum(point => point.x * point.x);
            double sumXY = points.Sum(point => point.x * point.y);
            double a = (sumXY / numPoints - meanX * meanY) / (sumXSquared / numPoints - meanX * meanX);

            //Compute the y -intercept of the line by using the formula:
            double b = (meanY - a * meanX);

            // Use the slope a and the y -intercept b to form the equation of the line y=a * point.x - b
            return points.Select(point => new Vector3(point.x, (float)(a * point.x - b), point.z)).ToList();
        }

        public Vector3 GetBestFitDirection(Vector3[] points)
        {
            if(points.Length < 3 )
            {
                return points[1] - points[0];
            }
            Vector3 averagePoint = GetAverage(points);
            Vector3 beginning = GetAverage(new Vector3[] { points[0], points[1], points[2] });
            return averagePoint - beginning;
        }

        private Vector3 GetAverage(Vector3[] points)
        {
            float sumX = 0f;
            float sumY = 0f;
            float sumZ = 0f;
            foreach (Vector3 v in points)
            {
                sumX += v.x;
                sumY += v.y;
                sumZ += v.z;
            }
            return new Vector3(sumX / points.Length, sumY / points.Length, sumZ / points.Length);
        }

        #endregion

        #region  Private Methods

        #endregion
    }
}
