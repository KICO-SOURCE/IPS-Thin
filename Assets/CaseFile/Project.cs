using Assets.CaseFile.DataConnector;
using Assets.CaseFile.Enums;
using Assets.CaseFile.Models;
using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Assets.CaseFile
{
    internal class Project : BaseProject
    {
        #region Fields

        public Patient PatientData;
        public Dictionary<string, Mesh> MeshGeoms { get; set; }
        public Dictionary<string, Vector3> LandMarks { get; set; }
        public Dictionary<string, string> PlanValues { get; set; }
        public Dictionary<string, string> TibiaTrayParameters { get; set; }
        public Dictionary<string, string> FemoralComponentParameters { get; set; }
        public Dictionary<string, string> PatellaButtonParameter { get; set; }
        public List<AnatomicalMeasurements> FunctionalValues { get; set; }
        public List<CustomImageModel> CustomImageModels { get; set; }
        public Dictionary<string, List<string>> PostOpREFTransform { get; set; }
        public Dictionary<string, AlignmentPresets> AlignmentPresets { get; set; }
        public Dictionary<string, List<SensitivityResults>> SensitivityResults { get; private set; }
        public List<TimeStep> SimualtionTimeSteps { get; protected set; }
        public Dictionary<string, string> TempVersion { get; set; }
        public List<DKSResultsHolder> DKSResults { get; protected set; }
        public Dictionary<string, Mesh> ExtraStls { get; set; }
        public List<KeyValuePair<string, Tuple<string, Mesh>>> ExtraStlDetails { get; set; }

        #endregion

        #region Private Methods

        private void InitializeData()
        {
            PatientData = new Patient();
            MeshGeoms = new Dictionary<string, Mesh>();
            LandMarks = new Dictionary<string, Vector3>();
            PlanValues = new Dictionary<string, string>();
            TibiaTrayParameters = new Dictionary<string, string>();
            FemoralComponentParameters = new Dictionary<string, string>();
            PatellaButtonParameter = new Dictionary<string, string>();
            FunctionalValues = new List<AnatomicalMeasurements>();
            CustomImageModels = new List<CustomImageModel>();
            PostOpREFTransform = new Dictionary<string, List<string>>();
            AlignmentPresets = new Dictionary<string, AlignmentPresets>();
            SensitivityResults = new Dictionary<string, List<SensitivityResults>>();
            TempVersion = new Dictionary<string, string>();
            ExtraStls = new Dictionary<string, Mesh>();
            ExtraStlDetails = new List<KeyValuePair<string, Tuple<string, Mesh>>>();
        }

        private void LoadLandmarks(XmlTextReader x)
        {
            string type = x.GetAttribute("ID");
            var x1 = float.Parse(x.GetAttribute("X"));
            var y = float.Parse(x.GetAttribute("Y"));
            var z = float.Parse(x.GetAttribute("Z"));

            LandMarks.Add(type, new Vector3(x1, y, z));
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
                    if (x.MoveToContent() == XmlNodeType.Element && x.Name == "KICOCASE")
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
                        PatientData.Leftright = (Leftright)Enum.Parse(typeof(Leftright), x.GetAttribute("Leg"));
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
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "MeshDistalFemur" || x.Name == "MeshProximalFemur"
                             || x.Name == "MeshDistalTibia" || x.Name == "MeshProximalTibia" || x.Name == "MeshPatella" || x.Name == "GeomFemurLeft"
                             || x.Name == "GeomFemurRight" || x.Name == "GeomTibiaLeft" || x.Name == "GeomTibiaRight" || x.Name == "GeomPelvis"
                             || x.Name == "GeomFemurCorticalLeft" || x.Name == "GeomFemurCorticalRight" || x.Name == "GeomFemurDecimatedLeft"
                             || x.Name == "GeomFemurDecimatedRight" || x.Name == "GeomPelvisDecimated" || x.Name == "TibiaCancellous"
                             || x.Name == "FemoralGuideRefModel" || x.Name == "TibiaGuideRefModel" || x.Name == "FemurTagFaces"
                             || x.Name == "FemurScanData" || x.Name == "FemurMeshDecimated" || x.Name == "TibiaScanData"
                             || x.Name == "FemurConfidenceMesh" || x.Name == "FemurNonConfidenceMesh" || x.Name == "FemurTrochMesh"))
                    {
                        MeshGeoms.Add(x.Name, MeshGeometryFunctions.GetXmlMeshGeometry3D(x));
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "FemurLandMarks" ||
                             x.Name == "FemurLandMarks" || x.Name == "PatellaLandMarks" || x.Name == "FemurLeftLandmarks" ||
                             x.Name == "FemurRightLandMarks" || x.Name == "TibiaLeftLandMarks" || x.Name == "TibiaRightLandMarks" ||
                             x.Name == "PelvisLandmarks"))
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "LandMark")
                            {
                                LoadLandmarks(x);
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "FemurLandMarks") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "KicoCASEfiles")
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

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "KicoCASEfiles") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "TibiaTrayParameters")
                    {
                        TibiaTrayParameters.Add("TibiaSlope", x.GetAttribute("TibiaSlope"));
                        TibiaTrayParameters.Add("TibiaInternalRotatio", x.GetAttribute("TibiaInternalRotation"));
                        TibiaTrayParameters.Add("TibiaVarusValgus", x.GetAttribute("TibiaVarusValgus"));
                        TibiaTrayParameters.Add("TibiaAnteriorPosterior", x.GetAttribute("TibiaAnteriorPosterior"));
                        TibiaTrayParameters.Add("TibiaProximalResection", x.GetAttribute("TibiaProximalResection"));
                        TibiaTrayParameters.Add("TibiaMedialLateral", x.GetAttribute("TibiaMedialLateral"));

                        TibiaTrayParameters.Add("InitialTibiaVariant", x.GetAttribute("TibiaComponentVariant"));
                        TibiaTrayParameters.Add("InitialTibiaSize", x.GetAttribute("TibiaComponentSize"));
                        TibiaTrayParameters.Add("InitialTibiaInsertVariant", x.GetAttribute("TibiaInsertVariant"));
                        TibiaTrayParameters.Add("InitialTibiaInsertSize", x.GetAttribute("TibiaInsertSize"));
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "FemoralComponentParameters")
                    {
                        FemoralComponentParameters.Add("FemurVarusValgusAdjustment", x.GetAttribute("FemurVarusValgusAdjustment"));
                        FemoralComponentParameters.Add("FemurInternalRotationAdjustment", x.GetAttribute("FemurInternalRotationAdjustment"));
                        FemoralComponentParameters.Add("FemurFlexionExtensionAdjustment", x.GetAttribute("FemurFlexionExtensionAdjustment"));
                        FemoralComponentParameters.Add("FemurPosteriorResectionAdjustment", x.GetAttribute("FemurPosteriorResectionAdjustment"));
                        FemoralComponentParameters.Add("FemurDistalResectionAdjustment", x.GetAttribute("FemurDistalResectionAdjustment"));
                        FemoralComponentParameters.Add("FemurMedialLateralAdjustment", x.GetAttribute("FemurMedialLateralAdjustment"));

                        FemoralComponentParameters.Add("InitialFemurVariant", x.GetAttribute("FemoralComponentVariant"));
                        FemoralComponentParameters.Add("InitialFemurSize", x.GetAttribute("FemoralComponentSize"));
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "PatellaButtonParameter")
                    {
                        PatellaButtonParameter.Add("PatellaFlexionExtension", x.GetAttribute("PatellaFlexionExtension"));
                        PatellaButtonParameter.Add("PatellaInternalRotation", x.GetAttribute("PatellaInternalRotation"));
                        PatellaButtonParameter.Add("PatellaSuperiorInferior", x.GetAttribute("PatellaSuperiorInferior"));
                        PatellaButtonParameter.Add("PatellaAnteriorPosterior", x.GetAttribute("PatellaAnteriorPosterior"));
                        PatellaButtonParameter.Add("PatellaMedialLateral", x.GetAttribute("PatellaMedialLateral"));
                        PatellaButtonParameter.Add("PatellaInternalExternal", x.GetAttribute("PatellaInternalExternal"));

                        PatellaButtonParameter.Add("InitialPatellaVariant", x.GetAttribute("PatellaComponentVariant"));
                        PatellaButtonParameter.Add("InitialPatellaSize", x.GetAttribute("PatellaComponentSize"));
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "FunctionalValues")
                    {
                        FunctionalValues.Clear();

                        while (!x.EOF)
                        {
                            AnatomicalMeasurements measurement = new AnatomicalMeasurements();

                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Measurement")
                            {
                                measurement.ReadXml(x);
                                FunctionalValues.Add(measurement);
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "FunctionalValues") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "CustomPanels")
                    {
                        string title = "";
                        string description = "";
                        string image = "";
                        string displayMode = "";

                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "CustomPanel")
                            {
                                title = x.GetAttribute("Title");
                                description = x.GetAttribute("Description");
                                image = x.GetAttribute("Image");
                                var index = x.GetAttribute("CaseHistoryIndex");
                                displayMode = x.GetAttribute("DisplayMode");
                                if (!x.Read()) break;

                                CustomImageModel model = new CustomImageModel();
                                model.Name = title;
                                model.Description = description;
                                byte[] imageArray = Convert.FromBase64String(image);
                                model.ReferenceImage = imageArray;
                                if (!string.IsNullOrEmpty(index))
                                {
                                    model.AttachedAlignment = int.Parse(index);
                                }
                                else
                                {
                                    model.AttachedAlignment = -1;
                                }

                                Enum.TryParse(displayMode, out DisplayModes displayModes);
                                model.DisplayMode = displayModes;

                                CustomImageModels.Add(model);
                            }
                            if (x.IsEmptyElement) break;
                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "CustomPanels") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "FemurPostOpREFTransform"
                        || x.Name == "TibiaPostOpREFTransform" || x.Name == "PatellaPostOpREFTransform"))
                    {
                        var postOpREFTransString = new List<string>();
                        postOpREFTransString.Add(x.GetAttribute("TransName"));
                        postOpREFTransString.Add(x.GetAttribute("Tx"));
                        postOpREFTransString.Add(x.GetAttribute("Ty"));
                        postOpREFTransString.Add(x.GetAttribute("Tz"));
                        postOpREFTransString.Add(x.GetAttribute("Ry"));
                        postOpREFTransString.Add(x.GetAttribute("Rx"));
                        postOpREFTransString.Add(x.GetAttribute("Rz"));
                        postOpREFTransString.Add(x.GetAttribute("Order"));
                        PostOpREFTransform.Add(x.Name, postOpREFTransString);
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "PostOpInfo")
                    {
                        PatientData.PostOpCaseCode = x.GetAttribute("PostOpCaseCode");
                        var date = x.GetAttribute("PostOpDateOfScan");
                        if (date != null)
                        {
                            PatientData.PostOpDateOfScan = ParseDate(date);
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "AlignmentMechanicalAxis")
                    {
                        var straightMa = new AlignmentPresets();
                        straightMa.ReadXml(x);
                        AlignmentPresets.Add(x.Name, straightMa);
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "AlignmentNatural")
                    {
                        var naturalAlignmnet = new AlignmentPresets();
                        naturalAlignmnet.ReadXml(x);
                        AlignmentPresets.Add(x.Name, naturalAlignmnet);
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "AlignmentConstitutionVV")
                    {
                        var constitutionalVv = new AlignmentPresets();
                        constitutionalVv.ReadXml(x);
                        AlignmentPresets.Add(x.Name, constitutionalVv);
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && (x.Name == "CongruentPCLRetainedResurfaced"
                             || x.Name == "CongruentPCLRemovedResurfaced" || x.Name == "UltraPCLRetainedResurfaced"
                             || x.Name == "UltraPCLRemovedResurfaced" || x.Name == "CongruentPCLRetainedNative"
                             || x.Name == "CongruentPCLRemovedNative" || x.Name == "UltraPCLRetainedNative"
                             || x.Name == "UltraPCLRemovedNative"))
                    {
                        int count = 0;
                        string name = x.Name;
                        List<SensitivityResults> results = new List<SensitivityResults>();
                        while (!x.EOF)
                        {
                            SensitivityResults sr = new SensitivityResults();
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "DKS")
                            {
                                sr.ReadXml(x);
                                results.Add(sr);
                                count++;
                            }
                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "CongruentPCLRetainedResurfaced") break;
                        }
                        SensitivityResults.Add(name, results);
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "SimlationTimeSteps")
                    {
                        int count = 0;
                        SimualtionTimeSteps = new List<TimeStep>();

                        while (!x.EOF)
                        {
                            TimeStep sr = new TimeStep();
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "TimeStep")
                            {
                                sr.ReadXml(x);
                                SimualtionTimeSteps.Add(sr);
                                count++;
                            }
                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "SimlationTimeSteps") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "TempVersion")
                    {
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Case")
                            {
                                string implantTypeStr = x.GetAttribute("ImplantType");                               
                                TempVersion.Add("FemurSizeString", x.GetAttribute("FemurImplant"));
                                TempVersion.Add("FemurVarientString", x.GetAttribute("FemurImplantVarient"));
                                TempVersion.Add("FemVv", x.GetAttribute("FemurVV"));
                                TempVersion.Add("FemFlex", x.GetAttribute("FemurFE"));
                                TempVersion.Add("FemIe", x.GetAttribute("FemurIE"));
                                TempVersion.Add("FemMl", x.GetAttribute("FemurML"));
                                TempVersion.Add("FemAp", x.GetAttribute("FemurAP"));
                                TempVersion.Add("FemSi", x.GetAttribute("FemurSI"));
                                TempVersion.Add("TibiaInsertSizeString", x.GetAttribute("TibiaInsert"));
                                TempVersion.Add("TibiaInsertVarientString", x.GetAttribute("TibiaInsertVarient"));
                                TempVersion.Add("TibiaVarientString", x.GetAttribute("TibiaImplant"));
                                TempVersion.Add("TibiaSizeString", x.GetAttribute("TibiaImplantSize"));
                                TempVersion.Add("TibVv", x.GetAttribute("TibiaVV"));
                                TempVersion.Add("TibFlex", x.GetAttribute("TibiaFE"));
                                TempVersion.Add("TibIe", x.GetAttribute("TibiaIE"));
                                TempVersion.Add("TibMl", x.GetAttribute("TibiaML"));
                                TempVersion.Add("TibAp", x.GetAttribute("TibiaAP"));
                                TempVersion.Add("TibSi", x.GetAttribute("TibiaSI"));
                                TempVersion.Add("PatellaSizeString", x.GetAttribute("PatellaImplant"));
                                TempVersion.Add("PatellaVarientString", x.GetAttribute("PatellaImplantVariant"));
                                TempVersion.Add("PatellaFE", x.GetAttribute("PatellaFE"));
                                TempVersion.Add("PatellaIE", x.GetAttribute("PatellaIE"));
                                string patellaSpin = x.GetAttribute("PatellaSpin");
                                TempVersion.Add("PatellaSpin", string.IsNullOrEmpty(patellaSpin) ? "0" : patellaSpin);
                                TempVersion.Add("PatellaML", x.GetAttribute("PatellaML"));
                                TempVersion.Add("PatellaAP", x.GetAttribute("PatellaAP"));
                                TempVersion.Add("PatellaSI", x.GetAttribute("PatellaSI"));
                                TempVersion.Add("Notes", x.GetAttribute("CaseNotes"));
                                TempVersion.Add("SurgeonsNotes", x.GetAttribute("SurgeonsNotes"));
                                TempVersion.Add("AlignmentLabel", x.GetAttribute("AlignmentLabel"));

                                try
                                {
                                    TempVersion.Add("KicoCaseId", x.GetAttribute("KicoCaseID"));
                                    TempVersion.Add("CaseHistoryId", x.GetAttribute("CaseHistoryID"));
                                    TempVersion.Add("AlignmentType", x.GetAttribute("AlignmentType"));
                                    TempVersion.Add("EngineerID", x.GetAttribute("Engineer"));

                                    TempVersion.Add("Approve", x.GetAttribute("Approve"));
                                    TempVersion.Add("ChangeRequest", x.GetAttribute("ChangeRequest"));
                                    TempVersion.Add("SurgeonAlignmentID ", x.GetAttribute("SurgeonAlignmentID"));
                                    TempVersion.Add("AnalysisType", x.GetAttribute("AnalysisType"));
                                    if (!string.IsNullOrEmpty(x.GetAttribute("LightBenderAlignment")))
                                    {
                                        var lightBenderAlignment = bool.Parse(x.GetAttribute("LightBenderAlignment"));
                                        if (lightBenderAlignment)
                                        {
                                            TempVersion.Add("FemurEuler",  x.GetAttribute("FemurEuler"));
                                            TempVersion.Add("TibiaEuler", x.GetAttribute("TibiaEuler"));
                                            TempVersion.Add("PatellaEuler", x.GetAttribute("PatellaEuler"));
                                        }
                                    }
                                }
                                catch
                                {
                                    // Log the error details.
                                }
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "TempVersion") break;
                        }
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "DKSResults")
                    {
                        List<DKSResultsHolder> ch = new List<DKSResultsHolder>();
                        while (!x.EOF)
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "StartDKSRecord")
                            {
                                DKSResultsHolder Temp = new DKSResultsHolder();
                                Temp.RawResultsFromDB = new DatabaseDKS(x);
                                ch.Add(Temp);
                            }

                            if (!x.Read() || x.MoveToContent() == XmlNodeType.EndElement && x.Name == "DKSResults") break;
                        }
                        if (ch != null && ch.Count > 0) DKSResults = ch;
                    }
                    else if (x.MoveToContent() == XmlNodeType.Element && x.Name == "ExtraStls")
                    {
                        string stlType = "";
                        string bone = "";
                        Mesh mesh = null;

                        if (!x.Read()) break;

                        while (!x.EOF && x.MoveToContent() == XmlNodeType.Element && x.Name == "ExtraStlsImport")
                        {
                            if (x.MoveToContent() == XmlNodeType.Element && x.Name == "ExtraStlsImport")
                            {
                                bone = x.GetAttribute("Bone");
                                stlType = x.GetAttribute("StlType");

                                if (!x.Read()) break;

                                if (x.MoveToContent() == XmlNodeType.Element && x.Name == "Mesh")
                                {
                                    mesh = MeshGeometryFunctions.GetXmlMeshGeometry3D(x);
                                }

                                if (bone == "Femur" && stlType == "Other")
                                {
                                    ExtraStls.Add(bone, mesh);
                                }
                                ExtraStlDetails.Add(new KeyValuePair<string, Tuple<string, Mesh>>(stlType,
                                                        new Tuple<string, Mesh>(bone, mesh)));
                                if (!x.Read()) break;
                                if (!x.Read()) break;
                            }
                        }
                    }

                    if (!x.Read()) break;
                }
                x.Close();
            }
            System.IO.File.Delete(fp);
            return true;
        }

        #endregion
    }
}
