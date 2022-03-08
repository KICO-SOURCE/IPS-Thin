using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
using Assets.Geometries;

namespace Assets.Import.PrefabScripts
{
    /// <summary>
    /// Thin Sandbox right side panel class.
    /// </summary>
    public class RightSidePanel : MonoBehaviour
    {
        internal static string PrefabName => "RightSidePanel";

        #region Private Members

        private bool IsSlide;

        #endregion

        #region Public Members

        public RectTransform ButtonPanelRect;
        public RectTransform SlidingPanelRect;
        public Button SlideLeftBTN;
        public Button TransparentBtn;
        public Action PanelToggled;

        public bool IsPanelOpen => !IsSlide;

        #endregion

        #region Public Methods

        // Start is called before the first frame update
        public void Start()
        {
            ButtonPanelRect.DOAnchorPosX(0f, 0f);
            SlidingPanelRect.DOAnchorPosX(SlidingPanelRect.rect.width, 0f);
            IsSlide = true;
            AttachListeners();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attach listeners.
        /// </summary>
        private void AttachListeners()
        {
            SlideLeftBTN.onClick.AddListener(OnSlideLeftClick);
            TransparentBtn.onClick.AddListener(OnTransparentClick);
        }

        /// <summary>
        /// Transparent button click listener.
        /// </summary>
        private void OnTransparentClick()
        {
            GeometryManager.Instance.ToggleTransparency();
            var color = GeometryManager.Instance.Transparent ?
                                    Color.gray : Color.white;

            TransparentBtn.GetComponentInChildren<Image>().color = color;
        }

        /// <summary>
        /// Slide left button click listener.
        /// </summary>
        private void OnSlideLeftClick()
        {
            if (IsSlide)
            {
                HideBtnPanel();
                ShowSlidingPanel();
                SlideLeftBTN.GetComponentInChildren<TMP_Text>().text = ">";
                IsSlide = false;
            }
            else
            {
                HideSlidingPanel();
                ShowBtnPanel();
                SlideLeftBTN.GetComponentInChildren<TMP_Text>().text = "<";
                IsSlide = true;
            }
        }

        /// <summary>
        /// Shows button panel.
        /// </summary>
        /// <param name="delay"></param>
        private void ShowBtnPanel(float delay = 0f)
        {
            ButtonPanelRect.DOAnchorPosX(0f, 0.5f).SetDelay(delay);
        }

        /// <summary>
        /// Hides button panel.
        /// </summary>
        /// <param name="delay"></param>
        private void HideBtnPanel(float delay = 0f)
        {
            ButtonPanelRect.DOAnchorPosX(ButtonPanelRect.rect.width * -1f, 0.5f).SetDelay(delay);
        }

        /// <summary>
        /// Shows sliding panel.
        /// </summary>
        /// <param name="delay"></param>
        private void ShowSlidingPanel(float delay = 0f)
        {
            var tweener = SlidingPanelRect.DOAnchorPosX(SlidingPanelRect.rect.width * 0.008f, 0.5f)
                                          .SetDelay(delay);
            tweener.OnPlay(AnimationCompleted);
        }

        /// <summary>
        /// Hides sliding panel.
        /// </summary>
        /// <param name="delay"></param>
        private void HideSlidingPanel(float delay = 0f)
        {
            var tweener = SlidingPanelRect.DOAnchorPosX(SlidingPanelRect.rect.width, 0.5f)
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
