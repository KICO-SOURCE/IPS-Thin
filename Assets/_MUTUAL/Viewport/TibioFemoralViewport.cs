#region Usings

using Assets.CaseFile;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// A viewport class for visualize femur and tibia alignments.
    /// </summary>
    public class TibioFemoralViewport : Viewport
    {
        #region Private Members

        private _3DView coronalFemurView;
        private _3DView coronalTibiaView;
        private _3DView axialFemurView;
        private _3DView axialTibiaView;
        private _3DView sagittalFemurView;
        private _3DView sagittalTibiaView;
        private _3DView longlegView;
        private Patient patient;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of Viewport.
        /// </summary>
        public TibioFemoralViewport(Patient patient)
        {
            this.patient = patient;
            coronalFemurView = new _3DView() { Postion = new Vector2(0, 0.475f), Size = new Vector2(0.25f, 0.475f) };
            coronalTibiaView = new _3DView() { Postion = new Vector2(0, 0), Size = new Vector2(0.25f, 0.475f) };
            axialFemurView = new _3DView() { Postion = new Vector2(0.25f, 0.475f), Size = new Vector2(0.25f, 0.475f) };
            axialTibiaView = new _3DView() { Postion = new Vector2(0.25f, 0), Size = new Vector2(0.25f, 0.475f) };
            sagittalFemurView = new _3DView() { Postion = new Vector2(0.5f, 0.475f), Size = new Vector2(0.25f, 0.475f), RotationAngle = 90 };
            sagittalTibiaView = new _3DView() { Postion = new Vector2(0.5f, 0), Size = new Vector2(0.25f, 0.475f), RotationAngle = 90 };
            longlegView = new _3DView() { Postion = new Vector2(0.75f, 0),
                        Size = new Vector2(0.25f, 0.95f), CameraPostion = 2000 };
            Views.Add(coronalFemurView);
            Views.Add(coronalTibiaView);
            Views.Add(axialFemurView);
            Views.Add(axialTibiaView);
            Views.Add(sagittalFemurView);
            Views.Add(sagittalTibiaView);
            Views.Add(longlegView);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public override void CreateViews()
        {
            var meshKeys = patient.MeshGeoms.Keys.Where(k => k != "Patella").ToArray();
            var meshes = patient.GetMeshes(meshKeys);

            var origin = patient.GetLandmarkPosition("posteriorApex");
            InitialiseMeshes(origin, meshes);

            var mask = GetCullingMask("DistalFemur");
            origin = patient.GetLandmarkPosition("femoralCenter");
            Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis,
                                            out var mlAxis, out var apAxis);
            coronalFemurView.InitialiseView(mask, origin, apAxis);
            axialFemurView.InitialiseView(mask, origin, siAxis);
            sagittalFemurView.InitialiseView(mask, origin, mlAxis);

            mask = GetCullingMask(meshKeys);
            longlegView.InitialiseView(mask, origin, apAxis);

            mask = GetCullingMask("ProximalTibia");
            var tubercle = patient.GetLandmarkPosition("Tubercle");
            var pclInsertion = patient.GetLandmarkPosition("PCLInsertion");
            origin = (tubercle + pclInsertion) * 0.5f;
            Ips.Utils.MeasurementUtils.GetTibiaAxes(patient, out siAxis, out mlAxis, out apAxis);
            coronalTibiaView.InitialiseView(mask, origin, -apAxis);
            axialTibiaView.InitialiseView(mask, origin, -siAxis);
            sagittalTibiaView.InitialiseView(mask, origin, -mlAxis);
            base.CreateViews();
        }

        #endregion
    }
}