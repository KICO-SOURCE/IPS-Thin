using Assets.CaseFile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Viewport
{
    class KneePlanViewport : Viewport
    {
        #region Private Members

        private _3DView coronalView;
        private _3DView femurComponent;
        private _3DView tibiaTrayComponent;
        private _3DView tibiaInsertComponent;
        private _3DView patellaComponent;
        private readonly Patient patient;
        private readonly Project project;
        private readonly int planIndex;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new CoronalViewport instance.
        /// </summary>
        public KneePlanViewport(Patient patient,
                Project project, int planIndex)
        {
            this.patient = patient;
            this.project = project;
            this.planIndex = planIndex;

            coronalView = new _3DView() { CameraPostion = 500, Background = Color.black };
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

            var components = GetComponents();
            InitialiseComponents(components, GetComponentTransforms());

            var layers = patient.MeshGeoms.Keys.ToList();
            layers.AddRange(components.Keys.ToList());
            var mask = GetCullingMask(layers.ToArray());

            origin = patient.GetLandmarkPosition("femoralCenter");
            coronalView.InitialiseView(mask, origin, apAxis);
            base.CreateViews();
        }

        #endregion

        #region Private Methods

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
