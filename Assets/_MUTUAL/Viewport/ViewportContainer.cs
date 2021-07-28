#region Usings

using Assets._MUTUAL.Viewport.Tab;
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

        private GameObject containerPrefab;
        private GameObject mainContainer;
        private GameObject parent;
        private TabGroup tabGroup;
        private ViewportManager viewportManager;

        #endregion

        #region Public Properties

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
        public ViewportContainer(TabGroup tabGroup, ViewportManager viewportManager)
        {
            containerPrefab = Resources.Load<GameObject>(prefabPath);
            parent = GameObject.FindGameObjectWithTag(parentTag);
            this.tabGroup = tabGroup;
            this.viewportManager = viewportManager;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Activates the viewport container.
        /// </summary>
        public void Activate()
        {
            mainContainer = UnityEngine.Object.Instantiate(containerPrefab, parent.transform);
            tabGroup.Activate();
            tabGroup.TabSelected += HandleTabSelected;
            PopulateViewportTabs();
            tabGroup.Tabs.First().OnSelected();
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
