using Assets.CaseFile;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets._MUTUAL.Viewport
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

            coronalView = new _3DView();
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
            return project.PlanImplants[planIndex].ToDictionary(x =>
                                    x.Value.Name, x => x.Value.Geometry);
        }

        private Dictionary<string, Transform> GetComponentTransforms()
        {
            var femTrans = project.PlanComponentPosition[planIndex]["FemurEuler"].GetTransfrom();
            var tibTrans = project.PlanComponentPosition[planIndex]["TibiaEuler"].GetTransfrom();
            var patTrans = project.PlanComponentPosition[planIndex]["PatellaEuler"].GetTransfrom();

            var transforms = new Dictionary<string, Transform>();

            foreach(var implant in project.PlanImplants[planIndex])
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
