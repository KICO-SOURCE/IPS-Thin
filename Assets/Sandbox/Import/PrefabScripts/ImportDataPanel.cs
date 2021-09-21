using Assets.Geometries;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Import.PrefabScripts
{
    /// <summary>
    /// Import Data Panel UI class.
    /// </summary>
    public class ImportDataPanel : MonoBehaviour
    {
        internal static string PrefabName => "ImportDataPanel";

        #region Private Members

        private string meshFilePath;

        #endregion

        #region Public Properties

        public TMP_Text FileNameTXT;
        public Button OpenBTN;
        public Button CancelBTN;
        public InputField ManualTypeInput;
        public ToggleGroup ToggleGroup;
        public TMP_Text WarningTXT;
        public Action DataPanelClosed;

        #endregion

        #region Public Methods

        public void Start()
        {
            AttachListeners();
            OpenBTN.interactable = false;
            DisableToggle();
        }

        public void Update()
        {
            var tag = GetTag();
            var warning = string.Empty;
            if (GeometryManager.Instance.IsExistingTag(tag))
            {
                warning = tag + " is already added. " +
                        "Please change the stl type to be loaded.";
                tag = string.Empty;
            }
            WarningTXT.text = warning;
            OpenBTN.interactable = !string.IsNullOrWhiteSpace(tag);
        }

        /// <summary>
        /// Set the loaded filename as title.
        /// </summary>
        /// <param name="file path"></param>
        public void SetFileTitle(string filePath)
        {
            meshFilePath = filePath;
            var loadedFile = System.IO.Path.GetFileName(filePath);

            FileNameTXT.text = loadedFile + " loaded.";
            Debug.Log("File Name:" + FileNameTXT.text);
            WarningTXT.text = "";
        }

        /// <summary>
        /// Attach Listeners.
        /// </summary>
        public void AttachListeners()
        {
            CancelBTN.onClick.AddListener(OnCancelBtnClick);
            OpenBTN.onClick.AddListener(OnOpenBtnClick);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Open the stl specified.
        /// </summary>
        private void OnOpenBtnClick()
        {
            var tag = GetTag();
            if (string.IsNullOrWhiteSpace(tag)) return;

            var type = GetActiveToggle()?.name;
            GeometryManager.Instance.LoadMesh(tag, type, meshFilePath);
            meshFilePath = string.Empty;

            Debug.Log("Active Toggle Name: " + type);
            Debug.Log("Manual Input : " + ManualTypeInput.text);

            ManualTypeInput.text = null;
            DisableToggle();
            this.gameObject.SetActive(false);
            DataPanelClosed?.Invoke();
        }

        /// <summary>
        /// Close the detailed stl UI.
        /// </summary>
        private void OnCancelBtnClick()
        {
            this.gameObject.SetActive(false);
            ManualTypeInput.text = null;
            DisableToggle();
            DataPanelClosed?.Invoke();
        }

        /// <summary>
        /// Get the object tag
        /// </summary>
        /// <returns></returns>
        private string GetTag()
        {
            var activeToggle = GetActiveToggle();
            var tag = string.IsNullOrWhiteSpace(ManualTypeInput.text) ?
                            activeToggle?.name : ManualTypeInput.text;
            return tag;
        }

        /// <summary>
        /// Disable active toggles.
        /// </summary>
        private void DisableToggle()
        {
            var toggles = ToggleGroup.GetComponentsInChildren<Toggle>();
            foreach (var type in toggles)
            {
                type.isOn = false;
            }
        }

        /// <summary>
        /// Get active toggle.
        /// </summary>
        /// <returns></returns>
        private Toggle GetActiveToggle()
        {
            var toggles = ToggleGroup.GetComponentsInChildren<Toggle>();
            foreach (var type in toggles)
            {
                if (type.isOn)
                {
                    return type;
                }
            }
            return null;
        }

        #endregion
    }
}
