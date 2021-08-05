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
        private Camera leftCamera;
        private Camera rightCamera;
        private Camera centerCamera;
        private Light leftLight;
        private Light rightLight;
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
            leftCamera = cameras[0];
            rightCamera = cameras[1];
            centerCamera = cameras[2];

            centerCamera.rect = new Rect(Postion, Size);
            leftCamera.rect = new Rect(Postion.x, Postion.y, Size.x/2, Size.y);
            rightCamera.rect = new Rect(Postion.x + Size.x / 2, Postion.y, Size.x/2, Size.y);

            centerCamera.cullingMask = cullingMask;
            leftCamera.cullingMask = cullingMask;
            rightCamera.cullingMask = cullingMask;

            leftCamera.enabled = false;
            rightCamera.enabled = false;
            centerCamera.enabled = false;

            var lights = parent.GetComponentsInChildren<Light>();
            leftLight = lights[0];
            rightLight = lights[1];
            centerLight = lights[2];

            leftLight.cullingMask = cullingMask;
            rightLight.cullingMask = cullingMask;
            centerLight.cullingMask = cullingMask;

            leftLight.enabled = false;
            rightLight.enabled = false;
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
                case ViewType.LongLegCoronalView:
                    CreateLongLegCoronalView();
                    break;
                case ViewType.LongLegView:
                    CreateLongLegView();
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
            centerCamera.transform.position = origin - apAxis * 250;
            centerCamera.transform.LookAt(origin, -apAxis);
            centerCamera.gameObject.SetActive(true);
        }

        private void CreateAxialView()
        {
            centerLight.enabled = true;
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - siAxis * 250;
            centerCamera.transform.LookAt(origin, -siAxis);
            centerCamera.gameObject.SetActive(true);
        }

        private void CreateSagittalView()
        {
            centerLight.enabled = true;
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - mlAxis * 250;
            centerCamera.transform.LookAt(origin, -mlAxis);

            //Need to remove after changing axis definition
            var camAxis = centerCamera.transform.TransformVector(Vector3.left);
            centerCamera.transform.Rotate(camAxis, 90);

            centerCamera.gameObject.SetActive(true);
        }

        private void CreateLongLegCoronalView()
        {
            centerLight.enabled = true;
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - apAxis * 2000;
            centerCamera.transform.LookAt(origin, -apAxis);
            leftCamera.gameObject.SetActive(true);
        }

        private void CreateLongLegView()
        {
            leftLight.enabled = true;
            leftCamera.enabled = true;
            leftCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            leftCamera.transform.position = origin - apAxis * 2000;
            leftCamera.transform.LookAt(origin, -apAxis);

            rightLight.enabled = true;
            rightCamera.enabled = true;
            rightCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            rightCamera.transform.position = origin - mlAxis * 2000;
            rightCamera.transform.LookAt(origin, -mlAxis);

            //Need to remove after changing axis definition
            var camAxis = rightCamera.transform.TransformVector(Vector3.left);
            rightCamera.transform.Rotate(camAxis, 90);

            leftCamera.gameObject.SetActive(true);
            rightCamera.gameObject.SetActive(true);
        }

        #endregion
    }
}