using Assets.CaseFile;
using Assets.Geometries;
using Assets.Sandbox.Import;
using System;
using System.Collections.Generic;
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
        private TMP_InputField ManualTypeInput;
        private ToggleGroup ToggleGroup;

        #endregion

        #region Public Properties

        public MeshData meshData { get; set; }
        public List<Landmark> landmarksData = new List<Landmark>();
        public Action DataPanelClosed;
        #endregion

        #region Public Methods

        public void Awake()
        {
            FileNameTXT = transform.Find("FileNameTXT").GetComponent<TMP_Text>();
            ToggleGroup = transform.Find("ObjectTypeContainer/ToggleGroup").GetComponent<ToggleGroup>();
            OpenBTN = transform.Find("ButtonPanel/OpenBtn").GetComponent<Button>();
            CancelBTN= transform.Find("ButtonPanel/CancelBtn").GetComponent<Button>();
            ManualTypeInput = transform.Find("ManualTypeContainer/ManualTypeInput").GetComponent<TMP_InputField>();
        }

        public void Start()
        {
            AttachListeners();
        }

        /// <summary>
        /// Set the loaded filename as title.
        /// </summary>
        /// <param name="text"></param>
        public void SetTitle(string text)
        {
            FileNameTXT.text = text + " loaded.";
            Debug.Log("File Name:" + FileNameTXT.text);
        }

        /// <summary>
        /// Attach Listeners.
        /// </summary>
        public void AttachListeners()
        {
            CancelBTN.onClick.AddListener(OnCancelBtnClick);
            OpenBTN.onClick.AddListener(OnOpenBtnClick);
        }

        /// <summary>
        /// Set landmark data as list.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="position"></param>
        public void SetLandmarkData(string type,Vector3 position)
        {
            landmarksData.Add(new Landmark
            {
                Type = type,
                Position = position
            });
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Open the stl specified.
        /// </summary>
        private void OnOpenBtnClick()
        {
            var activeToggle = GetActiveToggle();
            Debug.Log(activeToggle.name);

            Geometry geometryContent = new Geometry();
            Debug.Log("Manual Input : " + ManualTypeInput.text);
            geometryContent.Tag = ManualTypeInput.text;

            if(meshData!=null)
            {
                geometryContent.Mesh = meshData.ToMesh();
                meshData = null;
            }
            else if(null != landmarksData)
            {
                foreach (var lm in landmarksData)
                {
                    lm.Bone = activeToggle.name;
                }
                geometryContent.UpdateLandmarks(landmarksData);
                landmarksData = new List<Landmark>();
            }

            geometryContent.ObjectType = GetObjectType(GetActiveToggle());
            GeometryManager.Instance.ShowList();
            GeometryManager.Instance.UpdateDisplayList(geometryContent);

            Debug.Log("Tag :" + geometryContent.Tag);
            Debug.Log("Object Type : " + geometryContent.ObjectType);

            this.gameObject.SetActive(false);
            DataPanelClosed?.Invoke();
            ManualTypeInput.text = null;
            DisableToggle();
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
        /// Disable active toggles.
        /// </summary>
        private void DisableToggle()
        {
            var toggles=ToggleGroup.GetComponentsInChildren<Toggle>();
            foreach(var type in toggles)
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
                if(type.isOn)
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
        private ObjectType GetObjectType(Toggle type)
        {
            var obj=Enum.GetValues(typeof(ObjectType)).Cast<ObjectType>().ToList();
            if (type != null)
            {
                foreach (var item in obj)
                {
                    if (type.name == item.ToString())
                    {
                        return item;
                    }
                }
            }
            return ObjectType.Other;
        }

        #endregion
    }
}
