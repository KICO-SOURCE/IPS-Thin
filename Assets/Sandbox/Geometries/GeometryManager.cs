using Assets.CaseFile;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Geometries
{
    public class GeometryManager
    {
        #region Private Members

        private static readonly Lazy<GeometryManager> _instance = new Lazy<GeometryManager>(() => new GeometryManager());

        private const string buttonPrefabPath = "Prefabs/Button";
        private const string parentTag = "ListParent";

        private GameObject parent;
        private GameObject buttonPrefab;
        private List<Button> objectButtons;
        private Color normalColor;
        private string mainTag = string.Empty;

        #endregion

        #region Properties

        public static GeometryManager Instance
        {
            get { return _instance.Value; }
        }

        public List<Geometry> Geometries { get; private set; }

        public List<string> SelectedTags { get; private set; }

        public bool EnableLoad => SelectedTags.Count == 1;

        public bool EnableView => SelectedTags.Count > 0;

        #endregion

        #region Constructor

        private GeometryManager()
        {
            Geometries = new List<Geometry>();
            SelectedTags = new List<string>();

            buttonPrefab = Resources.Load<GameObject>(buttonPrefabPath);
            Button tempButton = buttonPrefab.GetComponent<Button>();
            normalColor = tempButton.GetComponent<Image>().color;

            AddContents();
        }

        #endregion

        #region Private Methods

        //Dummy data
        private void AddContents()
        {
            Geometries.Add(new Geometry() { Tag = "Pelvis Sample" });
        }

        public void AddContent(Geometry content)
        {
            Geometries.Add(content);
        }

        private void OnContentSelected(string tag)
        {
            Debug.Log($"Selected : {tag}");

            var clicked = objectButtons.FirstOrDefault(b => b.name == tag);

            if (SelectedTags.Contains(tag))
            {
                SelectedTags.Remove(tag);
                if (clicked == null) return;
                clicked.GetComponent<Image>().color = normalColor;
            }
            else
            {
                SelectedTags.Add(tag);
                if (clicked == null) return;
                clicked.GetComponent<Image>().color = Color.gray;
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Display the list of all objects
        /// </summary>
        public void DisplayList()
        {
            parent = GameObject.FindGameObjectWithTag(parentTag);

            objectButtons = new List<Button>();
            foreach (var data in Geometries)
            {
                GameObject goButton = GameObject.Instantiate(buttonPrefab);
                goButton.transform.SetParent(parent.transform, false);

                goButton.GetComponentInChildren<TextMeshProUGUI>().text = data.Tag;

                Button tempButton = goButton.GetComponent<Button>();
                tempButton.onClick.AddListener(() => OnContentSelected(data.Tag));
                tempButton.name = data.Tag;
                objectButtons.Add(tempButton);
            }
        }

        /// <summary>
        /// Update transform for selected geometry
        /// </summary>
        /// <param name="transformString"></param>
        public void UpdateTransform(string transformString)
        {
            if (!EnableLoad) return;

            var selectedGeometry = Geometries.FirstOrDefault(c =>
                                c.Tag == SelectedTags.FirstOrDefault());
            if (selectedGeometry != null)
            {
                selectedGeometry.EulerTransform = new PositionalData(transformString);
            }
        }

        /// <summary>
        /// Update landmarks for selected geometry
        /// </summary>
        /// <param name="bone"></param>
        /// <param name="landmarks"></param>
        public void UpdateLandmarks(string bone, List<Landmark> landmarks)
        {
            foreach (var lm in landmarks)
            {
                lm.Bone = bone;
            }

            if (!EnableLoad) return;

            var selectedGeometry = Geometries.FirstOrDefault(c =>
                                c.Tag == SelectedTags.FirstOrDefault());
            if (selectedGeometry != null)
            {
                selectedGeometry.Landmarks = new List<Landmark>(landmarks);
            }
        }

        /// <summary>
        /// Display all selected geometries
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="layer"></param>
        /// <param name="tag"></param>
        public void DisplaySelectedObjects(Transform parent,
                                           int layer, string tag = null)
        {
            var selectedItems = Geometries.Where(g =>
                                SelectedTags.Contains(g.Tag));

            mainTag = string.Empty;
            if (null == selectedItems || selectedItems.Count() == 0) return;

            mainTag = tag == null || !SelectedTags.Contains(tag) ?
                            selectedItems.FirstOrDefault().Tag : tag;

            foreach(var item in selectedItems)
            {
                item.DisplayObjects(parent, layer);
            }
        }

        /// <summary>
        /// Returns the main geometry object
        /// </summary>
        /// <returns></returns>
        public GameObject GetMainObject()
        {
            return Geometries.FirstOrDefault(c => c.Tag == mainTag)?.Object;
        }

        #endregion
    }
}
