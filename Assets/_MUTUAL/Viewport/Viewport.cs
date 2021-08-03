#region Usings

using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// Viewport class.
    /// </summary>
    public class Viewport : IViewport
    {

        #region Public Properties

        /// <summary>
        /// Getst the title of the viewport.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of viewports.
        /// </summary>
        public List<IView> Views
        {
            get;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public GameObject Parent
        {
            get;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of Viewport.
        /// </summary>
        public Viewport()
        {
            Views = new List<IView>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public void Activate()
        {
            foreach (var view in Views)
            {
                view.Activate();
            }
        }

        /// <summary>
        /// Deactivates the viewport.
        /// </summary>
        public void Deactivate()
        {
            foreach (var view in Views)
            {
                view.Deactivate();
            }
        }

        /// <summary>
        /// Create viewports.
        /// </summary>
        public virtual void CreateViews()
        {
            foreach(var view in Views)
            {
                view.CreateView();
            }
        }

        #endregion
    }
}
