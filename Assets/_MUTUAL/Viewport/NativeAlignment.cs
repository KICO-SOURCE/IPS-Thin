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
            Views.Add(new TableView() { Postion = new Vector2(0, 0), Size = new Vector2(50, 50) });
            Views.Add(new ImageView() { Postion = new Vector2(0, 0), Size = new Vector2(50, 50) });

            coronalView = new _3DView(8) { Postion = new Vector2(0.7f, 0),
                                           Size = new Vector2(0.15f, 0.95f),
                                           CameraPostion = 2000};
            sagittalView = new _3DView(9) { Postion = new Vector2(0.85f, 0),
                                            Size = new Vector2(0.15f, 0.95f),
                                            CameraPostion = 2000};
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
            if (patient.Landmarks.Any(lm => lm.Type == "femoralCenter"))
            {
                var origin = patient.Landmarks.FirstOrDefault(lm => lm.Type == "femoralCenter").Position;
                Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis, out var mlAxis, out var apAxis);

                coronalView.InitialiseView(patient.MeshGeoms, ViewType.CoronalView, origin, siAxis, mlAxis, apAxis);
                sagittalView.InitialiseView(patient.MeshGeoms, ViewType.SagittalView, origin, siAxis, mlAxis, apAxis);
            }
            base.CreateViews();
        }

        #endregion
    }
}
