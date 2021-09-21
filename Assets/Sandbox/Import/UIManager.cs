using Assets.Geometries;
using Assets.Import.PrefabScripts;
using Assets.Sandbox;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Import
{
    /// <summary>
    /// UI manager class.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Public Members

        public GameObject ViewPort;

        #endregion

        #region Private Members

        private LeftSidePanel LeftSidePanel;
        private ImportDataPanel ImportDataPanel;
        private RightSidePanel RightSidePanel;
        private Color normalColor;
        private ThreeDScene ThreeDScene;

        #endregion

        #region Public Methods

        public void Start()
        {
            LeftSidePanel= transform.Find("LeftSidePanel").GetComponent<LeftSidePanel>();
            ImportDataPanel = transform.Find("ImportDataPanel").GetComponent<ImportDataPanel>();
            ThreeDScene = GameObject.Find("ThreeDContainer").GetComponent<ThreeDScene>();
            RightSidePanel = transform.Find("RightSidePanel").GetComponent<RightSidePanel>();
            GeometryManager.Instance.DisplayList();
            AttachListeners();
            ImportDataPanel.DataPanelClosed += AddButton;
            normalColor = RightSidePanel.TransparentBtn.GetComponent<Image>().color;

            HandlePanelToggled();
            LeftSidePanel.PanelToggled += HandlePanelToggled;
            RightSidePanel.PanelToggled += HandlePanelToggled;
        }

        private void Update()
        {
            LeftSidePanel.LoadLMBtn.interactable = GeometryManager.Instance.EnableLoad;
            LeftSidePanel.ImportTransformBtn.interactable = GeometryManager.Instance.EnableLoad;
        }

        #endregion

        #region Private Methods

        private void HandlePanelToggled()
        {
            ThreeDScene.AdjustViewportSize(LeftSidePanel.IsPanelOpen,
                                           RightSidePanel.IsPanelOpen);
        }

        /// <summary>
        /// Attach listeners.
        /// </summary>
        private void AttachListeners()
        {
            LeftSidePanel.LoadLMBtn.onClick.AddListener(OnLoadLMClick);
            LeftSidePanel.LoadSTLBtn.onClick.AddListener(OnLoadStlClick);
            LeftSidePanel.ImportTransformBtn.onClick.AddListener(OnImportTransformClick);

            RightSidePanel.TransparentBtn.onClick.AddListener(OnTransparentClick);
        }

        /// <summary>
        /// Load STL button click listener.
        /// </summary>
        private void OnLoadStlClick()
        {
            string path = EditorUtility.OpenFilePanel("Select the " +
                   "STL file.", "", "stl, STL");

            if (string.IsNullOrEmpty(path)) return;

            ThreeDScene.gameObject.SetActive(false);
            ImportDataPanel.gameObject.SetActive(true);
            ImportDataPanel.SetFileTitle(path);
        }

        /// <summary>
        /// Add "Load New STL" button to layout group.
        /// </summary>
        private void AddButton()
        {
            var parent = GameObject.FindGameObjectWithTag("ListContainer");
            LeftSidePanel.LoadSTLBtn.transform.SetParent(parent.transform, false);
            LeftSidePanel.LoadSTLBtn.transform.SetAsLastSibling();

            ThreeDScene.DisplayMesh();
        }

        /// <summary>
        /// Load landmark button click listener.
        /// </summary>
        private void OnLoadLMClick()
        {
            string path = EditorUtility.OpenFilePanel("Select the " +
                "landmark file.", "", "csv, CSV");
            if (string.IsNullOrEmpty(path)) return;

            GeometryManager.Instance.LoadLandmarks(path);
            ThreeDScene.DisplayMesh();
        }

        /// <summary>
        /// Load transform button click listener.
        /// </summary>
        private void OnImportTransformClick()
        {
            string path = EditorUtility.OpenFilePanel("Select the " +
                "transform file.", "", "csv, CSV");
            if (string.IsNullOrEmpty(path)) return;

            GeometryManager.Instance.LoadTransform(path);
            ThreeDScene.DisplayMesh();
        }

        /// <summary>
        /// Transparent button click listener.
        /// </summary>
        private void OnTransparentClick()
        {
            GeometryManager.Instance.ToggleTransparency();
            var color = GeometryManager.Instance.Transparent ?
                                    Color.gray : normalColor;

            RightSidePanel.TransparentBtn.GetComponentInChildren<Image>().color = color;
        }

        #endregion
    }
}
