using Assets.CaseFile;
using System.Linq;

namespace Assets._MUTUAL.Viewport
{
    class CoronalViewport : Viewport
    {
        #region Private Members

        private _3DView coronalView;
        private _3DView femurComponent;
        private _3DView tibiaTrayComponent;
        private _3DView tibiaInsertComponent;
        private _3DView patellaComponent;
        private Patient patient;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new CoronalViewport instance.
        /// </summary>
        //private Project project;
        public CoronalViewport(Patient patient, Project project)
        {
            this.patient = patient;

            coronalView = new _3DView();
            Views.Add(coronalView);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public override void CreateViews()
        {
            Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis,
                                            out var mlAxis, out var apAxis);

            var origin = patient.GetLandmarkPosition("posteriorApex");

            InitialiseMeshes(origin, patient.MeshGeoms);

            var mask = GetCullingMask(patient.MeshGeoms.Keys.ToArray());
            origin = patient.GetLandmarkPosition("femoralCenter");
            coronalView.InitialiseView(mask, origin, apAxis);
            base.CreateViews();
        }

        #endregion
    }
}
