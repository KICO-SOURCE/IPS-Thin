#region Usings

using Assets.CaseFile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets.Viewport
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
            axialFemurView = new _3DView() { Postion = new Vector2(0.25f, 0.475f), Size = new Vector2(0.25f, 0.475f), RotationAngle = -90 };
            axialTibiaView = new _3DView() { Postion = new Vector2(0.25f, 0), Size = new Vector2(0.25f, 0.475f), RotationAngle = -90 };
            sagittalFemurView = new _3DView() { Postion = new Vector2(0.5f, 0.475f), Size = new Vector2(0.25f, 0.475f)};
            sagittalTibiaView = new _3DView() { Postion = new Vector2(0.5f, 0), Size = new Vector2(0.25f, 0.475f)};
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

            var components = GetComponents();
            InitialiseComponents(components, GetComponentTransforms());

            var layers = patient.MeshGeoms.Keys.Where(k => k != "Patella").ToList();
            layers.AddRange(components.Keys.ToList());

            var mask = GetCullingMask("DistalFemur", "Femur Component");
            origin = patient.GetLandmarkPosition("femoralCenter");
            Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis,
                                            out var mlAxis, out var apAxis);

            coronalFemurView.InitialiseView(mask, origin, apAxis);
            axialFemurView.InitialiseView(mask, origin, siAxis);
            sagittalFemurView.InitialiseView(mask, origin, mlAxis);

            mask = GetCullingMask(layers.ToArray());

            longlegView.InitialiseView(mask, origin, apAxis);

            mask = GetCullingMask("ProximalTibia", "Tibia Tray", "Tibia Insert");
            origin = patient.GetLandmarkPosition("PCLInsertion");
            Ips.Utils.MeasurementUtils.GetTibiaAxes(patient, out siAxis, out mlAxis, out apAxis);
            coronalTibiaView.InitialiseView(mask, origin, -apAxis);
            axialTibiaView.InitialiseView(mask, origin, -siAxis);
            sagittalTibiaView.InitialiseView(mask, origin, -mlAxis);
            base.CreateViews();
        }

        private Dictionary<string, Mesh> GetComponents()
        {
            return Project.Instance.PlanImplants[2].ToDictionary(x =>
                                    x.Value.Name, x => x.Value.Geometry);
        }

        private Dictionary<string, Transform> GetComponentTransforms()
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