#region Usings

using Assets.CaseFile;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// A viewport class for visualize native alignments.
    /// </summary>
    public class NativeViewport : Viewport
    {
        #region Private Members

        private _3DView coronalFemurView;
        private _3DView coronalTibiaView;
        private _3DView axialFemurView;
        private _3DView sagittalTibiaView;
        private Patient patient;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of Viewport.
        /// </summary>
        public NativeViewport(Patient patient)
        {
            this.patient = patient;
            coronalFemurView = new _3DView() { Postion = new Vector2(0, 0.475f), Size = new Vector2(0.5f, 0.475f) };
            coronalTibiaView = new _3DView() { Postion = new Vector2(0.5f, 0.475f), Size = new Vector2(0.5f, 0.475f) };
            axialFemurView = new _3DView() { Postion = new Vector2(0, 0), Size = new Vector2(0.5f, 0.475f), RotationAngle = -90 };
            sagittalTibiaView = new _3DView() { Postion = new Vector2(0.5f, 0), Size = new Vector2(0.5f, 0.475f)};
            Views.Add(coronalFemurView);
            Views.Add(coronalTibiaView);
            Views.Add(axialFemurView);
            Views.Add(sagittalTibiaView);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public override void CreateViews()
        {
            var meshes = patient.GetMeshes("DistalFemur", "ProximalTibia");

            var origin = patient.GetLandmarkPosition("posteriorApex");
            InitialiseMeshes(origin, patient.MeshGeoms);

            var mask = GetCullingMask("DistalFemur");
            origin = patient.GetLandmarkPosition("femoralCenter");
            Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis,
                                            out var mlAxis, out var apAxis);
            coronalFemurView.InitialiseView(mask, origin, apAxis);
            axialFemurView.InitialiseView(mask, origin, siAxis);

            mask = GetCullingMask("ProximalTibia");
            origin = patient.GetLandmarkPosition("PCLInsertion");
            Ips.Utils.MeasurementUtils.GetTibiaAxes(patient, out siAxis, out mlAxis, out apAxis);
            coronalTibiaView.InitialiseView(mask, origin, -apAxis);
            sagittalTibiaView.InitialiseView(mask, origin, mlAxis);
            base.CreateViews();
        }

        #endregion
    }
}