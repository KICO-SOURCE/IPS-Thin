using System;
using System.Xml;
using UnityEngine;

namespace Assets.CaseFile
{
    public class Landmark
    {
        public string Bone { get; set; }
        public string Type { get; set; }
        public Vector3 Position { get; set; }

        public void ReadLandmark(XmlTextReader reader,
            bool encrypted = false, string bone = null,
            bool switchCoordinateSystem = true)
        {
            string attachedBone = string.Empty;
            string type = string.Empty;
            string x = string.Empty;
            string y = string.Empty;
            string z = string.Empty;
            if(encrypted)
            {
                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                attachedBone = reader.GetAttribute("Bone");
                type = reader.GetAttribute("ID");
                x = reader.GetAttribute("X");
                y = reader.GetAttribute("Y");
                z = reader.GetAttribute("Z");

                attachedBone = encoder.GetString(Convert.FromBase64String(type));
                type = encoder.GetString(Convert.FromBase64String(type));
                x = encoder.GetString(Convert.FromBase64String(x));
                y = encoder.GetString(Convert.FromBase64String(y));
                z = encoder.GetString(Convert.FromBase64String(z));
            }
            else
            {
                attachedBone = reader.GetAttribute("Bone");
                type = reader.GetAttribute("ID");
                x = reader.GetAttribute("X");
                y = reader.GetAttribute("Y");
                z = reader.GetAttribute("Z");
            }

            Bone = string.IsNullOrEmpty(bone) ?
                        attachedBone : bone;
            Type = type;
            if (switchCoordinateSystem)
            {
                Position = new Vector3(-1 * float.Parse(y),
                                       float.Parse(z),
                                       float.Parse(x));
            }
            else
            {
                Position = new Vector3(float.Parse(x),
                                       float.Parse(y),
                                       float.Parse(z));
            }
        }
    }
}