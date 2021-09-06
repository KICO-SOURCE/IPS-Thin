using Assets._MUTUAL.Viewport;
using Assets.CaseFile;
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
        private readonly ViewportFactory m_ViewportFactory;

        #endregion

        #region Private Constants

        private const string LoadCaseBTNName = "LoadCaseBTN";
        private const string ViewportBTNName = "Viewport";
        private const string ControlParentName = "Overlay";
        private const string ViewBTNName = "View";

        #endregion

        #region Private Members

        private Button m_LoadCaseBtn;
        private Button m_ViewportBtn;
        private Button m_ViewBtn;

        #endregion

        #region Constructors

        public LoadCaseScreen()
        {
            m_Patient = Patient.Instance;
            m_Project = Project.Instance;
            m_CaseFileLoader = CaseFileLoader.Instance;
            m_ViewportContainer = ViewportContainer.Instance;
            m_ViewportFactory = ViewportFactory.Instance;
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
            DettachListeners();
            UnPopulateUiElements();
        }

        public void OnLoadCaseButtonClicked()
        {
            Debug.Log("Load");
            string path = EditorUtility.OpenFilePanel("Select case file", "", "case,CASE,kic,KIC,hic,HIC");
            if (path.Length != 0)
            {
                m_ViewportBtn.enabled = false;
                m_ViewBtn.enabled = false;
                byte[] file = File.ReadAllBytes(path);
                m_CaseFileLoader.LoadCaseFile(file);
                m_CaseFileLoader.LoadComponentData(LoadCompleted, 2);
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

        public void OnViewportButtonClicked()
        {
            GameObject.Find(ControlParentName).SetActive(false);
            m_ViewportFactory.PopulateReportViewports();
            m_ViewportContainer.Activate();
        }
		
		private void OnViewButtonClicked()
        {
            //GameObject.Find(ControlParentName).SetActive(false);
            //m_ViewportFactory.PopulatePlanViewports(2);
            //m_ViewportContainer.Activate();

            var cupAxis = new Vector3(1, 1, 1);
            cupAxis.Normalize();

            var PelvicCoronal = Vector3.forward;
            var cupCenter = Vector3.zero + cupAxis * 10;
            var cupAxisPt = cupCenter + cupAxis * 300;
            var cupAxisCoronalProjection = Vector3.ProjectOnPlane(cupAxis, PelvicCoronal);
            var cross = Vector3.Cross(cupAxis, cupAxisCoronalProjection);

            Plane projPlane = new Plane();
            var projPoint = Vector3.ProjectOnPlane(cupCenter, PelvicCoronal);
            projPlane.Set3Points(cupAxisPt, cupCenter, projPoint);

            Debug.Log($"Plane normal: {projPlane.normal}, Dot: {Vector3.Dot(projPlane.normal, cross)}");

            var cross1 = Vector3.Cross(cupAxis, PelvicCoronal);
            Debug.Log($"Cross normal: {cross1.normalized}, Dot: {Vector3.Dot(cross1.normalized, cross)}");
        }

        #endregion

        #region Private Methods

        private void LoadCompleted()
        {
            m_ViewportBtn.enabled = true;
            m_ViewBtn.enabled = true;
        }

        private void PopulateUiElements()
        {
            m_LoadCaseBtn = GameObject.Find(LoadCaseBTNName).GetComponent<Button>();
            m_ViewportBtn = GameObject.Find(ViewportBTNName).GetComponent<Button>();
            m_ViewBtn = GameObject.Find(ViewBTNName).GetComponent<Button>();
        }

        private void AttachListeners()
        {
            m_LoadCaseBtn.onClick.AddListener(OnLoadCaseButtonClicked);
            m_ViewportBtn.onClick.AddListener(OnViewportButtonClicked);
            m_ViewBtn.onClick.AddListener(OnViewButtonClicked);
        }

        private void DettachListeners()
        {
            m_LoadCaseBtn?.onClick.RemoveAllListeners();
            m_ViewportBtn?.onClick.RemoveAllListeners();
            m_ViewBtn?.onClick.RemoveAllListeners();
        }

        private void UnPopulateUiElements()
        {
            m_LoadCaseBtn = null;
            m_ViewportBtn = null;
            m_ViewBtn = null;
        }
    }

    #endregion
}
