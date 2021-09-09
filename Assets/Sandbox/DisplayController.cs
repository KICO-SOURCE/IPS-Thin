#region Usings

using Assets.CaseFile;
using Assets.Geometries;
using Assets.Sandbox.MouseActions;
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

            GeometryManager.Instance.DisplaySelectedObjects(Parent.transform,
                                                LayerMask.NameToLayer(layer));
            SetCameraAndLightPosition(GeometryManager.Instance.GetMainObject());
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
            if (null == mesh) return;

            var meshRenderer = mesh.GetComponent<MeshRenderer>();
            var bound = meshRenderer.bounds;

            var renderers = Parent.GetComponentsInChildren<MeshRenderer>();
            foreach (var renderer in renderers)
            {
                bound.Encapsulate(renderer.bounds);
            }

            var center = bound.center;

            var distance = 50 + (bound.size.x > bound.size.y ?
                (bound.size.x > bound.size.z ? bound.size.x : bound.size.z):
                (bound.size.y > bound.size.z ? bound.size.y : bound.size.z));

            var direction = mesh.transform.TransformVector(Vector3.down);
            var camPosition = center + direction * distance;

            Camera.transform.position = camPosition;
            Camera.transform.LookAt(center, direction);
            Camera.orthographicSize = distance;

            var camAxis = Camera.transform.TransformVector(Vector3.up);
            camAxis.Normalize();

            var meshAxis = mesh.transform.TransformVector(Vector3.forward);
            meshAxis.Normalize();

            var axis = Camera.transform.TransformVector(Vector3.forward);
            axis.Normalize();

            var angle = Vector3.SignedAngle(camAxis, meshAxis, axis);
            Debug.Log($"Angle : {angle}");

            Camera.transform.RotateAround(center, axis, angle);

            Light.transform.position = Camera.transform.position;
            Light.transform.eulerAngles = Camera.transform.eulerAngles;
            Light.transform.parent = Camera.transform;

            GameObject position = new GameObject();
            position.transform.position = center;
            Camera.gameObject.GetComponent<OrbitalMouseController>().target = position.transform;
            Camera.gameObject.GetComponent<OrbitalMouseController>().SetDefault();

            PivotPoint.transform.position = Vector3.zero;
        }

        private void ShowVerticesList()
        {
            var mesh = GeometryManager.Instance.GetMainObject();
            if (null == mesh) return;

            MeshPointDataManager meshPointDataManager = new MeshPointDataManager();
            meshPointDataManager.ShowMeshVerticesList(UIParent, mesh);
        }

        #endregion
    }
}
