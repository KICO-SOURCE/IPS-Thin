using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace Assets.CaseFile
{
    public class AlignmentPresets
    {
        public double FlexionTibia { get { return _flexiontibia; } set { _flexiontibia = value; } }
        private double _flexiontibia;

        public double InternalRotationTibia { get { return _internalrotationtibia; } set { _internalrotationtibia = value; } }
        private double _internalrotationtibia;

        public double VarusValgusTibia { get { return _varusvalgustibia; } set { _varusvalgustibia = value; } }
        private double _varusvalgustibia;

        public double FlexionFemur { get { return _flexionFemur; } set { _flexionFemur = value; } }
        private double _flexionFemur;

        public double InternalRotationFemur { get { return _internalrotationFemur; } set { _internalrotationFemur = value; } }
        private double _internalrotationFemur;

        public double VarusValgusFemur { get { return _varusvalgusFemur; } set { _varusvalgusFemur = value; } }
        private double _varusvalgusFemur;

        private double _resectiontibia;
        public double ResectionTibia { get { return _resectiontibia; } set { _resectiontibia = value; } }

        private double _resectionfemur;
        public double ResectionFemur { get { return _resectionfemur; } set { _resectionfemur = value; } }

        private double _aPfemur;
        public double ApFemur { get { return _aPfemur; } set { _aPfemur = value; } }

        private double _apTibia;
        public double ApTibia { get { return _apTibia; } set { _apTibia = value; } }

        private double _mlFemur;
        public double MlFemur { get { return _mlFemur; } set { _mlFemur = value; } }

        private double _mlTibia;
        public double MlTibia { get { return _mlTibia; } set { _mlTibia = value; } }

        private bool _isenabled;
        public bool ISEnabled { get { return _isenabled; } set { _isenabled = value; } }

        public AlignmentPresets()
        {

        }

        public AlignmentPresets(double flexTib, double flexFem, double vvTib, double vvFem, double rotTib, double rotFem, double resectionTib, double resectionFem, double apTib, double apFem, double mlTib, double mlFem)
        {
            _flexionFemur = flexFem;
            _flexiontibia = flexTib;
            _varusvalgustibia = vvTib;
            _varusvalgusFemur = vvFem;
            _internalrotationFemur = rotFem;
            _internalrotationtibia = rotTib;
            _resectionfemur = resectionFem;
            _resectiontibia = resectionTib;
            _aPfemur = apFem;
            _apTibia = apTib;
            _mlFemur = mlFem;
            _mlTibia = mlTib;
            _isenabled = true;
        }

        public void WriteXml(XmlTextWriter x)
        {
            x.WriteAttributeString("TibiaSlope", _flexiontibia.ToString());
            x.WriteAttributeString("TibiaInternalRotation", _internalrotationtibia.ToString());
            x.WriteAttributeString("TibiaVarusValgus", _varusvalgustibia.ToString());
            x.WriteAttributeString("FemurFlexion", _flexionFemur.ToString());
            x.WriteAttributeString("FemurInternalRotation", _internalrotationFemur.ToString());
            x.WriteAttributeString("FemurVarusValgus", _varusvalgusFemur.ToString());
            x.WriteAttributeString("ResectionFemur", _resectionfemur.ToString());
            x.WriteAttributeString("ResectionTibia", _resectiontibia.ToString());
            x.WriteAttributeString("APFemur", _aPfemur.ToString());
            x.WriteAttributeString("MLTranslation", _mlTibia.ToString());
            x.WriteAttributeString("APTibia", _apTibia.ToString());
            x.WriteAttributeString("MLTranslationFemur", _mlFemur.ToString());
            x.WriteAttributeString("IsEnabled", _isenabled.ToString());


        }

        public void ReadXml(XmlTextReader x)
        {
            double.TryParse(x.GetAttribute("TibiaSlope"), out _flexiontibia);
            double.TryParse(x.GetAttribute("TibiaInternalRotation"), out _internalrotationtibia);
            double.TryParse(x.GetAttribute("TibiaVarusValgus"), out _varusvalgustibia);
            double.TryParse(x.GetAttribute("FemurFlexion"), out _flexionFemur);
            double.TryParse(x.GetAttribute("FemurInternalRotation"), out _internalrotationFemur);
            double.TryParse(x.GetAttribute("FemurVarusValgus"), out _varusvalgusFemur);
            double.TryParse(x.GetAttribute("ResectionFemur"), out _resectionfemur);
            double.TryParse(x.GetAttribute("ResectionTibia"), out _resectiontibia);
            double.TryParse(x.GetAttribute("APFemur"), out _aPfemur);
            double.TryParse(x.GetAttribute("MLTranslation"), out _mlTibia);
            double.TryParse(x.GetAttribute("APTibia"), out _apTibia);
            double.TryParse(x.GetAttribute("MLTranslationFemur"), out _mlFemur);
            bool.TryParse(x.GetAttribute("IsEnabled"), out _isenabled);

        }

        public static AlignmentPresets ReadFromCsv(string fileName)
        {

            // Microsoft.Win32.OpenFileDialog openCsv = new Microsoft.Win32.OpenFileDialog();
            // openCsv.Filter = "CSV File (*.csv)|*.csv";

            // Nullable<bool> results = openCsv.ShowDialog();
            var results = File.Exists(fileName);

            AlignmentPresets outpreset = new AlignmentPresets();

            if (results == true)
            {

                var reader = new System.IO.StreamReader(fileName);//openCsv.FileName);
                List<string> lista = new List<string>();
                List<string> listb = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var value = line.Split(',');

                    if (value != null && value.Length == 2)
                    {
                        if (value[0] == "TibiaSlope")
                        {
                            double ft;
                            double.TryParse(value[1], out ft);
                            outpreset.FlexionTibia = ft;
                        }
                        else if (value[0] == "TibiaInternalRotation")
                        {
                            double tie;
                            double.TryParse(value[1], out tie);
                            outpreset.InternalRotationTibia = tie;
                        }
                        else if (value[0] == "TibiaVarusValgus")
                        {
                            double tvv;
                            double.TryParse(value[1], out tvv);
                            outpreset.VarusValgusTibia = tvv;
                        }
                        else if (value[0] == "FemurFlexion")
                        {
                            double ff;
                            double.TryParse(value[1], out ff);
                            outpreset.FlexionFemur = ff;
                        }
                        else if (value[0] == "FemurInternalRotation")
                        {
                            double fr;
                            double.TryParse(value[1], out fr);
                            outpreset.InternalRotationFemur = fr;
                        }
                        else if (value[0] == "FemurVarusValgus")
                        {
                            double fvv;
                            double.TryParse(value[1], out fvv);
                            outpreset.VarusValgusFemur = fvv;
                        }
                        else if (value[0] == "ResectionFemur")
                        {
                            double rf;
                            double.TryParse(value[1], out rf);
                            outpreset.ResectionFemur = rf;
                        }
                        else if (value[0] == "ResectionTibia")
                        {
                            double rt;
                            double.TryParse(value[1], out rt);
                            outpreset.ResectionTibia = rt;
                        }
                        else if (value[0] == "APFemur")
                        {
                            double ap;
                            double.TryParse(value[1], out ap);
                            outpreset.ApFemur = ap;
                        }
                        else if (value[0] == "MLTranslation")
                        {
                            double ml;
                            double.TryParse(value[1], out ml);
                            outpreset.MlTibia = ml;
                        }
                        else if (value[0] == "APTibia")
                        {
                            double ap;
                            double.TryParse(value[1], out ap);
                            outpreset.ApTibia = ap;
                        }
                        else if (value[0] == "MLTranslationFemur")
                        {
                            double ml;
                            double.TryParse(value[1], out ml);
                            outpreset.MlTibia = ml;
                        }
                    }
                }
            }

            outpreset.ISEnabled = true;

            return outpreset;
        }
    }
}