using Assets._MUTUAL.Viewport;
using Assets.CaseFile;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Ips.Screens
{
    class LoadCaseScreen : ILoadCaseScreen
    {
        #region Dependencies

        private readonly Patient m_Patient;
        private readonly Project m_Project;
        private readonly CaseFileLoader m_CaseFileLoader;
        private readonly ViewportContainer m_ViewportContainer;

        #endregion

        #region Private Constants

        private const string LoadCaseBTNName = "LoadCaseBTN";
        private const string ViewportBTNName = "Viewport";
        private const string ControlParentName = "Overlay";

        #endregion

        #region Private Members

        private Button m_LoadCaseBtn;
        private Button m_ViewportBtn;

        #endregion

        #region Constructors

        public LoadCaseScreen(Patient patient, Project project,
                              CaseFileLoader caseFileLoader, ViewportContainer viewportContainer)
        {
            m_Patient = patient;
            m_Project = project;
            m_CaseFileLoader = caseFileLoader;
            m_ViewportContainer = viewportContainer;
        }

        #endregion

        #region Public Methods

        public void ActivateScreen()
        {
            Debug.Log("Activated");
            PopulateUiElements();
            AttachListeners();
            m_ViewportBtn.enabled = false;
        }

        public void DeactivateScreen()
        {
            DettachListeners();
            UnPopulateUiElements();
        }

        public void OnLoadCaseButtonClicked()
        {
            Debug.Log("Load");
            string path = EditorUtility.OpenFilePanel("Select case file", "", "case,CASE,kic,KIC,hic,HIC");
            if (path.Length != 0)
            {
                byte[] file = File.ReadAllBytes(path);
                m_CaseFileLoader.LoadCaseFile(file);
            }

            m_ViewportBtn.enabled = true;

            Debug.Log($"Patient Details: \n");
            Debug.Log($"Patient ID: {m_Patient.PatientId}");
            Debug.Log($"Surgery ID: {m_Patient.SurgeryId}");
            Debug.Log($"Name: {m_Patient.PatientFirstName} {m_Patient.PatientLastName}");
            Debug.Log($"Case Number: {m_Patient.CaseNumber}");
            Debug.Log($"Side: {m_Patient.Leftright}");
            Debug.Log($"Surgeon: {m_Patient.SurgeonName}");
            Debug.Log($"Date of birth: {m_Patient.Dob}");
            Debug.Log($"Date of creation: {m_Patient.CreationDate.ToString()}");
            Debug.Log($"Date of surgery: {m_Patient.SurgeryDate.ToString()}");
            Debug.Log($"Date of scan: {m_Patient.DateOfScan.ToString()}");
            Debug.Log($"MRN: {m_Patient.Mrn}");
            Debug.Log($"Gender: {m_Patient.Gender}");
            Debug.Log($"Hospital: {m_Patient.Hospital}");
            Debug.Log($"Segmentation path: {m_Patient.SegmentationPath}");
            Debug.Log($"PostOp case code: {m_Patient.PostOpCaseCode}");
            Debug.Log($"PostOp date of scan: {m_Patient.PostOpDateOfScan.ToString()}");

            var meshKeys = string.Empty;
            foreach (var key in m_Patient.MeshGeoms.Keys)
            {
                meshKeys += (key + ", ");
            }
            Debug.Log($"Meshes: {meshKeys}");

            var landmarks = string.Empty;
            foreach (var lm in m_Patient.Landmarks)
            {
                landmarks += $"Bone:{lm.Bone}, Type: {lm.Type}, Position: {lm.Position}\n";
            }
            Debug.Log($"Landmarks: \n{landmarks}");

            var cases = string.Empty;
            foreach (var plan in m_Project.PlanValues)
            {
                foreach (var measurement in plan)
                {
                    cases += $"{measurement.Key} {measurement.Value}\n";
                }
                cases += $"\n";
            }
            Debug.Log($"Cases: \n{cases}");

            var functionalValues = string.Empty;
            foreach (var value in m_Project.FunctionalValues)
            {
                functionalValues += $"ID: {value.IdFunctionalValues}, " +
                    $"Measure: {value.Measure}, Value: {value.MeasureValue}\n";
            }
            Debug.Log($"Functional values: \n{functionalValues}");
        }

        public void OnViewportButtonClicked()
        {
            GameObject.Find(ControlParentName).SetActive(false);
            m_ViewportContainer.Activate();
        }

        #endregion

        #region Private Methods

        private void PopulateUiElements()
        {
            m_LoadCaseBtn = GameObject.Find(LoadCaseBTNName).GetComponent<Button>();
            m_ViewportBtn = GameObject.Find(ViewportBTNName).GetComponent<Button>();
        }

        private void AttachListeners()
        {
            m_LoadCaseBtn.onClick.AddListener(OnLoadCaseButtonClicked);
            m_ViewportBtn.onClick.AddListener(OnViewportButtonClicked);
        }

        private void DettachListeners()
        {
            m_LoadCaseBtn?.onClick.RemoveAllListeners();
            m_ViewportBtn?.onClick.RemoveAllListeners();
        }

        private void UnPopulateUiElements()
        {
            m_LoadCaseBtn = null;
            m_ViewportBtn = null;
        }
    }

    #endregion
}
