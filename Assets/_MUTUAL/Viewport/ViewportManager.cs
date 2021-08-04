#region Usings

using Assets.CaseFile;
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
        private readonly Patient patient;

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
        public ViewportManager(Patient patient)
        {
            this.patient = patient;
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

        #endregion

        #region Private Methods

        /// <summary>
        /// Populate viewports.
        /// </summary>
        private void PopulateViewports()
        {
            // TODO : Dummy viewports for testing
            viewports.Add(new NativeAlignment(patient) { Title = "NativeAlignment" });
            viewports.Add(new NativeViewport(patient) { Title = "VP2" });
            viewports.Add(new TibioFemoralViewport(patient) { Title = "VP3" });
            viewports.Add(new TibioFemoralViewport(patient) { Title = "VP4" });
            viewports.Add(new PatellaViewport(patient) { Title = "VP5" });
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
