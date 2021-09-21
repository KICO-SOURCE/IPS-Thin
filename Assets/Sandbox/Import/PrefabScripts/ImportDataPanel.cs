using Assets.CaseFile;
using Assets.Geometries;
using Assets.Sandbox.Import;
using System;
using System.Linq;
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

        private TMP_Text FileNameTXT;
        private Button OpenBTN;
        private Button CancelBTN;
        private InputField ManualTypeInput;
        private ToggleGroup ToggleGroup;
        private TMP_Text WarningTXT;

        #endregion

        #region Public Properties

        public MeshData meshData { get; set; }
        public Action DataPanelClosed;

        #endregion

        #region Public Methods

        public void Awake()
        {
            FileNameTXT = transform.Find("FileNameTXT").GetComponent<TMP_Text>();
            ToggleGroup = transform.Find("ObjectTypeContainer/ToggleGroup").GetComponent<ToggleGroup>();
            OpenBTN = transform.Find("ButtonPanel/OpenBtn").GetComponent<Button>();
            CancelBTN = transform.Find("ButtonPanel/CancelBtn").GetComponent<Button>();
            ManualTypeInput = transform.Find("ManualTypeContainer/ManualTypeInput").GetComponent<InputField>();
            WarningTXT = transform.Find("WarningTXT").GetComponent<TMP_Text>();
        }

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
            if (GeometryManager.Instance.Geometries.Any(g => g.Tag == tag))
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
        /// <param name="text"></param>
        public void SetTitle(string text)
        {
            FileNameTXT.text = text + " loaded.";
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

            Geometry geometryContent = new Geometry();
            geometryContent.Tag = tag;

            if (meshData != null)
            {
                geometryContent.Mesh = meshData.ToMesh();
                meshData = null;
            }

            geometryContent.ObjectType = GetObjectType(tag);
            GeometryManager.Instance.UpdateDisplayList(geometryContent);

            Debug.Log("Active Toggle Name: " + GetActiveToggle()?.name);
            Debug.Log("Manual Input : " + ManualTypeInput.text);
            Debug.Log("Tag :" + geometryContent.Tag);
            Debug.Log("Object Type : " + geometryContent.ObjectType);

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
            GeometryManager.Instance.ShowList();
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

        /// <summary>
        /// Get object type.
        /// </summary>
        /// <param name="type"></param>
        private ObjectType GetObjectType(string typeName)
        {
            var objectTypes = Enum.GetValues(typeof(ObjectType)).Cast<ObjectType>();

            var type = objectTypes.FirstOrDefault(ob =>
                            ob.ToString().ToLower() == typeName.ToLower());

            return type;
        }

        #endregion
    }
}
