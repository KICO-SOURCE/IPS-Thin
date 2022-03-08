#region Usings

using Assets.Geometries;
using Assets.Sandbox.MouseActions;
using DG.Tweening;
using UnityEngine;

#endregion

namespace Assets.Sandbox
{
    /// <summary>
    /// ThreeD Scene for the sandbox 3d models.
    /// </summary>
    public class ThreeDScene : MonoBehaviour
    {
        #region Private Fields

        private const string layer = "ThreeDLayer";
        //Dan has made these private, and they are now being set in the Awake function below
        private GameObject parent;
        private Camera displayCam;

        #endregion

        #region Public Fields


        #endregion

        #region Public Methods

        /// <summary>
        /// Display a mesh in UI
        /// </summary>
        public void DisplayMesh()
        {
            this.gameObject.SetActive(true);
            GeometryManager.Instance.DisplayAllObjects(parent.transform,
                                                LayerMask.NameToLayer(layer));
            SetCameraAndLightPosition(GeometryManager.Instance.GetMainObject());
        }

        /// <summary>
        /// Adjust viewport size of camera based on panel open/close status
        /// </summary>
        /// <param name="leftPanelOpen"></param>
        /// <param name="rightPanelOpen"></param>
        public void AdjustViewportSize(bool leftPanelOpen, bool rightPanelOpen)
        {
            float x = 0, y = 0, width = 0.95f, height = 0.9f;

            if (leftPanelOpen)
            {
                x = 0.22f;
                width -= x;
            }

            if (rightPanelOpen)
            {
                width -= 0.2f;
            }

            displayCam.DORect(new Rect(x, y, width, height), 0.5f);
        }

		#endregion

		#region Private Methods

		private void Awake() {
            parent = this.transform.Find("ThreeDObjects").gameObject;
            displayCam = this.transform.Find("ThreeDCamera").GetComponent<Camera>();
    }

		private void Start()
        {
            DisplayMesh();


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

            var renderers = parent.GetComponentsInChildren<MeshRenderer>();
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

            displayCam.gameObject.SetActive(false);

            displayCam.transform.position = camPosition;
            displayCam.transform.LookAt(center, direction);
            displayCam.orthographicSize = distance;

            var camAxis = displayCam.transform.TransformVector(Vector3.up);
            camAxis.Normalize();

            var meshAxis = mesh.transform.TransformVector(Vector3.forward);
            meshAxis.Normalize();

            var axis = displayCam.transform.TransformVector(Vector3.forward);
            axis.Normalize();

            var angle = Vector3.SignedAngle(camAxis, meshAxis, axis);
            Debug.Log($"Angle : {angle}");

            displayCam.transform.RotateAround(center, axis, angle);

            displayCam.gameObject.GetComponent<OrbitalMouseController>().target = center;
            displayCam.gameObject.GetComponent<OrbitalMousePanHelper>().pivotTarget = center;

            displayCam.gameObject.SetActive(true);
        }

        #endregion
    }
}
