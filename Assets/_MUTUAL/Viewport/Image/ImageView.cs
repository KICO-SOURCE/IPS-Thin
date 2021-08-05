using Assets._MUTUAL.Viewport;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets._MUTUAL.Viewport
{
    public class ImageView : IView
    {
        #region Private Constants

        const string prefabPath = "Prefabs/ImageView";
        private const string parentTag = "ViewportArea";
        private const string sampleImagePath = "Images/ImageDummy";

        #endregion

        #region Private Members

        private GameObject parent;
        private GameObject m_ImageView;
        private GameObject viewPrefab;

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

        #region Constructor

        /// <summary>
        /// Create new ImageView instance.
        /// </summary>
        public ImageView()
        {
            viewPrefab = Resources.Load<GameObject>(prefabPath);
        }

        #endregion

        /// <summary>
        /// Create the view.
        /// </summary>
        public void CreateView()
        {
            parent = GameObject.FindGameObjectWithTag(parentTag);
            m_ImageView = UnityEngine.Object.Instantiate(viewPrefab, parent.transform);

            // Positioning the image view
            // [ left - bottom ]
            m_ImageView.gameObject.GetComponent<RectTransform>().offsetMin = new Vector2(0f, 0);
            // [ right - top ]
            m_ImageView.gameObject.GetComponent<RectTransform>().offsetMax = new Vector2(-parent.GetComponent<RectTransform>().rect.width/2, 
                                                                            -parent.GetComponent<RectTransform>().rect.height / 2);
            // Loading sample image
            var sampleImage = Resources.Load<Sprite>(sampleImagePath);
            Texture2D texture2D = sampleImage.texture;

            var recttexture = texture2D.texelSize;
            var recttextureheight = texture2D.height;
            var recttexturewidth = texture2D.width;

            RectTransform rectTr = (RectTransform)m_ImageView.transform;
            float width = rectTr.rect.width;
            float height = rectTr.rect.height;

            // Rendering loaded image in image view area
            var render = m_ImageView.GetComponent<RawImage>();

            if (render == null) //Todo repeated call
            {
                render = m_ImageView.AddComponent<RawImage>();
            }

            render.texture = texture2D;
            m_ImageView.SetActive(false);
        }

        /// <summary>
        /// Activates the view.
        /// </summary>
        public void Activate()
        {
            m_ImageView.SetActive(true);
        }

        /// <summary>
        /// Deactivates the view.
        /// </summary>
        public void Deactivate()
        {
            m_ImageView.SetActive(false);
        }

        #endregion
    }
}
