#region Usings

using Assets.CaseFile;
using System.Collections.Generic;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// Viewport factory class.
    /// </summary>
    public class ViewportFactory
    {
        #region Private Members

        private List<IViewport> viewports;
        private readonly Patient patient;
        private readonly Project project;

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
        /// Creates new instance of viewport factory.
        /// </summary>
        public ViewportFactory(Patient patient, Project project)
        {
            this.patient = patient;
            this.project = project;
            viewports = new List<IViewport>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Populate report viewports.
        /// </summary>
        public void PopulateReportViewports()
        {
            viewports.Clear();
            viewports.Add(new NativeAlignment(patient) { Title = "NativeAlignment" });
            viewports.Add(new NativeViewport(patient) { Title = "VP2" });
            viewports.Add(new TibioFemoralViewport(patient) { Title = "VP3" });
            viewports.Add(new TibioFemoralViewport(patient) { Title = "VP4" });
            viewports.Add(new PatellaViewport(patient) { Title = "VP5" });
        }

        /// <summary>
        /// Populate knee plan viewports.
        /// </summary>
        public void PopulatePlanViewports(int planIndex = 0)
        {
            viewports.Clear();
            viewports.Add(new KneePlanViewport(patient, project, planIndex));
        }

        #endregion
    }
}
