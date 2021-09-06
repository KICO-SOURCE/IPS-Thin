#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// Viewport manager class.
    /// </summary>
    public class ViewportManager
    {

        #region Private Members

        private static readonly Lazy<ViewportManager> _instance = new Lazy<ViewportManager>(() => new ViewportManager());
        private readonly ViewportFactory viewportFactory;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ViewportManager Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Gets the list of viewports.
        /// </summary>
        public List<IViewport> Viewports { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of viewport manager.
        /// </summary>
        private ViewportManager()
        {
            viewportFactory = ViewportFactory.Instance;
            Viewports = new List<IViewport>();
            PopulateViewports();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Hanlde viewport selection.
        /// </summary>
        /// <param name="viewport"></param>
        public void HandleViewportSelected(string viewport)
        {
            Viewports.ForEach(v => {
                if (v.Title == viewport)
                {
                    v.Activate();
                }
                else
                {
                    v.Deactivate();
                }
            });
        }

        /// <summary>
        /// Populate viewports.
        /// </summary>
        public void PopulateViewports()
        {
            Viewports.ForEach(v => v.Deactivate());
            Viewports.Clear();

            Viewports.AddRange(viewportFactory.Viewports);
        }

        public void CreateViewports()
        {
            // TODO : Dummy viewports for testing
            foreach (var viewport in Viewports)
            {
                viewport.CreateViews();
            }
        }

        #endregion
    }
}
