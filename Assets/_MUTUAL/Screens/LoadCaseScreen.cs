using Assets.CaseFile;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Mutual.Screens
{
    class LoadCaseScreen : ILoadCaseScreen
    {
        #region Dependencies

        private readonly Project m_Project;

        #endregion

        #region Private Constants

        private const string LoadCaseBTNName = "LoadCaseBTN";
        private const string ViewBTNName = "ViewBTN";

        #endregion

        #region Private Members

        private Button m_LoadCaseBtn;
        private Button m_ViewBtn;

        #endregion

        #region Constructors

        public LoadCaseScreen(Project project)
        {
            m_Project = project;
        }

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
        }

        public void OnLoadCaseButtonClicked()
        {
            Debug.Log("Load");
            string path = EditorUtility.OpenFilePanel("Select case file", "", "CASE");
            if (path.Length != 0)
            {
                m_Project.LoadProject(path);
            }

            Debug.Log($"Patient Details: {m_Project.PatientData.ToString()}");

            var meshKeys = string.Empty;
            foreach(var key in m_Project.MeshGeoms.Keys)
            {
                meshKeys += (key + ", ");
            }
            Debug.Log($"Meshes: {meshKeys}");

            var landmarks = string.Empty;
            foreach (var lm in m_Project.LandMarks)
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
        }
    }
    #endregion
}
