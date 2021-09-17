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

        private const string meshMaterialPath = "Materials/BoneMaterial";
        private const string transMaterialPath = "Materials/BoneTransparent";
        private const string lmPrefabPath = "Prefabs/Landmark";
        private GameObject lmPrefab;
        private GameObject meshObject;
        private List<GameObject> objects;
        private Material meshMaterial;
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
            meshMaterial = Resources.Load<Material>(meshMaterialPath);
            transMaterial = Resources.Load<Material>(transMaterialPath);
        }

        #endregion

        #region Public Methods

        public void DisplayObjects(Transform parent, int layer)
        {
            DestroyObjects();

            if (EulerTransform == null) return;

            if (Mesh != null)
            {
                meshObject = new GameObject(Tag, typeof(MeshFilter), typeof(MeshRenderer));
                meshObject.transform.parent = parent;
                meshObject.GetComponent<MeshFilter>().mesh = Mesh;
                meshObject.GetComponent<MeshRenderer>().material = meshMaterial;

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

        public void ToggleTransparency(bool transparent)
        {
            if(meshObject != null)
            {
                var material = transparent ? transMaterial : meshMaterial;
                meshObject.GetComponent<MeshRenderer>().material = material;
            }
        }

        #endregion
    }
}
