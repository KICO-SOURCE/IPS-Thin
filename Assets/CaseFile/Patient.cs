using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.CaseFile
{
    public class Patient
    {
        public int PatientId { get; set; }

        public int SurgeryId { get; set; }

        public string PatientName { get; set; }

        public string PatientFirstName { get; set; }

        public string PatientLastName { get; set; }

        public string CaseNumber { get; set; }

        public string Leftright { get; set; }

        public string SurgeonName { get; set; }

        public string Dob { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime SurgeryDate { get; set; }

        public DateTime DateOfScan { get; set; }

        public string Mrn { get; set; }

        public int Gender { get; set; }

        public string Hospital { get; set; }

        public string SegmentationPath { get; set; }

        public DateTime PostOpDateOfScan { get; set; }

        public string PostOpCaseCode { get; set; }

        public Dictionary<string, Mesh> MeshGeoms { get; set; }

        public List<Landmark> Landmarks { get; set; }

        public Patient()
        {
            InitializeData();
        }

        public void InitializeData()
        {
            MeshGeoms = new Dictionary<string, Mesh>();
            Landmarks = new List<Landmark>();
        }

        public Vector3 GetLandmarkPosition(string landmarkType)
        {
            var landmark = Landmarks.FirstOrDefault(lm => lm.Type == landmarkType);
            return landmark?.Position ?? Vector3.zero;
        }

        public Dictionary<string, Mesh> GetMeshes(params string[] meshKeys)
        {
            if (meshKeys == null || meshKeys.Length <= 0) return null;
            return MeshGeoms.Where(m => meshKeys.Contains(m.Key))
                            .ToDictionary(x => x.Key, x => x.Value);
        }
    }
}
