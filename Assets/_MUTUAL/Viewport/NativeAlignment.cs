using Assets.CaseFile;
using System.Linq;
using UnityEngine;

namespace Assets._MUTUAL.Viewport
{
    public class NativeAlignment : Viewport
    {
        #region Private Members

        private _3DView _3DView;
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

            _3DView = new _3DView() { Postion = new Vector2(0.7f, 0), Size = new Vector2(0.3f, 0.95f) };
            Views.Add(_3DView);
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

                _3DView.InitialiseView(patient.MeshGeoms, ViewType.LongLegView, origin, siAxis, mlAxis, apAxis);
            }
            base.CreateViews();
        }

        #endregion
    }
}
