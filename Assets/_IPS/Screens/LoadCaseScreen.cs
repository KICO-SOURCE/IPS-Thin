using Assets.CaseFile;
using System.Collections.Generic;
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

        #endregion

        #region Private Constants

        private const string ParentTag = "MeshParent";
        private const string LoadCaseBTNName = "LoadCaseBTN";
        private const string ViewBTNName = "ViewBTN";

        #endregion

        #region Private Members

        private Button m_LoadCaseBtn;
        private Button m_ViewBtn;
        private Transform m_Parent;
        private List<GameObject> m_Meshes;

        #endregion

        #region Constructors

        public LoadCaseScreen(Patient patient, Project project,
                              CaseFileLoader caseFileLoader)
        {
            m_Patient = patient;
            m_Project = project;
            m_CaseFileLoader = caseFileLoader;
            m_Parent = GameObject.FindGameObjectWithTag(ParentTag).transform;
            m_Meshes = new List<GameObject>();
        }

        #endregion

        #region Public Methods

        public void ActivateScreen()
        {
            Debug.Log("Activated");
            PopulateUiElements();
            AttachListeners();
        }

        public void DeactivateScreen()
        {
            Camera.main.gameObject.SetActive(true);
            DettachListeners();
            UnPopulateUiElements();
        }

        public void OnLoadCaseButtonClicked()
        {
            Debug.Log("Load");
            string path = EditorUtility.OpenFilePanel("Select case file", "", "CASE");
            if (path.Length != 0)
            {
                m_CaseFileLoader.LoadCaseFile(path);
            }

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

        public void OnViewButtonClicked()
        {
            Debug.Log("View");

            foreach (var go in m_Meshes)
            {
                go.SetActive(false);
                UnityEngine.Object.DestroyImmediate(go);
            }
            m_Meshes.Clear();


            var material = Resources.Load<Material>("Materials/BoneMaterial");
            foreach (var mesh in m_Patient.MeshGeoms)
            {
                var go = new GameObject(mesh.Key, typeof(MeshFilter), typeof(MeshRenderer));
                go.transform.parent = m_Parent;
                go.GetComponent<MeshFilter>().mesh = mesh.Value;
                go.GetComponent<MeshRenderer>().material = material;
                go.transform.localPosition = new Vector3(0, 100, 100);
                go.SetActive(true);
                m_Meshes.Add(go);
            }
        }

        #endregion

        #region Private Methods

        private void PopulateUiElements()
        {
            m_LoadCaseBtn = GameObject.Find(LoadCaseBTNName).GetComponent<Button>();
            m_ViewBtn = GameObject.Find(ViewBTNName).GetComponent<Button>();
        }

        private void AttachListeners()
        {
            m_ViewBtn.onClick.AddListener(OnViewButtonClicked);
            m_LoadCaseBtn.onClick.AddListener(OnLoadCaseButtonClicked);
        }

        private void DettachListeners()
        {
            m_ViewBtn.onClick.RemoveAllListeners();
            m_LoadCaseBtn.onClick.RemoveAllListeners();
        }

        private void UnPopulateUiElements()
        {
            m_LoadCaseBtn = null;
            m_ViewBtn = null;

            foreach(var go in m_Meshes)
            {
                UnityEngine.Object.DestroyImmediate(go);
            }
            m_Meshes.Clear();
        }
    }
    #endregion
}
