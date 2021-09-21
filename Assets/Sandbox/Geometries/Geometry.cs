using Assets.CaseFile;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Assets.Sandbox.Import;

namespace Assets.Geometries
{
    public class Geometry
    {
        #region Constants

        private const string lmPrefabPath = "Prefabs/Landmark";
        private GameObject lmPrefab;
        private GameObject meshObject;
        private List<GameObject> objects;

        #endregion

        #region Materials

        private Material opaqueMaterial;
        private Material transMaterial;

        #endregion

        #region Properties

        public string Tag { get; set; }
        public Mesh Mesh { get; set; }
        public List<Landmark> Landmarks { get; private set; }
        public PositionalData EulerTransform { get; private set; }
        public ObjectType ObjectType { get; set; }

        public GameObject Object => meshObject != null ?
                            meshObject : objects.FirstOrDefault();

        #endregion

        #region Constructor

        public Geometry()
        {
            objects = new List<GameObject>();
            EulerTransform = new PositionalData(null);
        }

        #endregion

        #region Public Methods

        public void DisplayObjects(Transform parent, int layer,
                                   bool isTransparent, bool isSelected)
        {
            DestroyObjects();

            if (EulerTransform == null) return;

            if (Mesh != null)
            {
                var material = GetMaterial(isTransparent, isSelected);
                meshObject = new GameObject(Tag, typeof(MeshFilter), typeof(MeshRenderer));
                meshObject.transform.parent = parent;
                meshObject.GetComponent<MeshFilter>().mesh = Mesh;
                meshObject.GetComponent<MeshRenderer>().material = material;

                EulerTransform.TransformObject(meshObject);
                meshObject.layer = layer;

                meshObject.SetActive(true);
            }

            if (Landmarks == null) return;

            lmPrefab = Resources.Load<GameObject>(lmPrefabPath);
            foreach(var lm in Landmarks)
            {
                GameObject go = GameObject.Instantiate(lmPrefab);
                go.name = $"{Tag}_{lm.Type}";
                go.transform.parent = parent;
                go.GetComponent<MeshRenderer>().material.color = Color.red;

                var position = EulerTransform.TransformPoint(lm.Position);
                go.transform.localPosition = position;
                go.layer = layer;

                go.SetActive(true);
                objects.Add(go);
            }
        }

        public void DestroyObjects()
        {
            if(meshObject != null)
            {
                meshObject.SetActive(false);
                UnityEngine.Object.DestroyImmediate(meshObject);
                meshObject = null;
            }

            foreach (var go in objects)
            {
                go.SetActive(false);
                UnityEngine.Object.DestroyImmediate(go);
            }
            objects.Clear();
        }

        public void UpdateLandmarks(List<Landmark> landmarks)
        {
            Landmarks = new List<Landmark>(landmarks);
        }

        public void UpdateTransform(string transform)
        {
            EulerTransform = new PositionalData(transform);
        }

        public void UpdateHighlight(bool isTransparent, bool isSelected)
        {
            if (meshObject == null) return;

            var material = GetMaterial(isTransparent, isSelected);
            meshObject.GetComponent<MeshRenderer>().material = material;
        }

        public void UpdateMaterials(Material opaqueMaterial, Material transMaterial)
        {
            this.opaqueMaterial = Material.Instantiate(opaqueMaterial);
            this.transMaterial = Material.Instantiate(transMaterial);
        }

        #endregion

        #region Private Methods

        private Material GetMaterial(bool isTransparent, bool isSelected)
        {
            var material = isTransparent ? transMaterial : opaqueMaterial;

            var color = isSelected ? new Color(0.943f, 0.805f, 0.805f) :
                                     new Color(1.000f, 1.000f, 1.000f);
            color.a = material.color.a;

            material.color = color;
            return material;
        }

        #endregion
    }
}
