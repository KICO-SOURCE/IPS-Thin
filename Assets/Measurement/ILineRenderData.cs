using System.Collections.Generic;
using UnityEngine;

namespace Assets.Measurement
{
    /// <summary>
    /// Interface for line render data.
    /// </summary>
    public interface ILineRenderData
    {
        #region Properties

        /// <summary>
        /// Gets or Sets the list of line render points.
        /// </summary>
        List<Vector3> Points { get; set; }

        /// <summary>
        /// Gets or sets the color of line.
        /// </summary>
        Color LineColor { get; set; }

        /// <summary>
        /// Gets or Sets the thickness of line.
        /// </summary>
        float Thickness { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Prepare the line data.
        /// </summary>
        void PrepareData(DataModelsTypes.Lines line);

        #endregion
    }
}
