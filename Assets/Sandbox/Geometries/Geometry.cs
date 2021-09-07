using Assets.CaseFile;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Geometries
{
    public enum ObjectType
    {
        Femur_Bone,
        Tibia_Bone,
        Patella_Bone,
        Pelvis_Bone,
        Femur_Component,
        Tibia_TrayComponent,
        Tibia_InsertComponent,
        Patella_Component,
        PelvisCup_Component,
        PelvisLiner_Component,
        FemurHead_Component,
        FemurStem_Component,
        Other
    }

    public class Geometry
    {
        public string Tag { get; set; }
        public Mesh Mesh { get; set; }
        public ObjectType ObjectType { get; set; }
        public List<Landmark> Landmarks { get; set; }
        public PositionalData EulerTransform { get; set; }
    }
}
