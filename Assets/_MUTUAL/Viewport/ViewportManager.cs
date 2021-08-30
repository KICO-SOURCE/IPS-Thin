#region Usings

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

        private List<IViewport> viewports;
        private readonly ViewportFactory viewportFactory;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of viewports.
        /// </summary>
        public List<IViewport> Viewports
        {
            get
            {
                return viewports;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of viewport manager.
        /// </summary>
        public ViewportManager(ViewportFactory viewportFactory)
        {
            this.viewportFactory = viewportFactory;
            viewports = new List<IViewport>();
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
            viewports.ForEach(v => {
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
            viewports.ForEach(v => v.Deactivate());
            viewports.Clear();

            viewports.AddRange(viewportFactory.Viewports);
        }

        public void CreateViewports()
        {
            // TODO : Dummy viewports for testing
            foreach (var viewport in viewports)
            {
                viewport.CreateViews();
            }
        }

        #endregion
    }
}
