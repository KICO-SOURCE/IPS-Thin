#region Usings

using Assets.CaseFile;
using System;
using System.Collections.Generic;

#endregion

namespace Assets.Viewport
{
    /// <summary>
    /// Viewport factory class.
    /// </summary>
    public class ViewportFactory
    {
        #region Private Members

        private static readonly Lazy<ViewportFactory> _instance = new Lazy<ViewportFactory>(() => new ViewportFactory());
        private readonly Patient patient;
        private readonly Project project;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ViewportFactory Instance
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
        /// Creates new instance of viewport factory.
        /// </summary>
        private ViewportFactory()
        {
            this.patient = Patient.Instance;
            this.project = Project.Instance;
            Viewports = new List<IViewport>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Populate report viewports.
        /// </summary>
        public void PopulateReportViewports()
        {
            Viewports.Clear();
            Viewports.Add(new NativeAlignment(patient) { Title = "NativeAlignment" });
            Viewports.Add(new NativeViewport(patient) { Title = "VP2" });
            Viewports.Add(new TibioFemoralViewport(patient) { Title = "VP3" });
            Viewports.Add(new TibioFemoralViewport(patient) { Title = "VP4" });
            Viewports.Add(new PatellaViewport(patient) { Title = "VP5" });
        }

        /// <summary>
        /// Populate knee plan viewports.
        /// </summary>
        public void PopulatePlanViewports(int planIndex = 0)
        {
            Viewports.Clear();
            Viewports.Add(new KneePlanViewport(patient, project, planIndex));
        }

        #endregion
    }
}
