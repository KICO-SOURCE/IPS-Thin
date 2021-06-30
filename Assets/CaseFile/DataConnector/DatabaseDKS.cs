using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Xml;

namespace Assets.CaseFile.DataConnector
{
    public class DatabaseDKS
    {

        public int KicoCadDKSIndex { get; set; }
        public int CaseHistoryIndex { get; set; }
        public int DKSType { get; set; }
        public string DKSAlgorithm { get; set; }
        public string ModelType { get; set; }
        public string OptionName { get; set; }
        public List<string> DKSParams { get; set; }
        public List<double> DKSNumbers { get; set; }

        public DatabaseDKS()
        {
            DKSParams = new List<string>();
            DKSNumbers = new List<double>();
        }

        public DatabaseDKS(DataRow row)
        {
            DKSParams = new List<string>();
            DKSNumbers = new List<double>();

            KicoCadDKSIndex = (int)GetColumnData(row, "KicoCadDKSIndex", 0);
            CaseHistoryIndex = (int)GetColumnData(row, "CaseHistoryIndex", 0);
            DKSType = (int)GetColumnData(row, "DKSType", 0);
            DKSAlgorithm = (string)GetColumnData(row, "DKSAlgorithm", "");
            ModelType = (string)GetColumnData(row, "ModelType", "");
            OptionName = (string)GetColumnData(row, "OptionName", "");

            for (int i = 0; i < 20; i++)
            {
                DKSParams.Add((string)GetColumnData(row, "Param" + (i + 1), ""));
            }
            for (int i = 0; i < 100; i++)
            {
                DKSNumbers.Add((double)GetColumnData(row, "DKS" + (i + 1), 0));
            }
        }

        public DatabaseDKS(XmlReader xn)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            try
            {
                DKSParams = new List<string>();
                DKSNumbers = new List<double>();
                KicoCadDKSIndex = int.Parse(xn.GetAttribute("KicoCadDKSIndex"));
                CaseHistoryIndex = int.Parse(xn.GetAttribute("CaseHistoryIndex"));
                DKSType = int.Parse(xn.GetAttribute("DKSType"));
                DKSAlgorithm = xn.GetAttribute("DKSAlgorithm");
                ModelType = xn.GetAttribute("ModelType");
                OptionName = xn.GetAttribute("OptionName");
                for (int i = 0; i < 20; i++)
                {
                    DKSParams.Add(xn.GetAttribute("Param" + (i + 1)));
                }
                for (int i = 0; i < 100; i++)
                {
                    double value;
                    if (!double.TryParse(xn.GetAttribute("DKS" + (i + 1)), out value)) value = 0;

                    DKSNumbers.Add(value);
                }

            }
            catch
            {

            }
        }

        public void WriteXML(XmlWriter writer)
        {
            writer.WriteStartElement("StartDKSRecord");
            writer.WriteAttributeString("KicoCadDKSIndex", KicoCadDKSIndex.ToString());
            writer.WriteAttributeString("CaseHistoryIndex", CaseHistoryIndex.ToString());
            writer.WriteAttributeString("DKSType", DKSType.ToString());
            writer.WriteAttributeString("DKSAlgorithm", DKSAlgorithm.ToString());
            writer.WriteAttributeString("ModelType", ModelType.ToString());
            writer.WriteAttributeString("OptionName", OptionName.ToString());
            for (int i = 0; i < 20; i++)
            {
                writer.WriteAttributeString("Param" + (i + 1), DKSParams[i].ToString());
            }
            for (int i = 0; i < 100; i++)
            {
                writer.WriteAttributeString("DKS" + (i + 1), DKSNumbers[i].ToString());
            }
            writer.WriteEndElement();
        }

        internal static object GetColumnData(DataRow datarow, string columnName, object defaultval)
        {
            object ret;
            try
            {
                if (datarow != null && datarow.Table.Columns.Contains(columnName))
                    ret = datarow[columnName];
                else
                    ret = defaultval;
            }
            catch
            {
                ret = defaultval;
            }
            if (ret.GetType() == typeof(DBNull))
                ret = null;
            return ret;
        }
    }
}