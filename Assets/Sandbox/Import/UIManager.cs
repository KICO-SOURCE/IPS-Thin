using Assets.CaseFile;
using Assets.Geometries;
using Assets.Import.PrefabScripts;
using Assets.Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Import
{
    /// <summary>
    /// UI manager class.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        #region Public Members

        public GameObject ViewPort;

        #endregion

        #region Private Members

        private LeftSidePanel LeftSidePanel;
        private ImportDataPanel ImportDataPanel;
        private RightSidePanel RightSidePanel;
        private Color normalColor;
        private ThreeDScene ThreeDScene;

        #endregion

        #region Public Methods

        public void Start()
        {
            LeftSidePanel= transform.Find("LeftSidePanel").GetComponent<LeftSidePanel>();
            ImportDataPanel = transform.Find("ImportDataPanel").GetComponent<ImportDataPanel>();
            ThreeDScene = GameObject.Find("ThreeDContainer").GetComponent<ThreeDScene>();
            RightSidePanel = transform.Find("RightSidePanel").GetComponent<RightSidePanel>();
            GeometryManager.Instance.DisplayList();
            AttachListeners();
            ImportDataPanel.DataPanelClosed += AddButton;
            normalColor = RightSidePanel.TransparentBtn.GetComponent<Image>().color;
        }

        private void Update()
        {
            LeftSidePanel.LoadLMBtn.interactable = GeometryManager.Instance.EnableLoad;
            LeftSidePanel.ImportTransformBtn.interactable = GeometryManager.Instance.EnableLoad;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Attach listeners.
        /// </summary>
        private void AttachListeners()
        {
            LeftSidePanel.LoadLMBtn.onClick.AddListener(OnLoadLMClick);
            LeftSidePanel.LoadSTLBtn.onClick.AddListener(OnLoadStlClick);
            LeftSidePanel.ImportTransformBtn.onClick.AddListener(OnImportTransformClick);

            RightSidePanel.TransparentBtn.onClick.AddListener(OnTransparentClick);
        }

        /// <summary>
        /// Load STL button click listener.
        /// </summary>
        private void OnLoadStlClick()
        {
            LoadStl();
        }

        /// <summary>
        /// Add "Load New STL" button to layout group.
        /// </summary>
        private void AddButton()
        {
            var parent = GameObject.FindGameObjectWithTag("ListParent");
            LeftSidePanel.LoadSTLBtn.transform.SetParent(parent.transform, false);
            LeftSidePanel.LoadSTLBtn.transform.SetAsLastSibling();

            ThreeDScene.DisplayMesh();
        }

        /// <summary>
        /// Load Stl.
        /// </summary>
        private void LoadStl()
        {
            string path = EditorUtility.OpenFilePanel("Open file", "", "STL");

            if (string.IsNullOrEmpty(path)) return;

            ThreeDScene.gameObject.SetActive(false);
            ImportDataPanel.gameObject.SetActive(true);
            var loadedFile = System.IO.Path.GetFileName(path);
            ImportDataPanel.meshData = MeshGeometryFunctions.ReadStl(path);
            ImportDataPanel.SetTitle(loadedFile);

        }

        /// <summary>
        /// Load landmark button click listener.
        /// </summary>
        private void OnLoadLMClick()
        {
            LoadLandmarksFromCSV();
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
            ThreeDScene.DisplayMesh();
        }

        /// <summary>
        /// Load landmarks from csv.
        /// </summary>
        private void LoadLandmarksFromCSV()
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

                    if (!(value.Length > 4) && !value.Contains("Name") && CheckFormat(value))
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
            ThreeDScene.DisplayMesh();
        }

        /// <summary>
        /// Parse transform from string data.
        /// </summary>
        /// <param name="transformString"></param>
        /// <returns></returns>
        private static string ParseTransformString(string transformString)
        {
            string result = transformString;

            var splitInput = transformString.Split(',');
            splitInput = splitInput.Where(val => val != splitInput[0] &&
                                          val != splitInput[7]).ToArray();

            result = string.Join(",", splitInput);
            return result;
        }

        /// <summary>
        /// validate landmark data format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CheckFormat(string[] value)
        {
            if (IsChar(value[0]) && IsNumeric(value[1]) && IsNumeric(value[2]) && IsNumeric(value[3]))
                return true;
            return false;
        }

        /// <summary>
        /// Checks the string contains numeric value.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool IsNumeric(string val)
        {
            foreach (var str in val)
            {
                if (Char.IsDigit(str))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the string contains char data.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool IsChar(string val)
        {
            foreach (var str in val)
            {
                if (!(Char.IsDigit(str)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Transparent button click listener.
        /// </summary>
        private void OnTransparentClick()
        {
            GeometryManager.Instance.ToggleTransparency();
            var color = GeometryManager.Instance.Transparent ?
                                    Color.gray : normalColor;

            RightSidePanel.TransparentBtn.GetComponentInChildren<Image>().color = color;
        }
        #endregion
    }
}
