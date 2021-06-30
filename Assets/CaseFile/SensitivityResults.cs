using System.Xml;

namespace Assets.CaseFile
{
    public class SensitivityResults
    {
        public bool IsCongruent { get; set; }
        public bool IsResurfacedPatella { get; set; }
        public bool IsPclRetained { get; set; }
        public int TibIEoffset { get; set; }
        public int TibFlexionoffset { get; set; }
        public int TibInsertSlope { get; set; }
        public double Dks { get; set; }


        public void  WriteXml(XmlTextWriter x)
        {
            x.WriteAttributeString("IsCongruent", IsCongruent.ToString());
            x.WriteAttributeString("TibiaIEOffset", TibIEoffset.ToString());
            x.WriteAttributeString("TibiaFlexionOffset", TibFlexionoffset.ToString());
            x.WriteAttributeString("TibiaSlope", TibInsertSlope.ToString());
            x.WriteAttributeString("IsResurfaced", IsResurfacedPatella.ToString());
            x.WriteAttributeString("IsPCLRetained", IsPclRetained.ToString());
            x.WriteAttributeString("DKS", Dks.ToString());
        }
        public void ReadXml(XmlTextReader x)
        {
            bool congruentPH, resurfacedPH, pclRetPH;
            int TibIEPH, tibflexPH, tibInsertPH;
            double dksPH;

            bool.TryParse(x.GetAttribute("IsCongruent"), out congruentPH);
            int.TryParse(x.GetAttribute("TibiaIEOffset"), out TibIEPH);
            int.TryParse(x.GetAttribute("TibiaFlexionOffset"), out tibflexPH);
            int.TryParse(x.GetAttribute("TibiaSlope"), out tibInsertPH);
            bool.TryParse(x.GetAttribute("IsResurfaced"), out resurfacedPH);
            bool.TryParse(x.GetAttribute("IsPCLRetained"), out pclRetPH);
            double.TryParse(x.GetAttribute("DKS"), out dksPH);

            IsCongruent = congruentPH;
            TibIEoffset = TibIEPH;
            TibFlexionoffset = tibflexPH;
            TibInsertSlope = tibInsertPH;
            IsResurfacedPatella = resurfacedPH;
            IsPclRetained = pclRetPH;
            Dks = dksPH;
        }
    }
}