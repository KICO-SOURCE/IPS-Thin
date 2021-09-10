using Assets.CaseFile;
using Assets.Geometries;
using Assets.Import.PrefabScripts;
using System;
using System.Collections.Generic;
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
        public GameObject ImportTransform;
        public GameObject View;

        #endregion

        #region Private Members

        private Button LoadSTLBtn;
        private Button LoadLMBtn;
        private Button ImportTransformBtn;
        private Button ViewBtn;
        private ImportDataPanel ImportDataPanel;

        #endregion

        void Start()
        {
            LoadSTLBtn = LoadSTL.GetComponentInChildren<Button>();
            LoadLMBtn = LoadLM.GetComponentInChildren<Button>();
            ImportTransformBtn = ImportTransform.GetComponentInChildren<Button>();
            ViewBtn = View.GetComponentInChildren<Button>();
            ImportDataPanel = transform.Find("ImportDataPanel").GetComponent<ImportDataPanel>();
            GeometryManager.Instance.DisplayList();
            ImportDataPanel.DataPanelClosed += ShowButtons;
            AttachListeners();
        }

        private void Update()
        {
            LoadLMBtn.interactable = GeometryManager.Instance.EnableLoad;
            ImportTransformBtn.interactable = GeometryManager.Instance.EnableLoad;
            ViewBtn.interactable = GeometryManager.Instance.EnableView;
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
            ImportTransformBtn.onClick.AddListener(OnImportTransformClick);
            ViewBtn.onClick.AddListener(OnViewClick);
        }

        /// <summary>
        /// Menu button click listener.
        /// </summary>
        private void OnMenuIconClick()
        {
            LoadSTL.gameObject.SetActive(true);
        }

        /// <summary>
        /// Load STL button click listener.
        /// </summary>
        private void OnLoadStlClick()
        {
            LoadStl();
            LoadSTL.gameObject.SetActive(false);
        }

        /// <summary>
        /// Load Stl.
        /// </summary>
        private void LoadStl()
        {
            string path = EditorUtility.OpenFilePanel("Open file", "", "STL");
            if (path!=null)
            {
                GeometryManager.Instance.HideList();
                HideButtons();
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
        }

        /// <summary>
        /// Load transform button click listener.
        /// </summary>
        private void OnImportTransformClick()
        {
            string path = EditorUtility.OpenFilePanel("Select the transform file.", "", "csv, CSV");

            if (string.IsNullOrEmpty(path)) return;

            string transformString = System.IO.File.ReadAllText(path);
            transformString = ParseTransformString(transformString);
            Debug.Log(transformString);
            if (transformString == null)
            {
                Debug.Log("Implant transform is not available for the selected component");
            }
            else
            {
                GeometryManager.Instance.UpdateTransform(transformString);
            }
        }

        /// <summary>
        /// Load landmarks from csv.
        /// </summary>
        public void LoadLanmarksFromCSV()
        {
            string path = EditorUtility.OpenFilePanel("Open Folder", "", "CSV");

            if (string.IsNullOrEmpty(path)) return;

            var landmarks = new List<Landmark>();
            var reader = new System.IO.StreamReader(path);
            try
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var value = line.Split(',');

                    if (!value.Contains("Name") && value.Length > 3)
                    {
                        string type = value[0];
                        float x;
                        float.TryParse(value[1], out x);
                        float y;
                        float.TryParse(value[2], out y);
                        float z;
                        float.TryParse(value[3], out z);

                        landmarks.Add(new Landmark
                        {
                            Type = type,
                            Position = new Vector3(x, y, z)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
            }
            GeometryManager.Instance.UpdateLandmarks(landmarks);
        }

        private static string ParseTransformString(string transformString)
        {
            string result = transformString;

            var splitInput = transformString.Split(',');
            splitInput = splitInput.Where(val => val != splitInput[0] &&
                                          val != splitInput[7]).ToArray();

            result = string.Join(",", splitInput);
            return result;
        }

        private void OnViewClick()
        {
            SceneManager.LoadScene("Sandbox");
        }

        private void ShowButtons()
        {
            LoadLM.gameObject.SetActive(true);
            ImportTransform.gameObject.SetActive(true);
            View.gameObject.SetActive(true);
        }

        private void HideButtons()
        {
            LoadLM.gameObject.SetActive(false);
            ImportTransform.gameObject.SetActive(false);
            View.gameObject.SetActive(false);
        }

        #endregion
    }
}
