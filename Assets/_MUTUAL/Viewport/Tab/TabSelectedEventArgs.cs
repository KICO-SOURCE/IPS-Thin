#region Usings

using System;

#endregion

namespace Assets._MUTUAL.Viewport.Tab
{
    /// <summary>
    /// Tab item selected event args class.
    /// </summary>
    public class TabSelectedEventArgs : EventArgs
    {
        #region Public Memeber

        /// <summary>
        /// The selected tab to activate.
        /// </summary>
        public ITabItem TabItem;

        #endregion
    }
}
