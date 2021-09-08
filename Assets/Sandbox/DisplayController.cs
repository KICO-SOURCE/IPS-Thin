#region Usings

using Assets.CaseFile;
using Assets.Geometries;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Assets.Sandbox
{
    /// <summary>
    /// Display controller for the sandbox 3d models.
    /// </summary>
    public class DisplayController : MonoBehaviour
    {
        #region Private Fields

        private const string layer = "ThreeDLayer";

        #endregion

        #region Public Fields

        public GameObject Parent;
        public Material BoneMaterial;
        public Camera Camera;
        public Light Light;
        public GameObject PivotPoint;
        public GameObject ShowVerticesBtn;
        public GameObject UIParent;

        #endregion

        #region Private Methods

        GameObject Mesh => GeometryManager.Instance.SelectedGeometry?.Object;

        private void Start()
        {
            DisplayMesh();
            ShowVerticesBtn.SetActive(false);
            var verticesBtn = ShowVerticesBtn.GetComponent<Button>();
            verticesBtn.onClick.AddListener(ShowVerticesList);
        }

        /// <summary>
        /// Display a mesh in UI
        /// </summary>
        private void DisplayMesh()
        {
            //var path = Application.dataPath + @"\Sandbox\Sample\pelvis.stl";
            //Mesh = LoadStl(path);

            var selectedGeometry = GeometryManager.Instance.SelectedGeometry;

            selectedGeometry.DisplayObjects(Parent.transform,
                                LayerMask.NameToLayer(layer));
            SetCameraAndLightPosition(selectedGeometry.Object);
        }

        /// <summary>
        /// Load stl file from the given path
        /// </summary>
        /// <param name="path">File path</param>
        /// <returns>Return mesh game object created for the stl</returns>
        private GameObject LoadStl(string path)
        {
            MeshData output = MeshGeometryFunctions.ReadStl(path);
            GameObject mesh = new GameObject("Meshdata");
            mesh.transform.SetParent(Parent.transform);
            var meshFilter = mesh.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = output.ToMesh();

            var renderer = mesh.AddComponent<MeshRenderer>();
            renderer.material = BoneMaterial;
            mesh.layer = LayerMask.NameToLayer(layer);

            return mesh;
        }

        /// <summary>
        /// Set camera and light for the mesh
        /// </summary>
        /// <param name="mesh"></param>
        private void SetCameraAndLightPosition(GameObject mesh)
        {
            var renderer = mesh.GetComponent<MeshRenderer>();
            var bound = renderer.bounds;
            var center = bound.center;

            var direction = mesh.transform.TransformVector(Vector3.down);
            var pos = center + direction * bound.size.y;

            Camera.transform.position = pos;
            Camera.transform.LookAt(center, direction);

            //var axis = Camera.transform.TransformVector(Vector3.forward);
            //Camera.transform.RotateAround(center, axis, 45);

            Light.transform.position = Camera.transform.position;
            Light.transform.eulerAngles = Camera.transform.eulerAngles;

            PivotPoint.transform.position = Vector3.zero;
        }

        private void ShowVerticesList()
        {
            if (null == Mesh) return;

            MeshPointDataManager meshPointDataManager = new MeshPointDataManager();
            meshPointDataManager.ShowMeshVerticesList(UIParent, Mesh);
        }

        #endregion
    }
}
