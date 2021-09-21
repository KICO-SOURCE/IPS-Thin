#region Usings

using Assets.CaseFile;
using Assets.Geometries;
using Assets.Sandbox.MouseActions;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        private Color normalColor;

        #endregion

        #region Public Fields

        public GameObject Parent;
        public Camera Camera;
        public GameObject ShowVerticesButton;
        public GameObject CloseButton;
        public GameObject TransparentButton;
        public GameObject UIParent;

        #endregion

        #region Public Methods

        /// <summary>
        /// Display a mesh in UI
        /// </summary>
        public void DisplayMesh()
        {
            this.gameObject.SetActive(true);
            GeometryManager.Instance.DisplaySelectedObjects(Parent.transform,
                                                LayerMask.NameToLayer(layer));
            SetCameraAndLightPosition(GeometryManager.Instance.GetMainObject());
        }

        public void AdjustViewportSize(bool leftPanelOpen, bool rightPanelOpen)
        {
            float x = 0, y = 0, width = 0.95f, height = 0.9f;

            if(leftPanelOpen)
            {
                x = 0.22f;
                width -= x;
            }

            if (rightPanelOpen)
            {
                width -= 0.2f;
            }

            Camera.DORect(new Rect(x, y, width, height), 0.5f);
        }

        #endregion

        #region Private Methods

        private void Start()
        {
            //DisplayMesh();
            //ShowVerticesButton.SetActive(false);
            //var showVerticesBtn = ShowVerticesButton.GetComponentInChildren<Button>();
            //showVerticesBtn.onClick.AddListener(ShowVerticesList);
            //var closeBtn = CloseButton.GetComponentInChildren<Button>();
            //closeBtn.onClick.AddListener(OnCloseClick);
            //var transparentBtn = TransparentButton.GetComponentInChildren<Button>();
            //transparentBtn.onClick.AddListener(OnTransparentClick);
            //normalColor = transparentBtn.GetComponent<Image>().color;
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

            Camera.gameObject.SetActive(false);

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

            Camera.gameObject.GetComponent<OrbitalMouseController>().target = center;
            Camera.gameObject.GetComponent<OrbitalMousePanHelper>().pivotTarget = center;

            Camera.gameObject.SetActive(true);
        }

        private void ShowVerticesList()
        {
            var mesh = GeometryManager.Instance.GetMainObject();
            if (null == mesh) return;

            MeshPointDataManager meshPointDataManager = new MeshPointDataManager();
            meshPointDataManager.ShowMeshVerticesList(UIParent, mesh);
        }

        private void OnCloseClick()
        {
            GeometryManager.Instance.DistroyAllObjects();
            SceneManager.LoadScene("MainScene");
        }

        private void OnTransparentClick()
        {
            GeometryManager.Instance.ToggleTransparency();
            var color = GeometryManager.Instance.Transparent ?
                                    Color.gray : normalColor;

            TransparentButton.GetComponentInChildren<Image>().color = color;
        }

        #endregion
    }
}
