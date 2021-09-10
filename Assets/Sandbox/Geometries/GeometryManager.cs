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

        private void AddToDisplayList(Geometry data)
        {
            GameObject goButton = GameObject.Instantiate(buttonPrefab);
            goButton.transform.SetParent(parent.transform, false);

            goButton.GetComponentInChildren<TextMeshProUGUI>().text = data.Tag;

            Button tempButton = goButton.GetComponent<Button>();
            tempButton.onClick.AddListener(() => OnContentSelected(data.Tag));
            tempButton.name = data.Tag;
            objectButtons.Add(tempButton);
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
                AddToDisplayList(data);
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
            selectedGeometry?.UpdateTransform(transformString);
        }

        /// <summary>
        /// Update landmarks for selected geometry
        /// </summary>
        /// <param name="landmarks"></param>
        public void UpdateLandmarks(List<Landmark> landmarks)
        {
            if (!EnableLoad) return;

            var selectedGeometry = Geometries.FirstOrDefault(c =>
                                c.Tag == SelectedTags.FirstOrDefault());
            selectedGeometry?.UpdateLandmarks(landmarks);
        }

        /// <summary>
        /// Display all selected geometries
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="layer"></param>
        public void DisplaySelectedObjects(Transform parent, int layer)
        {
            var selectedItems = Geometries.Where(g =>
                                SelectedTags.Contains(g.Tag));

            if (null == selectedItems ||
                selectedItems.Count() == 0) return;

            foreach (var item in selectedItems)
            {
                item.DisplayObjects(parent, layer);
            }
        }

        /// <summary>
        /// Returns the main geometry object
        /// </summary>
        /// <param name="mainTag"></param>
        /// <returns></returns>
        public GameObject GetMainObject(string mainTag = null)
        {
            GameObject mainObject = null;

            if (!SelectedTags.Contains(mainTag))
            {
                mainTag = SelectedTags.FirstOrDefault();
            }

            mainObject = Geometries.FirstOrDefault(g =>
                                g.Tag == mainTag)?.Object;

            if (mainObject == null)
            {
                mainObject = Geometries.FirstOrDefault(g =>
                                SelectedTags.Contains(g.Tag) &&
                                g.Object != null)?.Object;
            }

            return mainObject;
        }

        public void UpdateDisplayList(Geometry data)
        {
            Geometries.Add(data);
            AddToDisplayList(data);
        }

        public void HideList()
        {
            parent.SetActive(false);
        }

        public void ShowList()
        {
            parent.SetActive(true);
        }

        /// <summary>
        /// Distroy all geometries
        /// </summary>
        public void DistroyAllObjects()
        {
            foreach (var data in Geometries)
            {
                data.DestroyObjects();
            }
            SelectedTags.Clear();
        }

        #endregion
    }
}
