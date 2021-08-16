using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.CaseFile.Components
{
    public abstract class Implant
    {
        public string Brand { get; set; }
        public string Variant { get; set; }
        public string Size { get; set; }
        public string Side { get; set; }
        public Mesh Geometry { get; set; }
        public List<Landmark> Landmarks { get; set; }
        public abstract string ComponentType();
        public Mesh CompVisual;
        public Transform ComponentTransformation { get; set; }
        public Vector3[] InnerTrayPoly { get; set; }
        public Vector3[] OuterPoly { get; set; }

        public static bool UseEncryption = true;

        public Landmark this[string landmarkId]
        {
            get
            {
                return this.Landmarks.FirstOrDefault(l => l.Type == landmarkId);
            }
        }

        protected Implant(string brand, string variant, string size, string side, Mesh mesh, List<Landmark> landmarks)
        {
            Brand = brand;
            Variant = variant;
            Size = size;
            Side = side;
            if (mesh != null) Geometry = mesh;
            if (landmarks != null) Landmarks = landmarks;
        }
        public Implant()
        {

        }

        internal abstract void ReadFromFixedpath(byte[] datFileContent);

        public string ConCatComp
        {
            get { return Brand + "," + Size; }

        }

        #region Visuals

        /// <summary>
        /// 7.2.2015  JKH  Added to clear visuals
        /// </summary>
        /// 

        internal void Clear()
        {
            if (CompVisual != null) CompVisual.Clear();
        }

        #endregion

    }
}