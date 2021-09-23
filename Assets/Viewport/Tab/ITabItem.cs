#region Usings

using System;

#endregion

namespace Assets.Viewport.Tab
{
    /// <summary>
    /// Interface for tab item.
    /// </summary>
    public interface ITabItem : IUIElement
    {
        #region Properties

        /// <summary>
        /// Gets or sets the tilte of tab item.
        /// </summary>
        string Title { get; set; }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event handler for selected tab.
        /// </summary>
        event EventHandler<TabSelectedEventArgs> TabSelected;

        #endregion

        #region Methods

        /// <summary>
        /// Sets the tab selected.
        /// </summary>
        void OnSelected();

        /// <summary>
        /// Sets the tab deselected.
        /// </summary>
        void OnDeSelected();

        #endregion
    }
}