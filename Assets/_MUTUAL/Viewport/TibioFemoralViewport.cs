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
            coronalFemurView = new _3DView(8) { Postion = new Vector2(0, 0.475f), Size = new Vector2(0.25f, 0.475f) };
            coronalTibiaView = new _3DView(9) { Postion = new Vector2(0, 0), Size = new Vector2(0.25f, 0.475f) };
            axialFemurView = new _3DView(10) { Postion = new Vector2(0.25f, 0.475f), Size = new Vector2(0.25f, 0.475f) };
            axialTibiaView = new _3DView(11) { Postion = new Vector2(0.25f, 0), Size = new Vector2(0.25f, 0.475f) };
            sagittalFemurView = new _3DView(12) { Postion = new Vector2(0.5f, 0.475f), Size = new Vector2(0.25f, 0.475f) };
            sagittalTibiaView = new _3DView(13) { Postion = new Vector2(0.5f, 0), Size = new Vector2(0.25f, 0.475f) };
            longlegView = new _3DView(14) { Postion = new Vector2(0.75f, 0),
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
            if (patient.Landmarks.Any(lm => lm.Type == "femoralCenter"))
            {
                var origin = patient.Landmarks.FirstOrDefault(lm => lm.Type == "femoralCenter").Position;
                Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis, out var mlAxis, out var apAxis);
                var meshes = patient.MeshGeoms.Where(m => m.Key == "DistalFemur").ToDictionary(x => x.Key, x => x.Value);
                coronalFemurView.InitialiseView(meshes, ViewType.CoronalView, origin, siAxis, mlAxis, apAxis);
                axialFemurView.InitialiseView(meshes, ViewType.AxialView, origin, siAxis, mlAxis, apAxis);
                sagittalFemurView.InitialiseView(meshes, ViewType.SagittalView, origin, siAxis, mlAxis, apAxis);

                meshes = patient.MeshGeoms.Where(m => m.Key != "Patella").ToDictionary(x => x.Key, x => x.Value);
                longlegView.InitialiseView(meshes, ViewType.CoronalView, origin, siAxis, mlAxis, apAxis);

                origin = patient.Landmarks.FirstOrDefault(lm => lm.Type == "PCLInsertion").Position;
                Ips.Utils.MeasurementUtils.GetTibiaAxes(patient, out siAxis, out mlAxis, out apAxis);
                meshes = patient.MeshGeoms.Where(m => m.Key == "ProximalTibia").ToDictionary(x => x.Key, x => x.Value);
                coronalTibiaView.InitialiseView(meshes, ViewType.CoronalView, origin, siAxis, mlAxis, -apAxis);
                axialTibiaView.InitialiseView(meshes, ViewType.AxialView, origin, -siAxis, mlAxis, apAxis);
                sagittalTibiaView.InitialiseView(meshes, ViewType.SagittalView, origin, siAxis, -mlAxis, apAxis);
            }
            base.CreateViews();
        }

        #endregion
    }
}