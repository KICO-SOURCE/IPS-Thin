using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace Assets.CaseFile
{
    internal class Project
    {
        #region Fields

        public Patient PatientData { get; set; }
        public Dictionary<string, Mesh> MeshGeoms { get; set; }
        public List<Landmark> LandMarks { get; set; }
        public Dictionary<string, string> PlanValues { get; set; }
        public List<Measurement> FunctionalValues { get; set; }

        #endregion

        #region Private Methods

        private void InitializeData()
        {
            PatientData = new Patient();
            MeshGeoms = new Dictionary<string, Mesh>();
            LandMarks = new List<Landmark>();
            PlanValues = new Dictionary<string, string>();
        }

        private void LoadLandmarks(XmlTextReader x)
        {
            string bone = x.GetAttribute("Bone");
            string type = x.GetAttribute("ID");
            var x1 = float.Parse(x.GetAttribute("X"));
            var y = float.Parse(x.GetAttribute("Y"));
            var z = float.Parse(x.GetAttribute("Z"));

            LandMarks.Add(new Landmark()
            {
                Bone = bone,
                Type = type,
                 Position = new Vector3(x1, y, z)
            });
        }

        private DateTime ParseDate(string dateString)
        {
            DateTime date = DateTime.Now;
            if (string.IsNullOrEmpty(dateString)) return date;
            string[] formats = { "dd/MM/yyyy h:m:s tt","MM/dd/yyyy h:m:s tt",
                                 "d/M/yyyy h:m:s tt", "M/d/yyyy h:m:s tt",
                                 "dd-MM-yyyy h:m:s tt", "MM-dd-yyyy h:m:s tt",
                                 "d-M-yyyy h:m:s tt", "M-d-yyyy h:m:s tt",
                                 "dd-MM-yyyy", "MM-dd-yyyy", "dd-MMM-yyyy",
                                 "dd/MM/yyyy", "MM/dd/yyyy", "dd/MMM/yyyy" };

            for (int i = 0; i < formats.Length; i++)
            {
                try
                {
                    date = DateTime.ParseExact(dateString, formats[i], CultureInfo.InvariantCulture);
                    break;
                }
                catch
                {
                    if (formats.Length - 1 != i) continue;
                    try
                    {
                        date = DateTime.Parse(dateString);
                    }
                    catch
                    {
                        return date;
                    }
                }
            }
            return date;
        }

        private string ModifyFormat(string dateString)
        {
            try
            {

                DateTime date = DateTime.ParseExact(dateString, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                return dateString;
            }
            catch
            {
                try
                {
                    DateTime date = ParseDate(dateString);
                    return date.ToString("dd-MM-yyyy");
                }
                catch
                {
                    return dateString;
                }
            }
        }

        #endregion

        #region Public Methods

        public Project()
        {
        }

        /// <summary>
        /// Load case file for knee and hip.
        /// This logic may change after the filetype defnitition.
        /// </summary>
        /// <param name="sourceByte"></param>
        /// <returns></returns>
        internal bool LoadProject(byte[] sourceByte)
        {
            InitializeData();
            string fp;
            bool chck = DecryptTheProject(sourceByte, out fp);

            if (!chck)
            {
                Console.WriteLine("Unable to open file the file",
                    "Case file is corrupt please re-download the file from the 360 Server");
                System.IO.File.Delete(fp);
                return false;
            }

            using (XmlTextReader x = new XmlTextReader(fp))
            {
                x.Read();

                while (!x.EOF)
                {
                    if (x.MoveToContent() == XmlNodeType.Element && x.Name == "CaseFile")
                    {
                        try
                        {
                            PatientData.PatientId = int.Parse(x.GetAttribute("PatientID"));
                        }
                        catch
                        {
                            PatientData.PatientId = 0;
                        }
                        try
                        {
                            PatientData.SurgeryId = int.Parse(x.GetAttribute("SurgeryID"));
                        }
                        catch
                        {
                            PatientData.SurgeryId = 0;
                        }
                        PatientData.PatientName = x.GetAttribute("PatientName");
                        PatientData.PatientFirstName = x.GetAttribute("FirstName");
                        if (string.IsNullOrEmpty(PatientData.PatientFirstName))
                        {
                            var name = PatientData.PatientName.Split(' ');
                            PatientData.PatientFirstName = name[0];
                            PatientData.PatientLastName = PatientData.PatientName.Substring(PatientData.PatientFirstName.Length, PatientData.PatientName.Length - PatientData.PatientFirstName.Length).TrimStart();
                        }
                        else
                        {
                            PatientData.PatientLastName = PatientData.PatientName.Substring(PatientData.PatientFirstName.Length, PatientData.PatientName.Length - PatientData.PatientFirstName.Length).TrimStart();
                        }
                        PatientData.CaseNumber = x.GetAttribute("CaseNumber");
                        PatientData.Leftright = x.GetAttribute("Leg");
                        PatientData.SurgeonName = x.GetAttribute("Surgeon");

                        PatientData.Dob = ModifyFormat(x.GetAttribute("DateOfBith"));
                        PatientData.CreationDate = ParseDate(x.GetAttribute("CreationDate"));
                        PatientData.SurgeryDate = ParseDate(x.GetAttribute("SurgeryDate"));
                        var dos = x.GetAttribute("DateOfScan");
                        if (!string.IsNullOrEmpty(dos))
                        {
                            PatientData.DateOfScan = ParseDate(dos);
                        }

                        try
                        {
                            PatientData.Mrn = x.GetAttribute("MRN");
                        }
                        catch
                        {
                            PatientData.Mrn = "";
                        }

                        try
                        {
                            PatientData.Gender = int.Parse(x.GetAttribute("Gender"));
                        }
                        catch
                        {
                            PatientData.Gender = 0;
                        }


                        try
                        {
                            PatientData.Hospital = int.Parse(x.GetAttribute("Hospital"));
                        }
                        catch
                        {
                            PatientData.Hospital = 0;
                        }

                        try
                        {
                            PatientData.SegmentationPath = x.GetAttribute("SegmentationPath");
                        }
                        catch
                        {
                            PatientData.SegmentationPath = " ";
                        }

                        if (!x.Read()) break;
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "Meshes"))
                    {
                        while (!x.EOF)
                        {
                            string bone = string.Empty;
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "MeshData")
                            {
                                if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Bone")
                                {
                                    bone = x.Value;
                                }
                                x.MoveToContent();
                                if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Mesh")
                                {
                                    MeshGeoms.Add(bone, MeshGeometryFunctions.GetXmlMeshGeometry3D(x));
                                }
                                x.MoveToContent();
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "Meshes") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "LandMarks"))
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "LandMark")
                            {
                                LoadLandmarks(x);
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "LandMarks") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Cases")
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Case")
                            {
                                string implantTypeStr = x.GetAttribute("ImplantType");
                                PlanValues.Add("FemurSizeString", x.GetAttribute("FemurImplant"));
                                PlanValues.Add("FemurVarientString", x.GetAttribute("FemurImplantVarient"));
                                PlanValues.Add("FemVv", x.GetAttribute("FemurVV"));
                                PlanValues.Add("FemFlex", x.GetAttribute("FemurFE"));
                                PlanValues.Add("FemIe", x.GetAttribute("FemurIE"));
                                PlanValues.Add("FemMl", x.GetAttribute("FemurML"));
                                PlanValues.Add("FemAp", x.GetAttribute("FemurAP"));
                                PlanValues.Add("FemSi", x.GetAttribute("FemurSI"));
                                PlanValues.Add("TibiaInsertSizeString", x.GetAttribute("TibiaInsert"));
                                PlanValues.Add("TibiaInsertVarientString", x.GetAttribute("TibiaInsertVarient"));
                                PlanValues.Add("TibiaVarientString", x.GetAttribute("TibiaImplant"));
                                PlanValues.Add("TibiaSizeString", x.GetAttribute("TibiaImplantSize"));
                                PlanValues.Add("TibVv", x.GetAttribute("TibiaVV"));
                                PlanValues.Add("TibFlex", x.GetAttribute("TibiaFE"));
                                PlanValues.Add("TibIe", x.GetAttribute("TibiaIE"));
                                PlanValues.Add("TibMl", x.GetAttribute("TibiaML"));
                                PlanValues.Add("TibAp", x.GetAttribute("TibiaAP"));
                                PlanValues.Add("TibSi", x.GetAttribute("TibiaSI"));
                                PlanValues.Add("PatellaSizeString", x.GetAttribute("PatellaImplant"));
                                PlanValues.Add("PatellaVarientString", x.GetAttribute("PatellaImplantVariant"));
                                PlanValues.Add("PatellaFE", x.GetAttribute("PatellaFE"));
                                PlanValues.Add("PatellaIE", x.GetAttribute("PatellaIE"));
                                string patellaSpin = x.GetAttribute("PatellaSpin");
                                PlanValues.Add("PatellaSpin", string.IsNullOrEmpty(patellaSpin) ? "" : patellaSpin);
                                PlanValues.Add("PatellaML", x.GetAttribute("PatellaML"));
                                PlanValues.Add("PatellaAP", x.GetAttribute("PatellaAP"));
                                PlanValues.Add("PatellaSI", x.GetAttribute("PatellaSI"));
                                PlanValues.Add("Notes", x.GetAttribute("CaseNotes"));
                                PlanValues.Add("SurgeonsNotes", x.GetAttribute("SurgeonsNotes"));
                                PlanValues.Add("AlignmentLabel", x.GetAttribute("AlignmentLabel"));

                                try
                                {
                                    PlanValues.Add("KicoCaseId", x.GetAttribute("KicoCaseID"));
                                    PlanValues.Add("CaseHistoryId", x.GetAttribute("CaseHistoryID"));
                                    PlanValues.Add("AlignmentType", x.GetAttribute("AlignmentType"));
                                    PlanValues.Add("EngineerID", x.GetAttribute("Engineer"));

                                    PlanValues.Add("Approve", x.GetAttribute("Approve"));
                                    PlanValues.Add("ChangeRequest", x.GetAttribute("ChangeRequest"));
                                    PlanValues.Add("SurgeonAlignmentID", (x.GetAttribute("SurgeonAlignmentID")));
                                    PlanValues.Add("AnalysisType", x.GetAttribute("AnalysisType"));
                                    if (!string.IsNullOrEmpty(x.GetAttribute("LightBenderAlignment")))
                                    {
                                        var lightBenderAlignment = bool.Parse(x.GetAttribute("LightBenderAlignment"));
                                        PlanValues.Add("LightBenderAlignment", lightBenderAlignment.ToString());
                                        if (lightBenderAlignment)
                                        {
                                            PlanValues.Add("FemurEuler", x.GetAttribute("FemurEuler"));
                                            PlanValues.Add("TibiaEuler", x.GetAttribute("TibiaEuler"));
                                            PlanValues.Add("PatellaEuler", x.GetAttribute("PatellaEuler"));
                                        }
                                    }
                                    PlanValues.Add("Delete", x.GetAttribute("Delete"));
                                }
                                catch
                                {
                                }
                                PlanValues.Add("StemImplantType", x.GetAttribute("StemImplantType"));
                                PlanValues.Add("HeadImplantType", x.GetAttribute("HeadImplantType"));
                                PlanValues.Add("CupImplantType", x.GetAttribute("CupImplantType"));
                                PlanValues.Add("LinerImplantType", x.GetAttribute("LinerImplantType"));

                                PlanValues.Add("PelvisCupVariant", x.GetAttribute("PelvisCupVariant"));
                                PlanValues.Add("PelvisLinerVariant", x.GetAttribute("PelvisLinerVariant"));
                                PlanValues.Add("FemurHeadVariant", x.GetAttribute("FemurHeadVariant"));
                                PlanValues.Add("FemurStemVariant", x.GetAttribute("FemurStemVariant"));
                                PlanValues.Add("PelvisCupSize", x.GetAttribute("PelvisCupSize"));
                                PlanValues.Add("PelvisLinerSize", x.GetAttribute("PelvisLinerSize"));
                                PlanValues.Add("FemurHeadSize", x.GetAttribute("FemurHeadSize"));
                                PlanValues.Add("FemurStemSize", x.GetAttribute("FemurStemSize"));

                                PlanValues.Add("PelvisAP", x.GetAttribute("PelvisAP"));
                                PlanValues.Add("PelvisAR", x.GetAttribute("PelvisAR"));
                                PlanValues.Add("PelvisML", x.GetAttribute("PelvisML"));
                                PlanValues.Add("PelvisInclination", x.GetAttribute("PelvisInclination"));
                                PlanValues.Add("PelvisSpin", x.GetAttribute("PelvisSpin"));
                                PlanValues.Add("PelvisSI", x.GetAttribute("PelvisSI"));
                                PlanValues.Add("FemurAR", x.GetAttribute("FemurAR"));
                                PlanValues.Add("FemurLL", x.GetAttribute("FemurLL"));
                                if (!string.IsNullOrEmpty(x.GetAttribute("FemurRO")))
                                {
                                    PlanValues.Add("FemurRO", x.GetAttribute("FemurRO"));
                                }
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "Cases") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "FunctionalValues")
                    {
                        FunctionalValues.Clear();

                        while (!x.EOF)
                        {
                            Measurement measurement = new Measurement();

                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Measurement")
                            {
                                var id = int.Parse(x.GetAttribute("ID"));
                                var measure = x.GetAttribute("Measure");
                                var measureVal = float.Parse(x.GetAttribute("MeasureValue"));

                                FunctionalValues.Add(new Measurement()
                                {
                                    IdFunctionalValues = id,
                                    Measure = measure,
                                    MeasureValue = measureVal
                                });
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "FunctionalValues") break;
                        }
                    }
                    if (!x.Read()) break;
                }
                x.Close();
            }
            System.IO.File.Delete(fp);
            return true;
        }

        internal bool DecryptTheProject(byte[] source, out string filePathOut)
        {
            filePathOut = System.IO.Path.GetTempFileName();
            DataEncryption encryptionFile = new DataEncryption();
            bool chk = encryptionFile.DecryptEncryptFile(source, filePathOut);

            return chk;
        }

        #endregion
    }
}
