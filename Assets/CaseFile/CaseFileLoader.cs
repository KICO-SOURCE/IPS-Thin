using Assets.CaseFile.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;

namespace Assets.CaseFile
{
    internal class CaseFileLoader
    {
        #region Fields

        private static readonly Lazy<CaseFileLoader> _instance = new Lazy<CaseFileLoader>(() => new CaseFileLoader());

        private readonly ComponentLoader m_ComponentLoader;
        private readonly Project m_Project;
        private readonly Patient m_Patient;
        private Action m_LoadCompleted;

        #endregion

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static CaseFileLoader Instance
        {
            get { return _instance.Value; }
        }

        #region Private Methods

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

        private CaseFileLoader()
        {
            m_ComponentLoader = ComponentLoader.Instance;
            m_Project = Project.Instance;
            m_Patient = Patient.Instance;
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

            string implantType = ImplantType.Apex.ToString();
            using (XmlTextReader x = new XmlTextReader(filePath))
            {
                x.Read();

                while (!x.EOF)
                {
                    if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "CaseFile" || x.Name == "KICOCASE"))
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
                            implantType = x.GetAttribute("ImplantType");
                        }
                        catch
                        {
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
                            m_Patient.Hospital = x.GetAttribute("Hospital");
                        }
                        catch
                        {
                            m_Patient.Hospital = "";
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
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "MeshDistalFemur" || x.Name == "MeshProximalFemur"
                             || x.Name == "MeshDistalTibia" || x.Name == "MeshProximalTibia" || x.Name == "MeshPatella" || x.Name == "GeomFemurLeft"
                             || x.Name == "GeomFemurRight" || x.Name == "GeomTibiaLeft" || x.Name == "GeomTibiaRight" || x.Name == "GeomPelvis"
                             || x.Name == "GeomFemurCorticalLeft" || x.Name == "GeomFemurCorticalRight" || x.Name == "GeomFemurDecimatedLeft"
                             || x.Name == "GeomFemurDecimatedRight" || x.Name == "GeomPelvisDecimated" || x.Name == "TibiaCancellous"
                             || x.Name == "FemoralGuideRefModel" || x.Name == "TibiaGuideRefModel" || x.Name == "FemurTagFaces"
                             || x.Name == "FemurScanData" || x.Name == "FemurMeshDecimated" || x.Name == "TibiaScanData"
                             || x.Name == "FemurConfidenceMesh" || x.Name == "FemurNonConfidenceMesh" || x.Name == "FemurTrochMesh"))
                    {
                        m_Patient.MeshGeoms.Add(x.Name.Replace("Mesh", ""), MeshGeometryFunctions.GetXmlMeshGeometry3D(x));
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "Landmarks" || x.Name == "FemurLandMarks" ||
                             x.Name == "TibiaLandMarks" || x.Name == "PatellaLandMarks" || x.Name == "FemurLeftLandmarks" ||
                             x.Name == "FemurRightLandmarks" || x.Name == "TibiaLeftLandmarks" || x.Name == "TibiaRightLandmarks" ||
                             x.Name == "PelvisLandmarks"))
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "Landmark" || x.Name == "LandMark"))
                            {
                                var lm = new Landmark();
                                lm.ReadLandmark(x);
                                m_Patient.Landmarks.Add(lm);
                            }

                        if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && (x.Name == "Landmarks" ||
                                x.Name == "FemurLandMarks" || x.Name == "TibiaLandMarks" || x.Name == "PatellaLandMarks" ||
                                x.Name == "FemurLeftLandmarks" || x.Name == "FemurRightLandmarks" || x.Name == "TibiaLeftLandmarks" ||
                                x.Name == "TibiaRightLandmarks" || x.Name == "PelvisLandmarks")) break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "Cases" ||
                        x.Name == "KicoCASEfiles" || x.Name == "TempVersion"))
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Case")
                            {
                                var plan = new Dictionary<string, string>();
                                string implantTypeStr = x.GetAttribute("ImplantType");
                                if(int.TryParse(implantTypeStr, out int implant))
                                {
                                    var type = (ImplantType)(implant -1);
                                    implantTypeStr = type.ToString();
                                }
                                else
                                {
                                    implantTypeStr = implantType;
                                }
                                plan.Add("Brand", implantTypeStr);
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
                                    
                                    plan.Add("FemurEuler", x.GetAttribute("FemurEuler"));
                                    plan.Add("TibiaEuler", x.GetAttribute("TibiaEuler"));
                                    plan.Add("PatellaEuler", x.GetAttribute("PatellaEuler"));

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
                                plan.Add("CupEuler", x.GetAttribute("CupPositionalDataString"));
                                plan.Add("StemEuler", x.GetAttribute("StemPositionalDataString"));
                                plan.Add("FemurHeadEuler", x.GetAttribute("FemurPositionalDataString"));
                                m_Project.PlanValues.Add(plan);
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement &&
                                (x.Name == "Cases" || x.Name == "KicoCASEfiles" || x.Name == "TempVersion")) break;
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
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "PostOpInfo")
                    {
                        m_Patient.PostOpCaseCode = x.GetAttribute("PostOpCaseCode");
                        var date = x.GetAttribute("PostOpDateOfScan");
                        if (date != null)
                        {
                            m_Patient.PostOpDateOfScan = ParseDate(date);
                        }
                    }

                    if (!x.Read()) break;
                }
                x.Close();
            }
            LoadPositionalData();
            m_ComponentLoader.LoadLibrary();
            return true;
        }

        /// <summary>
        /// Load component data
        /// </summary>
        /// <param name="loadCompleted"></param>
        /// <param name="index"></param>
        public void LoadComponentData(Action loadCompleted, int index = 0)
        {
            m_LoadCompleted = loadCompleted;

            if(m_Project.PlanValues == null ||
                m_Project.PlanValues.Count - 1 < index)
            {
                m_LoadCompleted?.Invoke();
                return;
            }

            var plan = m_Project.PlanValues[index];

            if (plan != null)
            {
                if (plan.ContainsKey("FemurSizeString") &&
                    !string.IsNullOrEmpty(plan["FemurSizeString"]))
                {
                    m_ComponentLoader.LoadDatFile(index, ComponentType.Femur, plan["Brand"],
                        m_Patient.Leftright, plan["FemurVarientString"],
                        plan["FemurSizeString"], OnComponentLoaded);
                }
                if (plan.ContainsKey("TibiaInsertSizeString") &&
                    !string.IsNullOrEmpty(plan["TibiaInsertSizeString"]))
                {
                    m_ComponentLoader.LoadDatFile(index, ComponentType.TibiaInsert, plan["Brand"],
                        m_Patient.Leftright, plan["TibiaInsertVarientString"],
                        plan["TibiaInsertSizeString"], OnComponentLoaded);
                }
                if (plan.ContainsKey("TibiaSizeString") &&
                    !string.IsNullOrEmpty(plan["TibiaSizeString"]))
                {
                    m_ComponentLoader.LoadDatFile(index, ComponentType.TibiaTray, plan["Brand"],
                        m_Patient.Leftright, plan["TibiaVarientString"],
                        plan["TibiaSizeString"], OnComponentLoaded);
                }
                if (plan.ContainsKey("PatellaSizeString") &&
                    !string.IsNullOrEmpty(plan["PatellaSizeString"]))
                {
                    m_ComponentLoader.LoadDatFile(index, ComponentType.Patella, plan["Brand"],
                        m_Patient.Leftright, plan["PatellaVarientString"],
                        plan["PatellaSizeString"], OnComponentLoaded);
                }
                if (plan.ContainsKey("PelvisCupSize") &&
                    !string.IsNullOrEmpty(plan["PelvisCupSize"]))
                {
                    var side = char.ToUpper(m_Patient.Leftright[0]) + m_Patient.Leftright.Substring(1);
                    m_ComponentLoader.LoadDatFile(index, ComponentType.PelvisCup, plan["CupImplantType"],
                       side, plan["PelvisCupVariant"], plan["PelvisCupSize"], OnComponentLoaded);
                }
                if (plan.ContainsKey("PelvisLinerSize") &&
                    !string.IsNullOrEmpty(plan["PelvisLinerSize"]))
                {
                    var side = char.ToUpper(m_Patient.Leftright[0]) + m_Patient.Leftright.Substring(1);
                    m_ComponentLoader.LoadDatFile(index, ComponentType.PelvisLiner, plan["LinerImplantType"],
                       side, plan["PelvisLinerVariant"], plan["PelvisLinerSize"], OnComponentLoaded);
                }
                if (plan.ContainsKey("FemurHeadSize") &&
                    !string.IsNullOrEmpty(plan["FemurHeadSize"]))
                {
                    m_ComponentLoader.LoadDatFile(index, ComponentType.FemurHead, plan["HeadImplantType"],
                        string.Empty, plan["FemurHeadVariant"], plan["FemurHeadSize"], OnComponentLoaded);
                }
                if (plan.ContainsKey("FemurStemSize") &&
                    !string.IsNullOrEmpty(plan["FemurStemSize"]))
                {
                    var side = char.ToUpper(m_Patient.Leftright[0]) + m_Patient.Leftright.Substring(1);
                    m_ComponentLoader.LoadDatFile(index, ComponentType.FemurStem, plan["StemImplantType"],
                        side, plan["FemurStemVariant"], plan["FemurStemSize"], OnComponentLoaded);
                }
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Load positional data.
        /// </summary>
        private void LoadPositionalData()
        {
            foreach (var plan in m_Project.PlanValues)
            {
                var index = m_Project.PlanValues.IndexOf(plan);
                var dictionary = new Dictionary<string,PositionalData>();
                dictionary.Add("FemurEuler", new PositionalData(plan["FemurEuler"]));
                dictionary.Add("TibiaEuler", new PositionalData(plan["TibiaEuler"]));
                dictionary.Add("PatellaEuler", new PositionalData(plan["PatellaEuler"]));
                dictionary.Add("CupEuler", new PositionalData(plan["CupEuler"]));
                dictionary.Add("StemEuler", new PositionalData(plan["StemEuler"]));
                dictionary.Add("FemurHeadEuler", new PositionalData(plan["FemurHeadEuler"]));

                m_Project.PlanComponentPosition.Add(index,dictionary);
            }
        }

        private void OnComponentLoaded(int index, ComponentType type, Implant implant)
        {
            implant.Name = ComponentLoader.GetComponentName(type);
            if (!m_Project.PlanImplants.ContainsKey(index))
            {
                m_Project.PlanImplants.Add(index, new Dictionary<ComponentType, Implant>());
            }
            m_Project.PlanImplants[index].Remove(type);
            m_Project.PlanImplants[index].Add(type, implant);

            if(IsLoadCompleted(index))
            {
                m_LoadCompleted?.Invoke();
            }
        }

        private bool IsLoadCompleted(int index)
        {
            bool loadCompleted = true;
            var plan = m_Project.PlanValues[index];

            if (!m_Project.PlanImplants.ContainsKey(index))
            {
                loadCompleted = false;
            }
            else if (plan != null)
            {
                if (plan.ContainsKey("FemurSizeString") &&
                    !string.IsNullOrEmpty(plan["FemurSizeString"]) &&
                    !m_Project.PlanImplants[index].ContainsKey(ComponentType.Femur))
                {
                    loadCompleted = false;
                }
                if (plan.ContainsKey("TibiaInsertSizeString") &&
                    !string.IsNullOrEmpty(plan["TibiaInsertSizeString"]) &&
                    !m_Project.PlanImplants[index].ContainsKey(ComponentType.TibiaInsert))
                {
                    loadCompleted = false;
                }
                if (plan.ContainsKey("TibiaSizeString") &&
                    !string.IsNullOrEmpty(plan["TibiaSizeString"]) &&
                    !m_Project.PlanImplants[index].ContainsKey(ComponentType.TibiaTray))
                {
                    loadCompleted = false;
                }
                if (plan.ContainsKey("PatellaSizeString") &&
                    !string.IsNullOrEmpty(plan["PatellaSizeString"]) &&
                    !m_Project.PlanImplants[index].ContainsKey(ComponentType.Patella))
                {
                    loadCompleted = false;
                }
                if (plan.ContainsKey("PelvisCupSize") &&
                    !string.IsNullOrEmpty(plan["PelvisCupSize"]) &&
                    !m_Project.PlanImplants[index].ContainsKey(ComponentType.PelvisCup))
                {
                    loadCompleted = false;
                }
                if (plan.ContainsKey("PelvisLinerSize") &&
                    !string.IsNullOrEmpty(plan["PelvisLinerSize"]) &&
                    !m_Project.PlanImplants[index].ContainsKey(ComponentType.PelvisLiner))
                {
                    loadCompleted = false;
                }
                if (plan.ContainsKey("FemurHeadSize") &&
                    !string.IsNullOrEmpty(plan["FemurHeadSize"]) &&
                    !m_Project.PlanImplants[index].ContainsKey(ComponentType.FemurHead))
                {
                    loadCompleted = false;
                }
                if (plan.ContainsKey("FemurStemSize") &&
                    !string.IsNullOrEmpty(plan["FemurStemSize"]) &&
                    !m_Project.PlanImplants[index].ContainsKey(ComponentType.FemurStem))
                {
                    loadCompleted = false;
                }
            }

            return loadCompleted;
        }

        #endregion
    }
}
