using Assets.CaseFile;
using System.Linq;
using UnityEngine;

namespace Assets._MUTUAL.Viewport
{
    public class NativeAlignment : Viewport
    {
        #region Private Members

        private _3DView coronalView;
        private _3DView sagittalView;
        private Patient patient;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new NativeAlignment instance.
        /// </summary>
        public NativeAlignment(Patient patient)
        {
            this.patient = patient;
            Views.Add(new AnatomicalTableView() { Postion = new Vector2(0, 0), Size = new Vector2(50, 50) });
            Views.Add(new CoronalAlignmentTableView() { Postion = new Vector2(0, 0), Size = new Vector2(100, 100) });
            Views.Add(new ImageView() { Postion = new Vector2(0, 0), Size = new Vector2(50, 50) });

            coronalView = new _3DView() { Postion = new Vector2(0.7f, 0),
                                           Size = new Vector2(0.15f, 0.95f),
                                           CameraPostion = 2000};
            sagittalView = new _3DView() { Postion = new Vector2(0.85f, 0),
                                            Size = new Vector2(0.15f, 0.95f),
                                            CameraPostion = 2000,
                                            RotationAngle = 90};
            Views.Add(coronalView);
            Views.Add(sagittalView);
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
            sagittalView.InitialiseView(mask, origin, mlAxis);
            base.CreateViews();
        }

        #endregion
    }
}
