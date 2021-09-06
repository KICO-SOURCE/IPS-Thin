#region Usings

using Assets._MUTUAL.Viewport.Tab;
using System;
using System.Linq;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// Viewport container class.
    /// </summary>
    public class ViewportContainer : IUIElement
    {

        #region Private Constants

        private const string parentTag = "UIParent";
        private const string prefabPath = "Prefabs/ViewportContainer";

        #endregion

        #region Private Members

        private static readonly Lazy<ViewportContainer> _instance = new Lazy<ViewportContainer>(() => new ViewportContainer());
        private GameObject containerPrefab;
        private GameObject mainContainer;
        private GameObject parent;
        private TabGroup tabGroup;
        private ViewportManager viewportManager;

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ViewportContainer Instance
        {
            get { return _instance.Value; }
        }

        /// <summary>
        /// Gets the parent.
        /// </summary>
        public GameObject Parent
        {
            get
            {
                return parent;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new instance of viewport container.
        /// </summary>
        /// <param name="tabGroup"></param>
        /// <param name="viewportManager"></param>
        private ViewportContainer()
        {
            containerPrefab = Resources.Load<GameObject>(prefabPath);
            parent = GameObject.FindGameObjectWithTag(parentTag);
            tabGroup = TabGroup.Instance;
            viewportManager = ViewportManager.Instance;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport container.
        /// </summary>
        public void Activate()
        {
            mainContainer = UnityEngine.Object.Instantiate(containerPrefab, parent.transform);
            viewportManager.PopulateViewports();
            if (viewportManager.Viewports.Count > 1)
            {
                tabGroup.Activate();
                tabGroup.TabSelected += HandleTabSelected;
                PopulateViewportTabs();
                viewportManager.CreateViewports();
                tabGroup.Tabs.First().OnSelected();
                viewportManager.HandleViewportSelected(tabGroup.Tabs.First().Title);
            }
            else
            {
                viewportManager.CreateViewports();
                viewportManager.Viewports.FirstOrDefault()?.Activate();
            }
        }

        /// <summary>
        /// Deactivates the viewport container.
        /// </summary>
        public void Deactivate()
        {
            if (mainContainer != null)
                UnityEngine.Object.Destroy(mainContainer);
            tabGroup.Deactivate();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Populate viewport tabs.
        /// </summary>
        private void PopulateViewportTabs()
        {
            var tabgroup = GameObject.FindGameObjectWithTag("TabGroup").transform;
            var tabItemPrefab = Resources.Load<GameObject>("Prefabs/TabItem");
            foreach (var vp in viewportManager.Viewports)
            {
                var tabItem = UnityEngine.Object.Instantiate(tabItemPrefab, tabgroup);
                var tab = tabItem.GetComponent<TabItem>();
                tab.Title = vp.Title;
                tab.Activate();
                tabGroup.Add(tab);
            }
        }

        #endregion

        #region Private Event Listener

        /// <summary>
        /// Event listener for handle selected tab operation.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleTabSelected(object sender, TabSelectedEventArgs e)
        {
            viewportManager.HandleViewportSelected(e.TabItem.Title);
        }

        #endregion
    }
}
