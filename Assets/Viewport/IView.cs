using UnityEngine;

namespace Assets.Viewport
{
    /// <summary>
    /// Interface for the basic view element.
    /// </summary>
    public interface IView : IUIElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the position of the view.
        /// </summary>
        /// <remarks>
        /// Top-Left position of the view element.
        /// </remarks>
        Vector2 Postion { get; set; }

        /// <summary>
        /// Gets or sets the size of the view.
        /// </summary>
        /// <remarks>
        /// The size of each “View” must be defined by a percentage of the screen. (i.e. 50% width, 100% height)
        /// </remarks>
        Vector2 Size { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// Create the view.
        /// </summary>
        void CreateView();

        #endregion
    }
}