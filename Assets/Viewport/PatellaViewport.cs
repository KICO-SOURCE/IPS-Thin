#region Usings

using Assets.CaseFile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets.Viewport
{
    /// <summary>
    /// A viewport class for visualize patella alignments.
    /// </summary>
    public class PatellaViewport : Viewport
    {
        #region Private Members

        private _3DView nativePatellaView;
        private _3DView coronalView;
        private _3DView resectedCoronalView;
        private _3DView axialView;
        private _3DView resectedAxialView;
        private Patient patient;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of Viewport.
        /// </summary>
        public PatellaViewport(Patient patient)
        {
            this.patient = patient;
            nativePatellaView = new _3DView() { Postion = new Vector2(0, 0), Size = new Vector2(0.4f, 0.95f) };
            axialView = new _3DView() { Postion = new Vector2(0.4f, 0), Size = new Vector2(0.3f, 0.475f), RotationAngle = -90 };
            coronalView = new _3DView() { Postion = new Vector2(0.4f, 0.475f), Size = new Vector2(0.3f, 0.475f) };
            resectedAxialView = new _3DView() { Postion = new Vector2(0.7f, 0), Size = new Vector2(0.3f, 0.475f), RotationAngle = -90 };
            resectedCoronalView = new _3DView() { Postion = new Vector2(0.7f, 0.475f), Size = new Vector2(0.3f, 0.475f) };

            Views.Add(nativePatellaView);
            Views.Add(axialView);
            Views.Add(coronalView);
            Views.Add(resectedAxialView);
            Views.Add(resectedCoronalView);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public override void CreateViews()
        {
            var origin = patient.GetLandmarkPosition("posteriorApex");
            Ips.Utils.MeasurementUtils.GetPatellaAxes(patient, out var siAxis,
                                            out var mlAxis, out var apAxis);

            var meshes = patient.GetMeshes("Patella");
            InitialiseMeshes(origin, meshes);

            var components = GetComponents();
            InitialiseComponents(components, GetComponentTransforms());

            var layers = patient.MeshGeoms.Keys.ToList();
            layers.AddRange(components.Keys.ToList());

            var mask = GetCullingMask("Patella","Patella Component");

            nativePatellaView.InitialiseView(mask, origin, apAxis);
            coronalView.InitialiseView(mask, origin, apAxis);
            resectedCoronalView.InitialiseView(mask, origin, apAxis);
            axialView.InitialiseView(mask, origin, -siAxis);
            resectedAxialView.InitialiseView(mask, origin, -siAxis);
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