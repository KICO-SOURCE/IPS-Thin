using System.Collections.Generic;
using UnityEngine;

namespace Assets._MUTUAL.Measurement
{
    /// <summary>
    /// Line render data class.
    /// </summary>
    public class LineRenderData : ILineRenderData
    {
        #region Public Properties

        /// <summary>
        /// Gets or Sets the list of line render points.
        /// </summary>
        public List<Vector3> Points 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the color of line.
        /// </summary>
        public Color LineColor 
        { 
            get; 
            set; 
        }

        /// <summary>
        /// Gets or sets the thickness of line.
        /// </summary>
        public float Thickness 
        { 
            get; 
            set; 
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of line render data.
        /// </summary>
        public LineRenderData()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Prepare the line data.
        /// </summary>
        public void PrepareData()
        {
            // TODO : Fetch landamarks mentioned in lide model data and fill in point collection.
        }

        #endregion
    }
}
