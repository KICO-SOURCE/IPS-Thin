#region Usings

using Assets.CaseFile;
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

        GameObject Mesh;

        private void Start()
        {
            DisplaySample();
            var verticesBtn = ShowVerticesBtn.GetComponent<Button>();
            verticesBtn.onClick.AddListener(ShowVerticesList);
        }

        /// <summary>
        /// Display a sample stl in UI
        /// </summary>
        private void DisplaySample()
        {
            var path = Application.dataPath + @"\Sandbox\Sample\pelvis.stl";
            Mesh = LoadStl(path);
            SetCameraAndLightPosition(Mesh);
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

            var pos = new Vector3(center.x, center.y + bound.size.y, center.z);
            var rotation = new Vector3(90, 0, 0);
            Camera.transform.position = pos;
            Camera.transform.eulerAngles = rotation;
            Camera.transform.LookAt(center, Vector3.up);

            Light.transform.position = pos;
            Light.transform.eulerAngles = rotation;

            PivotPoint.transform.position = Vector3.zero;
        }

        private void ShowVerticesList()
        {
            MeshPointDataManager meshPointDataManager = new MeshPointDataManager();
            meshPointDataManager.ShowMeshVerticesList(UIParent, Mesh);
        }

        #endregion
    }
}
