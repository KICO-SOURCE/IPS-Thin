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

        private const string parentTag = "MeshParent";
        private const string prefabPath = "Prefabs/3DViewport";

        #endregion

        #region Private Members

        private GameObject parent;
        private GameObject meshView;
        private Camera camera;
        private Vector3 origin, camAxis;
        private int cullingMask = -1;

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

        /// <summary>
        /// View rotation angle
        /// </summary>
        public int RotationAngle { get; set; } = 0;

        #endregion

        #region Constructor

        /// <summary>
        /// Create new test view instance.
        /// </summary>
        public _3DView()
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initialise the view
        /// </summary>
        /// <param name="cullingMask"></param>
        /// <param name="origin"></param>
        /// <param name="camAxis"></param>
        public void InitialiseView(int cullingMask,
                                   Vector3 origin, Vector3 camAxis)
        {
            this.cullingMask = cullingMask;
            this.origin = origin;
            this.camAxis = camAxis;
        }

        /// <summary>
        /// Create the view.
        /// </summary>
        public void CreateView()
        {
            parent = GameObject.FindGameObjectWithTag(parentTag);
            meshView = UnityEngine.Object.Instantiate(Resources
                        .Load<GameObject>(prefabPath), parent.transform);

            var cameras = meshView.GetComponentsInChildren<Camera>();
            camera = cameras[0];
            camera.rect = new Rect(Postion, Size);
            camera.cullingMask = cullingMask;

            camera.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            camera.transform.position = origin - camAxis * CameraPostion;
            camera.transform.LookAt(origin, -camAxis);

            var axis = camera.transform.TransformVector(Vector3.forward);
            camera.transform.RotateAround(origin, axis, RotationAngle);

            camera.gameObject.SetActive(true);
        }

        /// <summary>
        /// Activates the view.
        /// </summary>
        public void Activate()
        {
            camera.enabled = true;
            if (meshView != null)
            {
                meshView.SetActive(true);
            }
        }

        /// <summary>
        /// Deactivates the view.
        /// </summary>
        public void Deactivate()
        {
            camera.enabled = false;
            if (meshView != null)
            {
                meshView.SetActive(false);
                //UnityEngine.Object.DestroyImmediate(meshView);
                //meshView = null;
            }
        }

        #endregion
    }
}