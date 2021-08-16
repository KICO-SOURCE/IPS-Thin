using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Assets.CaseFile.Components
{
    public class PelvisCup : Implant
    {
        public const string ComponentTypeName = "Pelvis Cup";

        public Vector3 AcetabularAxis
        {
            get
            {
                return (this["CupApex"].Position - this["CupCentre"].Position);
            }
        }

        public Vector3 RimAxis
        {
            get
            {
                return (this["CupRim2"].Position - this["CupRim1"].Position);
            }
        }

        public PelvisCup()
        {

        }

        public PelvisCup(string brand, string variant, String size, string side, Mesh mesh, List<Landmark> landmark)
            : base(brand, variant, size, side, mesh, landmark)
        {
        }

        public override string ComponentType()
        {
            return ComponentTypeName;
        }

        internal override void ReadFromFixedpath(byte[] datFileContent)
        {
            var path = System.IO.Path.GetTempFileName();
            File.WriteAllBytes(path, datFileContent);
            ReadFile(path);
            File.Delete(path);
        }

        private void ReadFile(string path)
        {
            if (System.IO.File.Exists(path))
            {
                Landmarks = new List<Landmark>();
                using (XmlTextReader reader = new XmlTextReader(path))
                {
                    reader.Read();
                    while (!reader.EOF)
                    {
                        if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "Component")
                        {
                            string type = reader.GetAttribute("Type");
                            string brand = reader.GetAttribute("Brand");
                            string variant = reader.GetAttribute("Variant");
                            string size = reader.GetAttribute("Size");
                            string side = reader.GetAttribute("Side");
                            if (Implant.UseEncryption)
                            {
                                System.Text.ASCIIEncoding encoder = new System.Text.ASCIIEncoding();
                                type = string.IsNullOrEmpty(reader.GetAttribute("Type")) ? "" :
                                    encoder.GetString(Convert.FromBase64String(reader.GetAttribute("Type")));
                                brand = string.IsNullOrEmpty(reader.GetAttribute("Brand")) ? "" :
                                    encoder.GetString(Convert.FromBase64String(reader.GetAttribute("Brand")));
                                variant = string.IsNullOrEmpty(reader.GetAttribute("Variant")) ? "" :
                                    encoder.GetString(Convert.FromBase64String(reader.GetAttribute("Variant")));
                                size = string.IsNullOrEmpty(reader.GetAttribute("Size")) ? "" :
                                    encoder.GetString(Convert.FromBase64String(reader.GetAttribute("Size")));
                                side = string.IsNullOrEmpty(reader.GetAttribute("Side")) ? "" :
                                    encoder.GetString(Convert.FromBase64String(reader.GetAttribute("Side")));
                            }

                            Brand = brand;
                            Variant = variant;
                            Size = size;
                            Side = side;
                        }
                        else if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "Mesh")
                        {
                            Geometry = MeshGeometryFunctions.GetXmlMeshGeometry3D(reader);
                        }
                        else if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "LandMarks")
                        {
                            while (!reader.EOF)
                            {
                                Landmark kl = new Landmark();
                                if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "LandMark")
                                {
                                    kl.ReadLandmark(reader, UseEncryption);
                                    Landmarks.Add(kl);
                                }

                                if (!reader.Read()) break;
                            }
                        }

                        if (!reader.Read()) break;
                    }
                }
            }
        }
    }
}
