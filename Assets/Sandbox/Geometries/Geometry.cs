using Assets.CaseFile;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Geometries
{
    public class Geometry
    {
        #region Constants

        private const string meshMaterialPath = "Materials/BoneMaterial";
        private const string buttonPrefabPath = "Prefabs/Landmark";
        private GameObject buttonPrefab;
        private List<GameObject> objects;

        #endregion

        #region Properties

        public string Tag { get; set; }
        public Mesh Mesh { get; set; }
        public List<Landmark> Landmarks { get; set; }
        public PositionalData EulerTransform { get; set; }

        #endregion

        #region Constructor

        public Geometry()
        {
            objects = new List<GameObject>();
        }

        #endregion

        #region Private Methods

        private GameObject CreateObject(string tag, Transform parent,
                   Transform transform, Mesh mesh, Material material)
        {
            var go = new GameObject(tag, typeof(MeshFilter), typeof(MeshRenderer));
            go.transform.parent = parent;
            go.GetComponent<MeshFilter>().mesh = mesh;
            go.GetComponent<MeshRenderer>().material = material;

            go.transform.localPosition = transform.position;
            go.transform.localEulerAngles = transform.eulerAngles;

            go.SetActive(true);
            return go;
        }

        #endregion

        #region Public Methods

        public void DisplayObjects(Transform parent)
        {
            DestroyObjects();

            if (EulerTransform == null) return;

            var material = Resources.Load<Material>(meshMaterialPath);
            var transform = EulerTransform.GetTransfrom();

            if (Mesh != null)
            {
                var go = new GameObject(Tag, typeof(MeshFilter), typeof(MeshRenderer));
                go.transform.parent = parent;
                go.GetComponent<MeshFilter>().mesh = Mesh;
                go.GetComponent<MeshRenderer>().material = material;

                go.transform.localPosition = transform.position;
                go.transform.localEulerAngles = transform.eulerAngles;

                go.SetActive(true);
                objects.Add(go);
            }

            if (Landmarks == null) return;

            buttonPrefab = Resources.Load<GameObject>(buttonPrefabPath);
            material.color = Color.blue;
            foreach(var lm in Landmarks)
            {
                GameObject go = GameObject.Instantiate(buttonPrefab);
                go.name = $"{Tag}_{lm.Type}";
                go.transform.parent = parent;
                go.GetComponent<MeshRenderer>().material = material;

                var position = transform.TransformPoint(lm.Position);
                go.transform.localPosition = position;

                go.SetActive(true);
                objects.Add(go);
            }
        }

        public void DestroyObjects()
        {
            foreach (var go in objects)
            {
                go.SetActive(false);
                UnityEngine.Object.DestroyImmediate(go);
            }
            objects.Clear();
        }

        #endregion
    }
}
