#region Usings

using System.Collections.Generic;
using TMPro;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// A view class for 3D objects.
    /// </summary>
    public class _3DView : IView
    {
        #region Private Constants

        private const string parentPrefabPath = "Prefabs/3DViewport";
        private const string meshMaterialPath = "Materials/BoneMaterial";

        #endregion

        #region Private Members

        private GameObject parent;
        private List<GameObject> meshObjects;
        private Camera centerCamera;
        private Light centerLight;
        private Dictionary<string, Mesh> meshes;
        private ViewType viewType;
        private Vector3 origin, siAxis, mlAxis, apAxis;
        private int cullingMask = -1;
        private int layer = 8;

        #endregion

        #region Public Properties

        /// <summary>
        /// View position
        /// </summary>
        public Vector2 Postion { get; set; } = new Vector2(0, 0);

        /// <summary>
        /// Viewport size
        /// </summary>
        public Vector2 Size { get; set; } = new Vector2(1, 1);

        /// <summary>
        /// Parent element
        /// </summary>
        public GameObject Parent
        {
            get
            {
                return parent;
            }
        }

        /// <summary>
        /// Camera position
        /// </summary>
        public int CameraPostion { get; set; } = 250;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new test view instance.
        /// </summary>
        public _3DView(int layer = 8)
        {
            meshObjects = new List<GameObject>();
            this.layer = layer;
            cullingMask = (1 << layer);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise the view
        /// </summary>
        /// <param name="meshes"></param>
        /// <param name="viewType"></param>
        /// <param name="origin"></param>
        /// <param name="siAxis"></param>
        /// <param name="mlAxis"></param>
        /// <param name="apAxis"></param>
        public void InitialiseView(Dictionary<string, Mesh> meshes, ViewType viewType,
                                   Vector3 origin, Vector3 siAxis,
                                   Vector3 mlAxis, Vector3 apAxis)
        {
            this.viewType = viewType;
            this.meshes = meshes;
            this.origin = origin;
            this.siAxis = siAxis;
            this.mlAxis = mlAxis;
            this.apAxis = apAxis;
        }

        /// <summary>
        /// Create the view.
        /// </summary>
        public void CreateView()
        {
            parent = UnityEngine.Object.Instantiate(Resources.Load<GameObject>(parentPrefabPath));

            var cameras = parent.GetComponentsInChildren<Camera>();
            centerCamera = cameras[0];
            centerCamera.rect = new Rect(Postion, Size);
            centerCamera.cullingMask = cullingMask;
            centerCamera.enabled = false;

            var lights = parent.GetComponentsInChildren<Light>();
            centerLight = lights[0];
            centerLight.cullingMask = cullingMask;
            centerLight.enabled = false;

            switch (viewType)
            {
                case ViewType.CoronalView:
                    CreateCoronalView();
                    break;
                case ViewType.AxialView:
                    CreateAxialView();
                    break;
                case ViewType.SagittalView:
                    CreateSagittalView();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Activates the view.
        /// </summary>
        public void Activate()
        {
            var material = Resources.Load<Material>(meshMaterialPath);
            foreach (var mesh in meshes)
            {
                var go = new GameObject(mesh.Key, typeof(MeshFilter), typeof(MeshRenderer));
                go.transform.parent = parent.transform;
                go.GetComponent<MeshFilter>().mesh = mesh.Value;
                go.GetComponent<MeshRenderer>().material = material;
                go.layer = layer;

                //Need to revisit why this is required
                go.transform.localScale = new Vector3(1, -1, 1);
                go.SetActive(true);
                meshObjects.Add(go);
            }
            parent.SetActive(true);
        }

        /// <summary>
        /// Deactivates the view.
        /// </summary>
        public void Deactivate()
        {
            foreach (var go in meshObjects)
            {
                go.SetActive(false);
                UnityEngine.Object.DestroyImmediate(go);
            }
            meshObjects.Clear();

            if (parent != null)
            {
                parent.SetActive(false);
                //UnityEngine.Object.DestroyImmediate(m_Parent);
                //m_Parent = null;
            }
        }

        #endregion

        #region Private Methods

        private void CreateCoronalView()
        {
            centerLight.enabled = true;
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - apAxis * CameraPostion;
            centerCamera.transform.LookAt(origin, -apAxis);
            centerCamera.gameObject.SetActive(true);
        }

        private void CreateAxialView()
        {
            centerLight.enabled = true;
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - siAxis * CameraPostion;
            centerCamera.transform.LookAt(origin, -siAxis);
            centerCamera.gameObject.SetActive(true);
        }

        private void CreateSagittalView()
        {
            centerLight.enabled = true;
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - mlAxis * CameraPostion;
            centerCamera.transform.LookAt(origin, -mlAxis);

            //Need to remove after changing axis definition
            var camAxis = centerCamera.transform.TransformVector(Vector3.left);
            centerCamera.transform.Rotate(camAxis, 90);

            centerCamera.gameObject.SetActive(true);
        }

        #endregion
    }
}