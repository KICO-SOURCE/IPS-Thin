using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Assets.Import.PrefabScripts
{
    /// <summary>
    /// Thin Sandbox right side panel class.
    /// </summary>
    public class RightSidePanel : MonoBehaviour
    {
        internal static string PrefabName => "RightSidePanel";

        #region Private Members

        private RectTransform ButtonPanelRect;
        private RectTransform SlidingPanelRect;
        private Button SlideLeftBTN;
        private bool IsSlide;

        #endregion

        #region Public Members

        public Button TransparentBtn;
        public Button CloseBtn;

        #endregion

        #region Public Methods

        public void Awake()
        {
            ButtonPanelRect = transform.Find("SlideLeftBtnPanel").GetComponent<RectTransform>();
            SlidingPanelRect = transform.Find("SlidingPanel").GetComponent<RectTransform>();
            SlideLeftBTN = transform.Find("SlideLeftBtnPanel/SlideLeftBtn").GetComponent<Button>();
            TransparentBtn = transform.Find("SlidingPanel/ButtonContainer/TransparentBtn").GetComponentInChildren<Button>();
            CloseBtn = transform.Find("SlidingPanel/ButtonContainer/CloseBtn").GetComponentInChildren<Button>();
        }

        // Start is called before the first frame update
        public void Start()
        {
            ButtonPanelRect.DOAnchorPosX(0f, 0f);
            SlidingPanelRect.DOAnchorPosX(SlidingPanelRect.rect.width, 0f);
            IsSlide = true;
            SlideLeftBTN.onClick.AddListener(OnSlideLeftClick);
        }

        #endregion

        #region Private Methods

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
            SlidingPanelRect.DOAnchorPosX(SlidingPanelRect.rect.width * 0.008f, 0.5f).SetDelay(delay);
        }

        /// <summary>
        /// Hides sliding panel.
        /// </summary>
        /// <param name="delay"></param>
        private void HideSlidingPanel(float delay = 0f)
        {
            SlidingPanelRect.DOAnchorPosX(SlidingPanelRect.rect.width, 0.5f).SetDelay(delay);
        }

        #endregion
    }
}
