using System;
using System.ComponentModel;
using System.Xml;
using UnityEngine;

namespace Assets.CaseFile
{
    public class KeyLandMarkers : INotifyPropertyChanged
    {
        private string landMarkId;
        private Vector3 landMark;
        private Vector3 landMarkVector;

        public event PropertyChangedEventHandler PropertyChanged;

        public string LandMarkId
        {
            get => landMarkId;
            set
            {
                landMarkId = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LandMarkId)));
            }
        }
        public Vector3 LandMark
        {
            get => landMark;
            set
            {
                landMark = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LandMark)));
            }
        }
        public Vector3 LandMarkVector
        {
            get => landMarkVector;
            set
            {
                landMarkVector = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LandMarkVector)));
            }
        }
        public string LandMarkStr { get { return "X:" + Math.Round(LandMark.x, 2) + " Y:" + Math.Round(LandMark.y, 2) + " Z:" + Math.Round(LandMark.z, 2); } }

        /// <summary>
        /// Get the landmark point as comma separated value.(Rounded to 2 digits)
        /// </summary>
        public string LandMarkCSV
        {
            get
            {
                return
                    string.Format("{0},{1},{2}", Math.Round(LandMark.x, 2).ToString("0.00"),
                    Math.Round(LandMark.y, 2).ToString("0.00"), Math.Round(LandMark.z, 2).ToString("0.00"));
            }
        }


        internal void WriteXml(XmlWriter x)
        {
            x.WriteStartElement("LandMark");
            x.WriteAttributeString("ID", LandMarkId);
            //TODO: This settings may need to enabled in future
            //if (ApplicationSettings.SwitchCoordinateSystem)
            //{
            //    x.WriteAttributeString("X", LandMark.z.ToString());
            //    x.WriteAttributeString("Y", (-1 * LandMark.x).ToString());
            //    x.WriteAttributeString("Z", LandMark.y.ToString());
            //}
            //else
            {
                x.WriteAttributeString("X", LandMark.x.ToString());
                x.WriteAttributeString("Y", LandMark.y.ToString());
                x.WriteAttributeString("Z", LandMark.z.ToString());
            }
            x.WriteEndElement();
        }

        internal void WriteXmlNative(XmlWriter x)
        {
            x.WriteStartElement("LandMark");
            x.WriteAttributeString("ID", LandMarkId);
            x.WriteAttributeString("X", LandMark.x.ToString());
            x.WriteAttributeString("Y", LandMark.y.ToString());
            x.WriteAttributeString("Z", LandMark.z.ToString());
            x.WriteEndElement();
        }

        internal void ReadXml(XmlTextReader reader)
        {

            string type = reader.GetAttribute("ID");
            var x = float.Parse(reader.GetAttribute("X"));
            var y = float.Parse(reader.GetAttribute("Y"));
            var z = float.Parse(reader.GetAttribute("Z"));

            LandMarkId = type;

            //TODO: This settings may need to enabled in future
            //if (ApplicationSettings.SwitchCoordinateSystem)
            //{
            //    LandMark = new Vector3(-y, z, x);
            //}
            //else
            {
                LandMark = new Vector3(x, y, z);
            }

        }

        internal void ReadEncryptedXml(XmlTextReader reader)
        {
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            string type = reader.GetAttribute("ID");
            var x = reader.GetAttribute("X");
            var y = reader.GetAttribute("Y");
            var z = reader.GetAttribute("Z");

            type = encoder.GetString(Convert.FromBase64String(type));
            x = encoder.GetString(Convert.FromBase64String(x));
            y = encoder.GetString(Convert.FromBase64String(y));
            z = encoder.GetString(Convert.FromBase64String(z));
            LandMarkId = type;

            //TODO: This settings may need to enabled in future
            //if (ApplicationSettings.SwitchCoordinateSystem)
            //{
            //    LandMark = new Vector3(-1 * float.Parse(y), float.Parse(z), float.Parse(x));
            //}
            //else
            {
                LandMark = new Vector3(float.Parse(x), float.Parse(y), float.Parse(z));
            }
        }

        internal void WriteEncryptedXml(XmlWriter x)
        {
            System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
            x.WriteStartElement("LandMark");
            x.WriteAttributeString("ID", Convert.ToBase64String(encoder.GetBytes(LandMarkId)));
            x.WriteAttributeString("X", Convert.ToBase64String(encoder.GetBytes(LandMark.x.ToString())));
            x.WriteAttributeString("Y", Convert.ToBase64String(encoder.GetBytes(LandMark.y.ToString())));
            x.WriteAttributeString("Z", Convert.ToBase64String(encoder.GetBytes(LandMark.z.ToString())));
            x.WriteEndElement();
        }


        public KeyLandMarkers CreateDeepCopy()
        {
            KeyLandMarkers Temp = new KeyLandMarkers();

            Temp.LandMarkId = this.LandMarkId;
            Temp.LandMark = new Vector3(this.LandMark.x, this.LandMark.y, this.LandMark.z);
            Temp.LandMarkVector = new Vector3(this.LandMarkVector.x, this.LandMarkVector.y, this.LandMarkVector.z);
            return Temp;
        }

        /// <summary>
        /// Checking whether the landmark in the LM list and keylandmark are different
        /// </summary>
        public bool IsLandMarksEqual(Vector3 landMarks)
        {
            if (landMarks.x != LandMark.x || landMarks.y != LandMark.y || landMarks.z != LandMark.z)
            {
                return true;
            }
            return false;
        }
    }
}