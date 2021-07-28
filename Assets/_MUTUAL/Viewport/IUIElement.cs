#region Usings

using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// Interface for UI element.
    /// </summary>
    public interface IUIElement
    {
        #region Proprties

        /// <summary>
        /// Gets the parent.
        /// </summary>
        GameObject Parent { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Activates the UI.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates the UI.
        /// </summary>
        void Deactivate();

        #endregion
    }
}
