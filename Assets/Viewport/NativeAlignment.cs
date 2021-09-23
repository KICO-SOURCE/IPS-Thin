using Assets.CaseFile;
using Assets.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Viewport
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
            Views.Add(new AnatomicalTableView() { Postion = new Vector2(0, 0), Size = new Vector2(50, 50) });
            Views.Add(new CoronalAlignmentTableView() { Postion = new Vector2(0, 0), Size = new Vector2(100, 100) });
            Views.Add(new ImageView() { Postion = new Vector2(0, 0), Size = new Vector2(50, 50) });

            coronalView = new _3DView() { Postion = new Vector2(0.7f, 0),
                                           Size = new Vector2(0.15f, 0.95f),
                                           CameraPostion = 2000};
            sagittalView = new _3DView() { Postion = new Vector2(0.85f, 0),
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
            Ips.Utils.MeasurementUtils.GetFemurAxes(patient, out var siAxis,
                                            out var mlAxis, out var apAxis);

            var origin = patient.GetLandmarkPosition("posteriorApex");
            InitialiseMeshes(origin, patient.MeshGeoms);

            var components = GetComponents();
            InitialiseComponents(components, GetComponentTransforms());

            var layers = patient.MeshGeoms.Keys.ToList();
            layers.AddRange(components.Keys.ToList());

            List<string> lineMaskIds = new List<string>();
            lineMaskIds.Add(StringConstants.FemSIAxis);
            lineMaskIds.Add(StringConstants.FemMechanicalAxis);
            lineMaskIds.Add(StringConstants.TibSIAxis);

            layers.AddRange(lineMaskIds);

            var mask = GetCullingMask(layers.ToArray());
            origin = patient.GetLandmarkPosition("femoralCenter");
            Dictionary<string, int> keyValuePairs = new Dictionary<string, int>();
            coronalView.InitialiseView(mask, origin, apAxis);
            sagittalView.InitialiseView(mask, origin, mlAxis);

            base.CreateViews();

        //    DrawLine();
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
