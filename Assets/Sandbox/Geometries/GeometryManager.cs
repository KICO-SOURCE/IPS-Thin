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
        private List<int> selectedIndices;

        #endregion

        #region Properties

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
            goButton.transform.SetParent(parent.transform, false);

            goButton.GetComponentInChildren<TextMeshProUGUI>().text = tag;

            Button tempButton = goButton.GetComponent<Button>();
            tempButton.onClick.AddListener(() => OnContentSelected(index));
            tempButton.name = tag;
            objectButtons.Add(tempButton);
        }

        private void OnContentSelected(int index)
        {
            Debug.Log($"Selected : {index}");

            var clicked = objectButtons[index];

            if (selectedIndices.Contains(index))
            {
                selectedIndices.Remove(index);
                if (clicked == null) return;
                clicked.GetComponent<Image>().color = normalColor;
            }
            else
            {
                selectedIndices.Add(index);
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
            for (int index = 0; index < Geometries.Count; index++)
            {
                AddToDisplayList(index, Geometries[index].Tag);
            }
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
            for(int index = 0; index < Geometries.Count; index++)
            {
                if(selectedIndices.Contains(index))
                {
                    Geometries[index]?.DisplayObjects(parent, layer);
                }
            }
        }

        /// <summary>
        /// Returns the main geometry object
        /// </summary>
        /// <param name="mainTag"></param>
        /// <returns></returns>
        public GameObject GetMainObject(int index = -1)
        {
            index = selectedIndices.Contains(index) ?
                index : selectedIndices.FirstOrDefault();
            return Geometries[index].Object;
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
            selectedIndices.Clear();
            Transparent = false;
        }

        public void ToggleTransparency()
        {
            Transparent = !Transparent;
            foreach (var data in Geometries)
            {
                data.ToggleTransparency(Transparent);
            }
        }

        #endregion
    }
}
