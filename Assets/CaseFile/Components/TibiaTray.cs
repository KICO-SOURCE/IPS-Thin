﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine;

namespace Assets.CaseFile.Components
{
    public class TibiaTray : Implant
    {
        public TibiaTray(String brand, string variant, String size, string side, Mesh mesh, List<Landmark> landmark)
            : base(brand, variant, size, side, mesh, landmark)
        {
        }

        public override string ComponentType()
        {
            return "Tibia Tray";
        }

        public TibiaTray()
        {

        }

        internal override void ReadFromFixedpath(byte[] datFileContent)
        {
            var path = System.IO.Path.GetTempFileName();
            File.WriteAllBytes(path, datFileContent);
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
                            // Do not update the variant for "Apex" since the apex variant is given as "Right" in DAT file  convention.
                            if (brand != "Apex" && brand != "Attune")
                            {
                                Variant = variant;
                            }
                            Size = size;
                            Side = side;
                        }
                        else if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "Mesh")
                        {
                            Mesh mesh = MeshGeometryFunctions.GetXmlMeshGeometry3D(reader);
                            Geometry = mesh;

                        }
                        else if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "OuterPoly")
                        {
                            OuterPoly = new Vector3[0];
                        }
                        else if (reader.MoveToContent() == XmlNodeType.Element && reader.Name == "InnerPoly")
                        {
                            InnerTrayPoly = new Vector3[0];
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
                File.Delete(path);
            }
        }
    }
}
