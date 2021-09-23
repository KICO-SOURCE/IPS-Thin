#region Usings

using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Assets.CaseFile;
using UnityEngine.SceneManagement;
using Assets.Measurement;
using Assets.Utils;

#endregion

namespace Assets.Viewport
{
    /// <summary>
    /// Viewport class.
    /// </summary>
    public class Viewport : IViewport
    {
        #region Private Members

        private Vector3 origin;
        private GameObject parent;
        private GameObject meshPrefab;
        private GameObject meshView;
        private const string prefabPath = "Prefabs/3DView";
        private const string parentTag = "MeshParent";
        private const string meshMaterialPath = "Materials/BoneTransparent";
        private const string componentMaterialPath = "Materials/ImplantOpaque";
        private const string lineMaterialPath = "Materials/PredictionLine";

        private Dictionary<string, Mesh> meshes;
        private Dictionary<string, Mesh> components;
        private Dictionary<string, Transform> componentPositions;
        private Dictionary<string, int> layers;
        private List<GameObject> meshObjects;
        private List<GameObject> lineObjects;
        private int nextLayer = 8;

        #endregion

        #region Public Properties

        /// <summary>
        /// Getst the title of the viewport.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the list of viewports.
        /// </summary>
        public List<IView> Views
        {
            get;
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public GameObject Parent
        {
            get;
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of Viewport.
        /// </summary>
        public Viewport()
        {
            Views = new List<IView>();
            meshObjects = new List<GameObject>();
            layers = new Dictionary<string, int>();
            meshPrefab = Resources.Load<GameObject>(prefabPath);
            lineObjects = new List<GameObject>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public void Activate()
        {
            LoadMeshes();
            DrawLine();
            foreach (var view in Views)
            {
                view.Activate();
            }
        }

        /// <summary>
        /// Deactivates the viewport.
        /// </summary>
        public void Deactivate()
        {
            UnloadMeshes();
            RemoveLines();
            foreach (var view in Views)
            {
                view.Deactivate();
            }
        }

        /// <summary>
        /// Create viewports.
        /// </summary>
        public virtual void CreateViews()
        {
            foreach(var view in Views)
            {
                view.CreateView();
            }
        }

        #endregion

        #region Private Methods

        protected void InitialiseMeshes(Vector3 origin,
                            Dictionary<string, Mesh> meshes)
        {
            this.origin = origin;
            this.meshes = meshes;
            if (meshes == null) return;

            foreach (var mesh in meshes)
            {
                layers.Add(mesh.Key, nextLayer);
                nextLayer++;
            }
            if (parent == null)
            {
                parent = GameObject.FindGameObjectWithTag(parentTag);
            }

            List<string> LineList = new List<string>();
            LineList.Add(StringConstants.FemSIAxis);
            LineList.Add(StringConstants.FemMechanicalAxis);
            LineList.Add(StringConstants.TibSIAxis);
            foreach (var line in LineList)
            {
                layers.Add(line, nextLayer);
                nextLayer++;
            }

        }

        protected void InitialiseComponents(Dictionary<string, Mesh> meshes,
                                            Dictionary<string, Transform> transforms)
        {
            components = meshes;
            componentPositions = transforms;
            if (components == null) return;

            foreach (var component in components)
            {
                layers.Add(component.Key, nextLayer);
                nextLayer++;
            }

            if (parent == null)
            {
                parent = GameObject.FindGameObjectWithTag(parentTag);
            }
        }

        protected void LoadMeshes()
        {
            meshView = UnityEngine.Object.Instantiate(meshPrefab, parent.transform);
            var lights = meshView.GetComponentsInChildren<Light>();

            lights[0].transform.position = origin + Vector3.left * 2000;
            lights[0].transform.LookAt(origin, Vector3.left);
            //lights[1].transform.position = origin + Vector3.right * 2000;
            //lights[1].transform.LookAt(origin, Vector3.right);
            //lights[2].transform.position = origin + Vector3.forward * 2000;
            //lights[2].transform.LookAt(origin, Vector3.forward);
            //lights[3].transform.position = origin + Vector3.back * 2000;
            //lights[3].transform.LookAt(origin, Vector3.back);
            //lights[4].transform.position = origin + Vector3.up * 2000;
            //lights[4].transform.LookAt(origin, Vector3.up);
            //lights[5].transform.position = origin + Vector3.down * 2000;
            //lights[5].transform.LookAt(origin, Vector3.down);

            var material = Resources.Load<Material>(meshMaterialPath);

            if (meshes != null)
            {
                foreach (var mesh in meshes)
                {
                    var go = new GameObject(mesh.Key, typeof(MeshFilter), typeof(MeshRenderer));
                    go.transform.parent = meshView.transform;
                    go.GetComponent<MeshFilter>().mesh = mesh.Value;
                    go.GetComponent<MeshRenderer>().material = material;
                    go.layer = layers[mesh.Key];

                    go.SetActive(true);
                    meshObjects.Add(go);
                }
            }

            material = Resources.Load<Material>(componentMaterialPath);

            if (components != null)
            {
                foreach (var component in components)
                {
                    var go = new GameObject(component.Key, typeof(MeshFilter), typeof(MeshRenderer));
                    go.transform.parent = meshView.transform;
                    go.GetComponent<MeshFilter>().mesh = component.Value;
                    go.GetComponent<MeshRenderer>().material = material;
                 //   go.GetComponent<MeshRenderer>().material.color = Color.blue;
                    go.layer = layers[component.Key];

                    go.transform.localPosition = componentPositions[component.Key].position;
                    go.transform.localEulerAngles = componentPositions[component.Key].eulerAngles;

                    go.SetActive(true);
                    meshObjects.Add(go);
                }
            }
        }

        protected void UnloadMeshes()
        {
            foreach (var go in meshObjects)
            {
                go.SetActive(false);
                UnityEngine.Object.DestroyImmediate(go);
            }
            meshObjects.Clear();

            if(meshView != null)
            {
                meshView.SetActive(false);
                UnityEngine.Object.DestroyImmediate(meshView);
                meshView = null;
            }
        }

        protected int GetCullingMask(params string[] meshes)
        {
            if (meshes == null || meshes.Length <= 0) return -1;
            if (layers.Count <= 0) return -1;

            var layer = layers.First(l => l.Key == meshes.First()).Value;

            var cullingMask = (1 << layer);
            for (int i = 0; i < meshes.Length; i++)
            {
                cullingMask |= (1 << layers[meshes[i]]);
            }

            return cullingMask;
        }

        protected int GetLayerID(string layerKey)
        {
            return layers.FirstOrDefault(i => i.Key == layerKey).Value;
        }

        protected void DrawLine()
        {
            var femoralCenter = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "femoralCenter");
            var hipCenter = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "hipCenter");

            var lateralEpicondyle = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "lateralEpicondyle");
            var greaterTrochanter = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "greaterTrochanter");


