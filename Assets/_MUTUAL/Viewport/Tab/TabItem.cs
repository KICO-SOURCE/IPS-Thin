#region Usings

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Assets._MUTUAL.Viewport.Tab
{
    /// <summary>
    /// Tab item class.
    /// </summary>
    public class TabItem : MonoBehaviour, ITabItem
    {
        #region Private Constants

        private const string titleControl = "Title";
        private const string parentTag = "TabGroup";
        private const string tabItemPrefabPath = "Prefabs/TabItem";

        #endregion

        #region Private Members

        private TMP_Text title;
        private GameObject tabItemPrefab;
        private GameObject parent;
        private Sprite buttonSprite;

        #endregion

        #region Public Properties

        /// <summary>
        ///  Gets or sets the tilte of tab item.
        /// </summary>
        public string Title
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the parent object.
        /// </summary>
        public GameObject Parent
        {
            get
            {
                return parent;
            }
        }

        #endregion

        #region Public Events

        public event EventHandler<TabSelectedEventArgs> TabSelected;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates new instance of tab item.
        /// </summary>
        public TabItem(TabGroup tabGroup)
        {
            tabItemPrefab = Resources.Load<GameObject>(tabItemPrefabPath);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Avtivates the tab item.
        /// </summary>
        public void Activate()
        {
            parent = transform.parent.gameObject;
            title = transform.Find(titleControl).GetComponent<TMP_Text>();
            if (title != null)
            {
                title.text = Title;
            }
            buttonSprite = transform.GetComponent<UnityEngine.UI.Image>().sprite;
            AttachListeners();
        }

        /// <summary>
        /// Deactivates the tab item.
        /// </summary>
        public void Deactivate()
        {
            DetachListeners();
        }

        /// <summary>
        /// Sets the tab selected.
        /// </summary>
        public void OnSelected()
        {
            GetComponent<UnityEngine.UI.Image>().sprite = null;
        }

        /// <summary>
        /// Sets the tab deselected.
        /// </summary>
        public void OnDeSelected()
        {
            GetComponent<UnityEngine.UI.Image>().sprite = buttonSprite;
        }

        #endregion

        #region Private Methods

        private void Awake()
        {
            title = transform.Find(titleControl).GetComponent<TMP_Text>();
            title.text = Title;
        }

        /// <summary>
        /// Attach listeners.
        /// </summary>
        private void AttachListeners()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }

        /// <summary>
        /// Detach listeners.
        /// </summary>
        private void DetachListeners()
        {
            GetComponent<Button>().onClick.RemoveAllListeners();
        }

        #endregion

        #region Private Event Listeners

        /// <summary>
        /// Event listener for selected tab.
        /// </summary>
        private void OnClick()
        {
            var args = new TabSelectedEventArgs() { TabItem = this };
            TabSelected?.Invoke(this, args);
        }

        #endregion
    }
}