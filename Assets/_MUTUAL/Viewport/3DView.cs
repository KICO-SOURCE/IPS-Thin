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
        private Dictionary<string, Mesh> meshes;
        private ViewType viewType;
        private Vector3 origin, siAxis, mlAxis, apAxis;

        #endregion

        #region Public Properties

        /// <summary>
        /// View position
        /// </summary>
        public Vector2 Postion { get; set; }

        /// <summary>
        /// Viewport size
        /// </summary>
        public Vector2 Size { get; set; }

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
        public _3DView()
        {
            meshObjects = new List<GameObject>();
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
                       Vector3 origin, Vector3 siAxis, Vector3 mlAxis, Vector3 apAxis)
        {
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

            leftCamera.enabled = false;
            rightCamera.enabled = false;
            centerCamera.enabled = false;

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

                //Need to revisit why this is required
                go.transform.localScale = new Vector3(1, -1, 1);
                go.SetActive(true);
                meshObjects.Add(go);
            }
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
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - apAxis * 400;
            centerCamera.transform.LookAt(origin, -apAxis);
            centerCamera.gameObject.SetActive(true);
        }

        private void CreateAxialView()
        {
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - siAxis * 400;
            centerCamera.transform.LookAt(origin, -siAxis);

            //Need to remove after changing axis definition
            var camAxis = centerCamera.transform.TransformVector(Vector3.back);
            centerCamera.transform.Rotate(camAxis, 5);

            centerCamera.gameObject.SetActive(true);
        }

        private void CreateSagittalView()
        {
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - mlAxis * 400;
            centerCamera.transform.LookAt(origin, -mlAxis);

            //Need to remove after changing axis definition
            var camAxis = centerCamera.transform.TransformVector(Vector3.left);
            centerCamera.transform.Rotate(camAxis, 80);

            centerCamera.gameObject.SetActive(true);
        }

        private void CreateLongLegCoronalView()
        {
            centerCamera.enabled = true;
            centerCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            centerCamera.transform.position = origin - apAxis * 2000;
            centerCamera.transform.LookAt(origin, -apAxis);

            //Need to remove after changing axis definition
            var camAxis = centerCamera.transform.TransformVector(Vector3.up);
            centerCamera.transform.Rotate(camAxis, 10);

            leftCamera.gameObject.SetActive(true);
        }

        private void CreateLongLegView()
        {
            leftCamera.enabled = true;
            leftCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            leftCamera.transform.position = origin - apAxis * 1000;
            leftCamera.transform.LookAt(origin, -apAxis);

            //Need to remove after changing axis definition
            var camAxis = leftCamera.transform.TransformVector(Vector3.up);
            leftCamera.transform.Rotate(camAxis, 10);

            rightCamera.enabled = true;
            rightCamera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            rightCamera.transform.position = origin - mlAxis * 1000;
            rightCamera.transform.LookAt(origin, -mlAxis);

            //Need to remove after changing axis definition
            camAxis = rightCamera.transform.TransformVector(Vector3.left);
            rightCamera.transform.Rotate(camAxis, 80);

            leftCamera.gameObject.SetActive(true);
            rightCamera.gameObject.SetActive(true);
        }

        #endregion
    }
}