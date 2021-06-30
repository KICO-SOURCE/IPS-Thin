using System.Xml;

namespace Assets.CaseFile
{
    public class TimeStep
    {
        private double _time;
        internal double Time { get { return _time; } set { _time = value; } }

        private double _tibfem3131;
        internal double TibFem3131 { get { return _tibfem3131; } set { _tibfem3131 = value; } }

        private double _tibfem3132;
        internal double TibFem3132 { get { return _tibfem3132; } set { _tibfem3132 = value; } }

        private double _tibfem3133;
        internal double TibFem3133 { get { return _tibfem3133; } set { _tibfem3133 = value; } }

        private double _tibfemX;
        internal double TibFemX { get { return _tibfemX; } set { _tibfemX = value; } }

        private double _tibfemY;
        internal double TibFemY { get { return _tibfemY; } set { _tibfemY = value; } }

        private double _tibfemZ;
        internal double TibFemZ { get { return _tibfemZ; } set { _tibfemZ = value; } }

        private double _tibpat3131;
        internal double Tibpat3131 { get { return _tibpat3131; } set { _tibpat3131 = value; } }

        private double _tibpat3132;
        internal double Tibpat3132 { get { return _tibpat3132; } set { _tibpat3132 = value; } }

        private double _tibpat3133;
        internal double Tibpat3133 { get { return _tibpat3133; } set { _tibpat3133 = value; } }

        private double _tibpatX;
        internal double TibpatX { get { return _tibpatX; } set { _tibpatX = value; } }

        private double _tibpatY;
        internal double TibpatY { get { return _tibpatY; } set { _tibpatY = value; } }

        private double _tibpatZ;
        internal double TibpatZ { get { return _tibpatZ; } set { _tibpatZ = value; } }


        internal void WriteXml(XmlTextWriter x)
        {
            x.WriteAttributeString("Time", _time.ToString());
            x.WriteAttributeString("TibFem313_1", _tibfem3131.ToString());
            x.WriteAttributeString("TibFem313_2", _tibfem3132.ToString());
            x.WriteAttributeString("TibFem313_3", _tibfem3133.ToString());
            x.WriteAttributeString("TibFemX", _tibfemX.ToString());
            x.WriteAttributeString("TibFemY", _tibfemY.ToString());
            x.WriteAttributeString("TibFemZ", _tibfemZ.ToString());

            x.WriteAttributeString("TibPat313_1", _tibpat3131.ToString());
            x.WriteAttributeString("TibPat313_2", _tibpat3132.ToString());
            x.WriteAttributeString("TibPat313_3", _tibpat3133.ToString());
            x.WriteAttributeString("TibPatX", _tibpatX.ToString());
            x.WriteAttributeString("TibPatY", _tibpatY.ToString());
            x.WriteAttributeString("TibPatZ", _tibpatZ.ToString());
        }

        internal void ReadXml(XmlTextReader x)
        {
            double.TryParse(x.GetAttribute("Time"), out _time);
            double.TryParse(x.GetAttribute("TibFem313_1"), out _tibfem3131);
            double.TryParse(x.GetAttribute("TibFem313_2"), out _tibfem3132);
            double.TryParse(x.GetAttribute("TibFem313_3"), out _tibfem3133);
            double.TryParse(x.GetAttribute("TibFemX"), out _tibfemX);
            double.TryParse(x.GetAttribute("TibFemY"), out _tibfemY);
            double.TryParse(x.GetAttribute("TibFemZ"), out _tibfemZ);
            double.TryParse(x.GetAttribute("TibPat313_1"), out _tibpat3131);
            double.TryParse(x.GetAttribute("TibPat313_2"), out _tibpat3132);
            double.TryParse(x.GetAttribute("TibPat313_3"), out _tibpat3133);
            double.TryParse(x.GetAttribute("TibPatX"), out _tibpatX);
            double.TryParse(x.GetAttribute("TibPatY"), out _tibpatY);
            double.TryParse(x.GetAttribute("TibPatZ"), out _tibpatZ);
          
        }
    }
}