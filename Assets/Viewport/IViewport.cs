#region Usings

using System.Collections.Generic;

#endregion

namespace Assets.Viewport
{
    /// <summary>
    /// Interface for viewport.
    /// </summary>
    public interface IViewport : IUIElement
    {
        #region Properties

        /// <summary>
        /// Gets the title of viewport.
        /// </summary>
        string Title { get; }

        /// <summary>
        /// Gets the list of viewports.
        /// </summary>
        List<IView> Views { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Create viewports.
        /// </summary>
        void CreateViews();

        #endregion
    }
}