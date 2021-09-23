#region Usings

using Assets.CaseFile;
using Assets.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets.Viewport
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

            origin = patient.GetLandmarkPosition("femoralCenter");
            Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis,
                                            out var mlAxis, out var apAxis);

            var components = GetComponents();
            InitialiseComponents(components, GetComponentTransforms());

            var layers = patient.MeshGeoms.Keys.ToList();
            layers.AddRange(components.Keys.ToList());

            var mask = GetCullingMask("DistalFemur", "Femur Component");

            coronalFemurView.InitialiseView(mask, origin, apAxis);
            axialFemurView.InitialiseView(mask, origin, siAxis);

            mask = GetCullingMask("ProximalTibia", "Tibia Tray", "Tibia Insert");
            origin = patient.GetLandmarkPosition("PCLInsertion");
            Ips.Utils.MeasurementUtils.GetTibiaAxes(patient, out siAxis, out mlAxis, out apAxis);
            coronalTibiaView.InitialiseView(mask, origin, -apAxis);
            sagittalTibiaView.InitialiseView(mask, origin, mlAxis);
            base.CreateViews();

        }

        public Dictionary<string, Mesh> GetComponents()
        {
            return Project.Instance.PlanImplants[2].ToDictionary(x =>
                                    x.Value.Name, x => x.Value.Geometry);
        }

        public Dictionary<string, Transform> GetComponentTransforms()
        {
            var femTrans = Project.Instance.PlanComponentPosition[2]["FemurEuler"].GetTransfrom();
            var tibTrans = Project.Instance.PlanComponentPosition[2]["TibiaEuler"].GetTransfrom();
            var patTrans = Project.Instance.PlanComponentPosition[2]["PatellaEuler"].GetTransfrom();

            var transforms = new Dictionary<string, Transform>();

            foreach (var implant in Project.Instance.PlanImplants[2])
            {
                if (implant.Value.Name.Contains("Femur"))
                {
                    transforms.Add(implant.Value.Name, femTrans);
                }
                else if (implant.Value.Name.Contains("Tibia"))
                {
                    transforms.Add(implant.Value.Name, tibTrans);
                }
                else if (implant.Value.Name.Contains("Patella"))
                {
                    transforms.Add(implant.Value.Name, patTrans);
                }
            }

            return transforms;
        }

        #endregion
    }
}