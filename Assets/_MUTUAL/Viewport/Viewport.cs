#region Usings

using System.Linq;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
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
        private const string meshMaterialPath = "Materials/BoneMaterial";

        private Dictionary<string, Mesh> meshes;
        private Dictionary<string, int> layers;
        private List<GameObject> meshObjects;

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
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport.
        /// </summary>
        public void Activate()
        {
            LoadMeshes();
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

            int layer = 8;
            foreach (var mesh in meshes)
            {
                layers.Add(mesh.Key, layer);
                layer++;
            }
            parent = GameObject.FindGameObjectWithTag(parentTag);
        }

        protected void LoadMeshes()
        {
            if (meshes == null) return;

            meshView = UnityEngine.Object.Instantiate(meshPrefab, parent.transform);
            var lights = meshView.GetComponentsInChildren<Light>();

            lights[0].transform.position = origin + Vector3.left * 2000;
            lights[0].transform.LookAt(origin, Vector3.left);
            lights[1].transform.position = origin + Vector3.right * 2000;
            lights[1].transform.LookAt(origin, Vector3.right);
            lights[2].transform.position = origin + Vector3.forward * 2000;
            lights[2].transform.LookAt(origin, Vector3.forward);
            lights[3].transform.position = origin + Vector3.back * 2000;
            lights[3].transform.LookAt(origin, Vector3.back);
            lights[4].transform.position = origin + Vector3.up * 2000;
            lights[4].transform.LookAt(origin, Vector3.up);
            lights[5].transform.position = origin + Vector3.down * 2000;
            lights[5].transform.LookAt(origin, Vector3.down);

            var material = Resources.Load<Material>(meshMaterialPath);
            foreach (var mesh in meshes)
            {
                var go = new GameObject(mesh.Key, typeof(MeshFilter), typeof(MeshRenderer));
                go.transform.parent = meshView.transform;
                go.GetComponent<MeshFilter>().mesh = mesh.Value;
                go.GetComponent<MeshRenderer>().material = material;
                go.layer = layers[mesh.Key];

                //Need to revisit why this is required
                go.transform.localScale = new Vector3(1, -1, 1);
                go.SetActive(true);
                meshObjects.Add(go);
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
            for (int i = 1; i < meshes.Length; i++)
            {
                cullingMask |= (1 << layers[meshes[i]]);
            }

            return cullingMask;
        }

        #endregion
    }
}
