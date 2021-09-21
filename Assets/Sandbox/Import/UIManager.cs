using Assets.Geometries;
using Assets.Import.PrefabScripts;
using Assets.Sandbox;
using UnityEngine;

namespace Assets.Import
{
    /// <summary>
    /// UI manager class.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Public Members

        public LeftSidePanel LeftSidePanel;
        public ImportDataPanel ImportDataPanel;
        public RightSidePanel RightSidePanel;
        public ThreeDScene ThreeDScene;
        public GameObject ViewPort;

        #endregion

        #region Public Methods

        public void Start()
        {
            GeometryManager.Instance.DisplayList();
            ImportDataPanel.DataPanelClosed += AddButton;

            HandlePanelToggled();
            LeftSidePanel.PanelToggled += HandlePanelToggled;
            RightSidePanel.PanelToggled += HandlePanelToggled;
            LeftSidePanel.LoadCompleted += HandleLoadCompleted;
            LeftSidePanel.LoadStlClicked += HandleLoadStlClicked;
        }

        #endregion

        #region Private Methods

        private void HandlePanelToggled()
        {
            ThreeDScene.AdjustViewportSize(LeftSidePanel.IsPanelOpen,
                                           RightSidePanel.IsPanelOpen);
        }

        private void HandleLoadCompleted()
        {
            ThreeDScene.DisplayMesh();
        }

        private void HandleLoadStlClicked(string path)
        {
            ThreeDScene.gameObject.SetActive(false);
            ImportDataPanel.gameObject.SetActive(true);
            ImportDataPanel.SetFileTitle(path);
        }

        /// <summary>
        /// Add "Load New STL" button to layout group.
        /// </summary>
        private void AddButton()
        {
            LeftSidePanel.AddButton();
            ThreeDScene.DisplayMesh();
        }

        #endregion
    }
}
