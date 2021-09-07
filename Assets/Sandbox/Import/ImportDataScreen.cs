using Assets.CaseFile;
using Assets.Import.PrefabScripts;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Assets.Import
{
    /// <summary>
    /// Imposrt STL and Landmarks class.
    /// </summary>
    public class ImportDataScreen : MonoBehaviour
    {
        #region Public Memebers

        public Button MenuBTN;
        public GameObject LoadSTL;
        public GameObject LoadLM;
        public GameObject LoadTransform;

        #endregion

        #region Private Members

        private Button LoadSTLBtn;
        private Button LoadLMBtn;
        private Button LoadTransformBtn;
        private ImportDataPanel ImportDataPanel;

        #endregion

        void Start()
        {
            LoadSTLBtn = LoadSTL.GetComponentInChildren<Button>();
            LoadLMBtn = LoadLM.GetComponentInChildren<Button>();
            LoadTransformBtn = LoadTransform.GetComponentInChildren<Button>();
            ImportDataPanel = transform.Find("ImportDataPanel").GetComponent<ImportDataPanel>();
            AttachListeners();
        }

        #region Private Methods

        /// <summary>
        /// Attach listeners.
        /// </summary>
        private void AttachListeners()
        {
            MenuBTN.onClick.AddListener(OnMenuIconClick);
            LoadSTLBtn.onClick.AddListener(OnLoadStlClick);
            LoadLMBtn.onClick.AddListener(OnLoadLMClick);
            LoadTransformBtn.onClick.AddListener(OnLoadTransformClick);
        }

        /// <summary>
        /// Menu button click listener.
        /// </summary>
        private void OnMenuIconClick()
        {
            LoadSTL.gameObject.SetActive(true);
            LoadLM.gameObject.SetActive(true);
            LoadTransform.gameObject.SetActive(true);
        }

        /// <summary>
        /// Load STL button click listener.
        /// </summary>
        private void OnLoadStlClick()
        {
            LoadStl();
            LoadSTL.gameObject.SetActive(false);
            LoadLM.gameObject.SetActive(false);
            LoadTransform.gameObject.SetActive(false);
        }

        /// <summary>
        /// Load Stl.
        /// </summary>
        private void LoadStl()
        {
            string path = EditorUtility.OpenFilePanel("Open file", "", "STL");
            if (path!=null)
            {
                ImportDataPanel.gameObject.SetActive(true);
                var loadedFile = System.IO.Path.GetFileName(path);
                ImportDataPanel.meshData = MeshGeometryFunctions.ReadStl(path);
                ImportDataPanel.SetTitle(loadedFile);
            }
        }


        /// <summary>
        /// Load landmark button click listener.
        /// </summary>
        private void OnLoadLMClick()
        {
            LoadLanmarksFromCSV();
            LoadSTL.gameObject.SetActive(false);
            LoadLM.gameObject.SetActive(false);
            LoadTransform.gameObject.SetActive(false);
        }

        /// <summary>
        /// Load transform button click listener.
        /// </summary>
        private void OnLoadTransformClick()
        {
            LoadSTL.gameObject.SetActive(false);
            LoadLM.gameObject.SetActive(false);
            LoadTransform.gameObject.SetActive(false);

            SceneManager.LoadScene("TransformImportScene");
        }

        /// <summary>
        /// Load landmarks from csv.
        /// </summary>
        public void LoadLanmarksFromCSV()
        {
            string path = EditorUtility.OpenFilePanel("Open Folder", "", "CSV");
            if (path != null)
            {
                ImportDataPanel.gameObject.SetActive(true);
                var loadedFile = System.IO.Path.GetFileName(path);
                ImportDataPanel.SetTitle(loadedFile);

                var reader = new System.IO.StreamReader(path);
                try
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var value = line.Split(',');

                        if (!value.Contains("Name") && value.Length > 3)
                        {
                            string str = value[0];
                            float x;
                            float.TryParse(value[1], out x);
                            float y;
                            float.TryParse(value[2], out y);
                            float z;
                            float.TryParse(value[3], out z);

                            ImportDataPanel.SetLandmarkData(str, new Vector3(x, y, z));
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }
        }

        #endregion
    }
}
