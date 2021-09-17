using DG.Tweening;
using System;
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

        private RectTransform MenuBtnPanelRect;
        private RectTransform LeftButtonPanel;
        private Button MenuBtn;
        private bool IsSlide;

        #endregion

        #region Public Members

        public Button LoadSTLBtn;
        public Button LoadLMBtn;
        public Button ImportTransformBtn;
        public Action PanelToggled;

        public bool IsPanelOpen => !IsSlide;

        #endregion

        #region Public Methods

        public void Awake()
        {
            MenuBtnPanelRect = transform.Find("MenuBtnPanel").GetComponent<RectTransform>();
            MenuBtn = transform.Find("MenuBtnPanel").GetComponentInChildren<Button>();
            LeftButtonPanel = transform.Find("LeftButtonPanel").GetComponent<RectTransform>();
            LoadSTLBtn = transform.Find("LeftButtonPanel/LoadStlContainer").GetComponentInChildren<Button>();
            LoadLMBtn = transform.Find("LeftButtonPanel/LoadLM").GetComponentInChildren<Button>();
            ImportTransformBtn = transform.Find("LeftButtonPanel/ImportTransform").GetComponentInChildren<Button>();
        }

        // Start is called before the first frame update
        public void Start()
        {
            MenuBtnPanelRect.DOAnchorPosX(0f, 0f);
            LeftButtonPanel.DOAnchorPosX(LeftButtonPanel.rect.width*-1, 0f);
            IsSlide = true;
            AttachListener();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attach button listeners.
        /// </summary>
        private void AttachListener()
        {
            MenuBtn.onClick.AddListener(OnMenuBtnClick);
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
