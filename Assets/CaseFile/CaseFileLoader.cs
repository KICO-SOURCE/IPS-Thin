using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using UnityEngine;

namespace Assets.CaseFile
{
    internal class CaseFileLoader
    {
        #region Fields

        private readonly Project m_Project;
        private readonly Patient m_Patient;

        #endregion

        #region Private Methods

        private void LoadLandmarks(XmlTextReader x)
        {
            string bone = x.GetAttribute("Bone");
            string type = x.GetAttribute("ID");
            var x1 = float.Parse(x.GetAttribute("X"));
            var y = float.Parse(x.GetAttribute("Y"));
            var z = float.Parse(x.GetAttribute("Z"));

            m_Patient.Landmarks.Add(new Landmark()
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

        private bool DecryptCaseFile(byte[] source, out string filePathOut)
        {
            filePathOut = System.IO.Path.GetTempFileName();
            DataEncryption encryptionFile = new DataEncryption();
            bool chk = encryptionFile.DecryptEncryptFile(source, filePathOut);

            return chk;
        }

        #endregion

        #region Constructors

        public CaseFileLoader(Project project,
                              Patient patient)
        {
            m_Project = project;
            m_Patient = patient;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load case file for knee and hip.
        /// </summary>
        /// <param name="sourceByte"></param>
        /// <returns></returns>
        internal bool LoadCaseFile(byte[] sourceByte)
        {
            string fp;
            bool chck = DecryptCaseFile(sourceByte, out fp);

            if (!chck)
            {
                Console.WriteLine("Unable to open file the file",
                    "Case file is corrupt please re-download the file from the 360 Server");
                System.IO.File.Delete(fp);
                return false;
            }

            var ret = LoadCaseFile(fp);
            System.IO.File.Delete(fp);
            return ret;
        }

        /// <summary>
        /// Load case file for knee and hip.
        /// Use this method only for not encrypted files
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal bool LoadCaseFile(string filePath)
        {
            m_Project.InitializeData();
            m_Patient.InitializeData();
            using (XmlTextReader x = new XmlTextReader(filePath))
            {
                x.Read();

                while (!x.EOF)
                {
                    if (x.MoveToContent() == XmlNodeType.Element && x.Name == "CaseFile")
                    {
                        try
                        {
                            m_Patient.PatientId = int.Parse(x.GetAttribute("PatientID"));
                        }
                        catch
                        {
                            m_Patient.PatientId = 0;
                        }
                        try
                        {
                            m_Patient.SurgeryId = int.Parse(x.GetAttribute("SurgeryID"));
                        }
                        catch
                        {
                            m_Patient.SurgeryId = 0;
                        }
                        m_Patient.PatientName = x.GetAttribute("PatientName");
                        m_Patient.PatientFirstName = x.GetAttribute("FirstName");
                        if (string.IsNullOrEmpty(m_Patient.PatientFirstName))
                        {
                            var name = m_Patient.PatientName.Split(' ');
                            m_Patient.PatientFirstName = name[0];
                            m_Patient.PatientLastName = m_Patient.PatientName.Substring(m_Patient.PatientFirstName.Length, m_Patient.PatientName.Length - m_Patient.PatientFirstName.Length).TrimStart();
                        }
                        else
                        {
                            m_Patient.PatientLastName = m_Patient.PatientName.Substring(m_Patient.PatientFirstName.Length, m_Patient.PatientName.Length - m_Patient.PatientFirstName.Length).TrimStart();
                        }
                        m_Patient.CaseNumber = x.GetAttribute("CaseNumber");
                        m_Patient.Leftright = x.GetAttribute("Leg");
                        m_Patient.SurgeonName = x.GetAttribute("Surgeon");

                        m_Patient.Dob = ModifyFormat(x.GetAttribute("DateOfBith"));
                        m_Patient.CreationDate = ParseDate(x.GetAttribute("CreationDate"));
                        m_Patient.SurgeryDate = ParseDate(x.GetAttribute("SurgeryDate"));
                        var dos = x.GetAttribute("DateOfScan");
                        if (!string.IsNullOrEmpty(dos))
                        {
                            m_Patient.DateOfScan = ParseDate(dos);
                        }

                        try
                        {
                            m_Patient.Mrn = x.GetAttribute("MRN");
                        }
                        catch
                        {
                            m_Patient.Mrn = "";
                        }

                        try
                        {
                            m_Patient.Gender = int.Parse(x.GetAttribute("Gender"));
                        }
                        catch
                        {
                            m_Patient.Gender = 0;
                        }


                        try
                        {
                            m_Patient.Hospital = int.Parse(x.GetAttribute("Hospital"));
                        }
                        catch
                        {
                            m_Patient.Hospital = 0;
                        }

                        try
                        {
                            m_Patient.SegmentationPath = x.GetAttribute("SegmentationPath");
                        }
                        catch
                        {
                            m_Patient.SegmentationPath = " ";
                        }

                        if (!x.Read()) break;
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "Meshes"))
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "MeshData")
                            {
                                string bone = x.GetAttribute("Bone");
                                m_Patient.MeshGeoms.Add(bone, MeshGeometryFunctions.GetXmlMeshGeometry3D(x));
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "Meshes") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "Landmarks"))
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Landmark")
                            {
                                LoadLandmarks(x);
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "Landmarks") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Cases")
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Case")
                            {
                                var plan = new Dictionary<string, string>();
                                string implantTypeStr = x.GetAttribute("ImplantType");
                                plan.Add("FemurSizeString", x.GetAttribute("FemurImplant"));
                                plan.Add("FemurVarientString", x.GetAttribute("FemurImplantVarient"));
                                plan.Add("FemVv", x.GetAttribute("FemurVV"));
                                plan.Add("FemFlex", x.GetAttribute("FemurFE"));
                                plan.Add("FemIe", x.GetAttribute("FemurIE"));
                                plan.Add("FemMl", x.GetAttribute("FemurML"));
                                plan.Add("FemAp", x.GetAttribute("FemurAP"));
                                plan.Add("FemSi", x.GetAttribute("FemurSI"));
                                plan.Add("TibiaInsertSizeString", x.GetAttribute("TibiaInsert"));
                                plan.Add("TibiaInsertVarientString", x.GetAttribute("TibiaInsertVarient"));
                                plan.Add("TibiaVarientString", x.GetAttribute("TibiaImplant"));
                                plan.Add("TibiaSizeString", x.GetAttribute("TibiaImplantSize"));
                                plan.Add("TibVv", x.GetAttribute("TibiaVV"));
                                plan.Add("TibFlex", x.GetAttribute("TibiaFE"));
                                plan.Add("TibIe", x.GetAttribute("TibiaIE"));
                                plan.Add("TibMl", x.GetAttribute("TibiaML"));
                                plan.Add("TibAp", x.GetAttribute("TibiaAP"));
                                plan.Add("TibSi", x.GetAttribute("TibiaSI"));
                                plan.Add("PatellaSizeString", x.GetAttribute("PatellaImplant"));
                                plan.Add("PatellaVarientString", x.GetAttribute("PatellaImplantVariant"));
                                plan.Add("PatellaFE", x.GetAttribute("PatellaFE"));
                                plan.Add("PatellaIE", x.GetAttribute("PatellaIE"));
                                string patellaSpin = x.GetAttribute("PatellaSpin");
                                plan.Add("PatellaSpin", string.IsNullOrEmpty(patellaSpin) ? "" : patellaSpin);
                                plan.Add("PatellaML", x.GetAttribute("PatellaML"));
                                plan.Add("PatellaAP", x.GetAttribute("PatellaAP"));
                                plan.Add("PatellaSI", x.GetAttribute("PatellaSI"));
                                plan.Add("Notes", x.GetAttribute("CaseNotes"));
                                plan.Add("SurgeonsNotes", x.GetAttribute("SurgeonsNotes"));
                                plan.Add("AlignmentLabel", x.GetAttribute("AlignmentLabel"));

                                try
                                {
                                    plan.Add("KicoCaseId", x.GetAttribute("KicoCaseID"));
                                    plan.Add("CaseHistoryId", x.GetAttribute("CaseHistoryID"));
                                    plan.Add("AlignmentType", x.GetAttribute("AlignmentType"));
                                    plan.Add("EngineerID", x.GetAttribute("Engineer"));

                                    plan.Add("Approve", x.GetAttribute("Approve"));
                                    plan.Add("ChangeRequest", x.GetAttribute("ChangeRequest"));
                                    plan.Add("SurgeonAlignmentID", (x.GetAttribute("SurgeonAlignmentID")));
                                    plan.Add("AnalysisType", x.GetAttribute("AnalysisType"));
                                    if (!string.IsNullOrEmpty(x.GetAttribute("LightBenderAlignment")))
                                    {
                                        var lightBenderAlignment = bool.Parse(x.GetAttribute("LightBenderAlignment"));
                                        plan.Add("LightBenderAlignment", lightBenderAlignment.ToString());
                                        if (lightBenderAlignment)
                                        {
                                            plan.Add("FemurEuler", x.GetAttribute("FemurEuler"));
                                            plan.Add("TibiaEuler", x.GetAttribute("TibiaEuler"));
                                            plan.Add("PatellaEuler", x.GetAttribute("PatellaEuler"));
                                        }
                                    }
                                    plan.Add("Delete", x.GetAttribute("Delete"));
                                }
                                catch
                                {
                                }
                                plan.Add("StemImplantType", x.GetAttribute("StemImplantType"));
                                plan.Add("HeadImplantType", x.GetAttribute("HeadImplantType"));
                                plan.Add("CupImplantType", x.GetAttribute("CupImplantType"));
                                plan.Add("LinerImplantType", x.GetAttribute("LinerImplantType"));

                                plan.Add("PelvisCupVariant", x.GetAttribute("PelvisCupVariant"));
                                plan.Add("PelvisLinerVariant", x.GetAttribute("PelvisLinerVariant"));
                                plan.Add("FemurHeadVariant", x.GetAttribute("FemurHeadVariant"));
                                plan.Add("FemurStemVariant", x.GetAttribute("FemurStemVariant"));
                                plan.Add("PelvisCupSize", x.GetAttribute("PelvisCupSize"));
                                plan.Add("PelvisLinerSize", x.GetAttribute("PelvisLinerSize"));
                                plan.Add("FemurHeadSize", x.GetAttribute("FemurHeadSize"));
                                plan.Add("FemurStemSize", x.GetAttribute("FemurStemSize"));

                                plan.Add("PelvisAP", x.GetAttribute("PelvisAP"));
                                plan.Add("PelvisAR", x.GetAttribute("PelvisAR"));
                                plan.Add("PelvisML", x.GetAttribute("PelvisML"));
                                plan.Add("PelvisInclination", x.GetAttribute("PelvisInclination"));
                                plan.Add("PelvisSpin", x.GetAttribute("PelvisSpin"));
                                plan.Add("PelvisSI", x.GetAttribute("PelvisSI"));
                                plan.Add("FemurAR", x.GetAttribute("FemurAR"));
                                plan.Add("FemurLL", x.GetAttribute("FemurLL"));
                                if (!string.IsNullOrEmpty(x.GetAttribute("FemurRO")))
                                {
                                    plan.Add("FemurRO", x.GetAttribute("FemurRO"));
                                }
                                m_Project.PlanValues.Add(plan);
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "Cases") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "FunctionalValues")
                    {
                        m_Project.FunctionalValues.Clear();

                        while (!x.EOF)
                        {
                            Measurement measurement = new Measurement();

                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Measurement")
                            {
                                var id = int.Parse(x.GetAttribute("ID"));
                                var measure = x.GetAttribute("Measure");
                                var measureVal = float.Parse(x.GetAttribute("MeasureValue"));

                                m_Project.FunctionalValues.Add(new Measurement()
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

            return true;
        }

        #endregion
    }
}
