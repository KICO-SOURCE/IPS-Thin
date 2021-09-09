#region Usings

using TMPro;
using UnityEngine;

#endregion

namespace Assets._MUTUAL.Viewport
{
    /// <summary>
    /// A dummy view class for testing.
    /// </summary>
    public class TestView : IView
    {
        #region Private Constants

        private const string parentTag = "ViewportContainer(Clone)/ViewportArea";
        private const string prefabPath = "Prefabs/TestView";
        private static int id = 1;

        #endregion

        #region Private Members

        private GameObject viewPrefab;
        private GameObject testView;
        private GameObject parent;

        #endregion

        #region Public Properties
        
        /// <summary>
        /// View position
        /// </summary>
        public Vector2 Postion { get; set; }

        /// <summary>
        /// Viewport size
        /// </summary>
        public Vector2 Size { get; set; }

        /// <summary>
        /// Parent element
        /// </summary>
        public GameObject Parent
        {
            get
            {
                return parent;
            }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Create new test view instance.
        /// </summary>
        public TestView()
        {
            viewPrefab = Resources.Load<GameObject>(prefabPath);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Create the view.
        /// </summary>
        public void CreateView()
        {
            parent = ViewportContainer.Instance.Parent.transform.Find(parentTag).gameObject;
            testView = UnityEngine.Object.Instantiate(viewPrefab, parent.transform);
            testView.GetComponentInChildren<TMP_Text>().text = "Test Viewport_" + (id++);
            testView.SetActive(false);
        }

        /// <summary>
        /// Activates the view.
        /// </summary>
        public void Activate()
        {
           
            testView.SetActive(true);
        }

        /// <summary>
        /// Deactivates the view.
        /// </summary>
        public void Deactivate()
        {
            testView.SetActive(false);
        }

        #endregion
    }
}