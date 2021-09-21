using Assets.Geometries;
using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Import.PrefabScripts
{
    /// <summary>
    /// Thin Sandbox left side panel class.
    /// </summary>
    public class LeftSidePanel : MonoBehaviour
    {
        internal static string PrefabName => "LeftSidePanel";

        #region Private Members

        private bool IsSlide;

        #endregion

        #region Public Members

        public RectTransform MenuBtnPanelRect;
        public RectTransform LeftButtonPanel;
        public Button MenuBtn;
        public Button LoadSTLBtn;
        public Button LoadLMBtn;
        public Button ImportTransformBtn;
        public Action PanelToggled;
        public Action LoadCompleted;
        public Action<string> LoadStlClicked;

        public bool IsPanelOpen => !IsSlide;

        #endregion

        #region Public Methods

        // Start is called before the first frame update
        public void Start()
        {
            MenuBtnPanelRect.DOAnchorPosX(0f, 0f);
            LeftButtonPanel.DOAnchorPosX(LeftButtonPanel.rect.width*-1, 0f);
            IsSlide = true;
            AttachListener();
        }

        private void Update()
        {
            LoadLMBtn.interactable = GeometryManager.Instance.EnableLoad;
            ImportTransformBtn.interactable = GeometryManager.Instance.EnableLoad;
        }

        /// <summary>
        /// Add "Load New STL" button to layout group.
        /// </summary>
        public void AddButton()
        {
            var parent = GameObject.FindGameObjectWithTag("ListContainer");
            if (null == parent) return;
            LoadSTLBtn.transform.SetParent(parent.transform, false);
            LoadSTLBtn.transform.SetAsLastSibling();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attach button listeners.
        /// </summary>
        private void AttachListener()
        {
            MenuBtn.onClick.AddListener(OnMenuBtnClick);
            LoadLMBtn.onClick.AddListener(OnLoadLMClick);
            LoadSTLBtn.onClick.AddListener(OnLoadStlClick);
            ImportTransformBtn.onClick.AddListener(OnImportTransformClick);
        }

        /// <summary>
        /// Menu button click listener.
        /// </summary>
        private void OnMenuBtnClick()
        {
            if (IsSlide)
            {
                ShowLeftSlidingPanel();
                IsSlide = false;
            }
            else
            {
                HideLeftSlidingPanel();
                IsSlide = true;
            }
        }

        /// <summary>
        /// Load STL button click listener.
        /// </summary>
        private void OnLoadStlClick()
        {
            string path = EditorUtility.OpenFilePanel("Select the " +
                   "STL file.", "", "stl, STL");

            if (string.IsNullOrEmpty(path)) return;
            LoadStlClicked?.Invoke(path);
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
            LoadCompleted?.Invoke();
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
            LoadCompleted?.Invoke();
        }

        /// <summary>
        /// Shows left sliding button panel.
        /// </summary>
        /// <param name="delay"></param>
        private void ShowLeftSlidingPanel(float delay = 0f)
        {
            var tweener = LeftButtonPanel.DOAnchorPosX(LeftButtonPanel.rect.width*0.005f, 0.5f)
                                         .SetDelay(delay);
            tweener.OnPlay(AnimationCompleted);
        }

        /// <summary>
        /// Hides left sliding button panel.
        /// </summary>
        /// <param name="delay"></param>
        private void HideLeftSlidingPanel(float delay = 0f)
        {
            var tweener = LeftButtonPanel.DOAnchorPosX(LeftButtonPanel.rect.width*-1, 0.5f)
                                         .SetDelay(delay);
            tweener.OnComplete(AnimationCompleted);
        }

        private void AnimationCompleted()
        {
            PanelToggled?.Invoke();
        }

        #endregion
    }
}
