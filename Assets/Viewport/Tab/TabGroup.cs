#region Usings

using System;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace Assets.Viewport.Tab
{
    /// <summary>
    /// Tab group class.
    /// </summary>
    public class TabGroup : IUIElement
    {
        #region Dependencies

        private ITabItem selectedTab;

        #endregion

        #region Private Constants

        private const string parentTag = "ViewportContainer(Clone)/TabArea";
        private const string prefabPath = "Prefabs/TabGroup";

        #endregion

        #region Private Memebers

        private static readonly Lazy<TabGroup> _instance = new Lazy<TabGroup>(() => new TabGroup());

        private GameObject tabGroupPrefab;
        private GameObject tabGroup;
        private GameObject parent;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static TabGroup Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Gets the list of tabs.
        /// </summary>
        public List<ITabItem> Tabs { get; }

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
        /// Creates new instance of tab group.
        /// </summary>
        private TabGroup()
        {
            Tabs = new List<ITabItem>();
            tabGroupPrefab = Resources.Load<GameObject>(prefabPath);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add tab item to list.
        /// </summary>
        /// <param name="tabItem"></param>
        public void Add(ITabItem tabItem)
        {
            Tabs.Add(tabItem);
            tabItem.TabSelected += HandleTabSelected;
        }

        /// <summary>
        /// Set the selected tab and tab style.
        /// </summary>
        /// <param name="tab"></param>
        public void OnTabSelected(ITabItem tab)
        {
            selectedTab = tab;
            SetTabStyle();
        }

        /// <summary>
        /// Activates the UI.
        /// </summary>
        public void Activate()
        {
            parent = ViewportContainer.Instance.Parent.transform.Find(parentTag).gameObject;
            tabGroup = UnityEngine.Object.Instantiate(tabGroupPrefab, parent.transform);
        }

        /// <summary>
        /// Deactivates the UI.
        /// </summary>
        public void Deactivate()
        {
            foreach(var tab in Tabs)
            {
                tab.TabSelected -= HandleTabSelected;
                tab.Deactivate();
            }

            if (tabGroup != null)
            {
                UnityEngine.Object.Destroy(tabGroup);
            }

        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Set the tab button style.
        /// </summary>
        private void SetTabStyle()
        {
            foreach (var tab in Tabs)
            {
                if (tab == selectedTab)
                {
                    tab.OnSelected();
                }
                else
                {
                    tab.OnDeSelected();
                }
            }
        }

        #endregion

        #region Private Event Listeners

        /// <summary>
        /// Event listener for handle selected tab operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleTabSelected(object sender, TabSelectedEventArgs e)
        {
            OnTabSelected(e.TabItem);
            TabSelected?.Invoke(this, e);
        }

        #endregion
    }
}
