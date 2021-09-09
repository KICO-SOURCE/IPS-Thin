using Assets.CaseFile;
using Assets.Geometries;
using System.Collections.Generic;
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
        private GameObject ComponentSelctionPanel;
        private GameObject BoneSelectionPanel;
        private Toggle[] Components;
        private Toggle[] Bones;
        
        #endregion

        public MeshData meshData;
        public List<Landmark> landmarksData;

        #region Public Methods

        public void Awake()
        {
            FileNameTXT = transform.Find("FileNameTXT").GetComponent<TMP_Text>();
            ComponentSelctionPanel = transform.Find("ObjectTypeContainer/ComponentSelectionPanel").gameObject;
            BoneSelectionPanel = transform.Find("ObjectTypeContainer/BoneSelectionPanel").gameObject;
            OpenBTN= transform.Find("ButtonPanel/OpenBtn").GetComponent<Button>();
            CancelBTN= transform.Find("ButtonPanel/CancelBtn").GetComponent<Button>();
            ManualTypeInput = transform.Find("ManualTypeContainer/ManualTypeInput").GetComponent<TMP_InputField>();
        }

        public void Start()
        {
            AttachListeners();
            Components = ComponentSelctionPanel.GetComponentsInChildren<Toggle>();
            Bones = BoneSelectionPanel.GetComponentsInChildren<Toggle>();
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
            if(landmarksData == null)
            {
                landmarksData = new List<Landmark>();
            }
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

            Geometry stlContent = new Geometry();
            stlContent.Tag = ManualTypeInput.text;

            if(meshData!=null)
            {
                stlContent.Mesh = meshData.ToMesh();
                meshData = null;
            }
            else if(null != landmarksData)
            {
                stlContent.UpdateLandmarks(activeToggle.name, landmarksData);
                landmarksData = null;
            }

            // stlContent.ObjectType = ;
            GeometryManager.Instance.AddContent(stlContent);

            Debug.Log("Manual Input : " + ManualTypeInput.text);

            this.gameObject.SetActive(false);
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
        }

        private void DisableToggle()
        {
            foreach (var comp in Components)
            {
                comp.isOn = false;
            }

            foreach (var bone in Bones)
            {
                bone.isOn = false;
            }
        }

        private Toggle GetActiveToggle()
        {
            foreach (var comp in Components)
            {
                if (comp.isOn) return comp;
            }

            foreach (var bone in Bones)
            {
                if (bone.isOn) return bone;
            }
            return null;
        }

        #endregion
    }
}
