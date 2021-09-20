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
        private const string containerTag = "ListContainer";

        private GameObject parent;
        private GameObject container;
        private GameObject buttonPrefab;
        private Color normalColor;
        private List<int> selectedIndices;

        #endregion

        #region Properties

        public List<Button> objectButtons;
        public static GeometryManager Instance
        {
            get { return _instance.Value; }
        }

        public List<Geometry> Geometries { get; private set; }

        public bool EnableLoad => selectedIndices.Count == 1;

        public bool EnableView => selectedIndices.Count > 0;

        public bool Transparent { get; private set; } = false;

        #endregion

        #region Constructor

        private GeometryManager()
        {
            Geometries = new List<Geometry>();
            selectedIndices = new List<int>();

            buttonPrefab = Resources.Load<GameObject>(buttonPrefabPath);
            Button tempButton = buttonPrefab.GetComponent<Button>();
            normalColor = tempButton.GetComponent<Image>().color;
        }

        #endregion

        #region Private Methods

        private void AddToDisplayList(int index, string tag)
        {
            GameObject goButton = GameObject.Instantiate(buttonPrefab);
            goButton.transform.SetParent(container.transform, false);

            goButton.GetComponentInChildren<TextMeshProUGUI>().text = tag;

            Button tempButton = goButton.GetComponent<Button>();
            tempButton.onClick.AddListener(() => OnContentSelected(index));
            tempButton.name = tag;
            objectButtons.Add(tempButton);
        }

        private void OnContentSelected(int index)
        {
            Debug.Log($"Selected : {index}");

            //Single selection
            var clicked = objectButtons[index];
            if (selectedIndices.Contains(index))
            {
                selectedIndices.Clear();
                clicked.GetComponent<Image>().color = normalColor;
            }
            else
            {
                foreach (var selectedIndex in selectedIndices)
                {
                    var selected = objectButtons[selectedIndex];
                    selected.GetComponent<Image>().color = normalColor;
                }
                selectedIndices.Clear();
                selectedIndices.Add(index);
                clicked.GetComponent<Image>().color = Color.gray;
            }


            //Multiple selection
            //var clicked = objectButtons[index];

            //if (selectedIndices.Contains(index))
            //{
            //    selectedIndices.Remove(index);
            //    clicked.GetComponent<Image>().color = normalColor;
            //}
            //else
            //{
            //    selectedIndices.Add(index);
            //    clicked.GetComponent<Image>().color = Color.gray;
            //}
            UpdateHighlights();
        }

        private void UpdateHighlights()
        {
            for (int index = 0; index < Geometries.Count; index++)
            {
                var selected = selectedIndices.Contains(index);
                Geometries[index].UpdateMeshMaterial(Transparent, selected);
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
            container = GameObject.FindGameObjectWithTag(containerTag);
            objectButtons = new List<Button>();
            for (int index = 0; index < Geometries.Count; index++)
            {
                AddToDisplayList(index, Geometries[index].Tag);
            }

            var active = objectButtons.Count > 0;
            parent.SetActive(active);
        }

        /// <summary>
        /// Update transform for selected geometry
        /// </summary>
        /// <param name="transformString"></param>
        public void UpdateTransform(string transformString)
        {
            if (!EnableLoad) return;

            var selectedGeometry = Geometries[selectedIndices.First()];
            selectedGeometry?.UpdateTransform(transformString);
        }

        /// <summary>
        /// Update landmarks for selected geometry
        /// </summary>
        /// <param name="landmarks"></param>
        public void UpdateLandmarks(List<Landmark> landmarks)
        {
            if (!EnableLoad) return;

            var selectedGeometry = Geometries[selectedIndices.First()];
            selectedGeometry?.UpdateLandmarks(landmarks);
        }

        /// <summary>
        /// Display all selected geometries
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="layer"></param>
        public void DisplaySelectedObjects(Transform parent, int layer)
        {
            foreach(var geometry in Geometries)
            {
                geometry?.DisplayObjects(parent, layer);
            }
        }

        /// <summary>
        /// Returns the main geometry object
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject GetMainObject(int index = -1)
        {
            if(index > 0 && index < Geometries.Count)
            {
                return Geometries[index]?.Object;
            }
            else
            {
                return Geometries.FirstOrDefault()?.Object;
            }
        }

        public void UpdateDisplayList(Geometry data)
        {
            var index = Geometries.Count;
            Geometries.Add(data);
            parent.SetActive(true);
            AddToDisplayList(index, data.Tag);
        }

        public void HideList()
        {
            parent.SetActive(false);
        }

        public void ShowList()
        {
            var active = objectButtons.Count > 0;
            parent.SetActive(active);
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
            selectedIndices.Clear();
            Transparent = false;
        }

        public void ToggleTransparency()
        {
            Transparent = !Transparent;
            UpdateHighlights();
        }

        #endregion
    }
}
