using System.ComponentModel;
using System.Xml;

namespace Assets.CaseFile
{
    public class AnatomicalMeasurements : INotifyPropertyChanged
    {
        private int idFunctionalValues;
        private string measure;
        private float measureValue;

        public event PropertyChangedEventHandler PropertyChanged;
        public int IdFunctionalValues
        {
            get => idFunctionalValues;
            set
            {
                idFunctionalValues = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IdFunctionalValues)));
            }
        }
        public string Measure
        {
            get => measure;
            set
            {
                measure = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Measure)));
            }
        }
        public float MeasureValue
        {
            get => measureValue;
            set
            {
                measureValue = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeasureValue)));
            }
        }


        internal void WriteXml(XmlWriter x)
        {
            x.WriteStartElement("Measurement");
            x.WriteAttributeString("ID", IdFunctionalValues.ToString());
            x.WriteAttributeString("Measure", Measure);
            x.WriteAttributeString("MeasureValue", MeasureValue.ToString());
            x.WriteEndElement();
        }

        internal void ReadXml(XmlTextReader reader)
        {
            string id = reader.GetAttribute("ID");
            var measure = reader.GetAttribute("Measure");
            var measureVal = float.Parse(reader.GetAttribute("MeasureValue"));

            int.TryParse(id, out int ID);
            IdFunctionalValues = ID;
            Measure = measure;
            MeasureValue = measureVal;

        }
    }
}