            var tubercle = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "Tubercle");
            var pclInsertion = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "PCLInsertion");
            Vector3 tibCenter = (tubercle.Position + pclInsertion.Position) * 0.5f;

            var medMal = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "medialMalleolus");
            var latMal = Patient.Instance.Landmarks.FirstOrDefault(lm => lm.Type == "lateralMalleolus");
            Vector3 ankleCenter = (medMal.Position + latMal.Position) * 0.5f;

            //Draw first line
            var parent = GameObject.FindGameObjectWithTag(parentTag);

            GameObject femSIObj = new GameObject("FemSI");
            femSIObj.transform.SetParent(parent.transform);
            femSIObj.layer = GetLayerID(StringConstants.FemSIAxis);
            lineObjects.Add(femSIObj);

            List<Vector3> points = new List<Vector3>();
            points.Add(femoralCenter.Position);
            points.Add(hipCenter.Position);
            LineRenderData lineData = new LineRenderData();
            lineData.LineColor = new Color(0, 255, 0,50);
            lineData.Points = points;
            lineData.Thickness = 5;

            MeasurementManager measurementManager = new MeasurementManager();
            measurementManager.DrawLine(lineData, femSIObj);

            // Draw second line
            GameObject femMechAxisObj = new GameObject("FemMechAxis");
            femMechAxisObj.transform.SetParent(parent.transform);
            femMechAxisObj.layer = GetLayerID(StringConstants.FemMechanicalAxis);
            lineObjects.Add(femMechAxisObj);

            points = new List<Vector3>();
            points.Add(lateralEpicondyle.Position);
            points.Add(greaterTrochanter.Position);
            lineData = new LineRenderData();
            lineData.LineColor = Color.yellow;
            lineData.Points = points;
            lineData.Thickness = 5;

            measurementManager = new MeasurementManager();
            measurementManager.DrawLine(lineData, femMechAxisObj);

            // Draw third line
            GameObject TibSIAxisObj = new GameObject("TibSIAxis");
            TibSIAxisObj.transform.SetParent(parent.transform);
            TibSIAxisObj.layer = GetLayerID(StringConstants.TibSIAxis);
            lineObjects.Add(TibSIAxisObj);

            points = new List<Vector3>();
            points.Add(tibCenter);
            points.Add(ankleCenter);
            lineData = new LineRenderData();
            lineData.LineColor = new Color(0, 255, 255,60);
            lineData.Points = points;
            lineData.Thickness = 5;

            measurementManager = new MeasurementManager();
            measurementManager.DrawLine(lineData, TibSIAxisObj);
        }

        protected void RemoveLines()
        {
            foreach (var line in lineObjects)
            {
                line.SetActive(false);
                UnityEngine.Object.DestroyImmediate(line);
            }
            lineObjects.Clear();
        }

        #endregion
    }
}
