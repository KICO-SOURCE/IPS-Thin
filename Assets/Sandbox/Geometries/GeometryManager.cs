using Assets.CaseFile;
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

        private const string buttonPrefabPath = "Prefabs/Button";
        private const string parentTag = "ListParent";

        private GameObject parent;
        private GameObject buttonPrefab;
        private List<Button> objectButtons;

        #endregion

        #region Properties

        public List<Geometry> Geometries { get; private set; }
        public string SelectedTag { get; private set; }

        public Geometry SelectedGeometry => Geometries.FirstOrDefault(c =>
                                                        c.Tag == SelectedTag);

        #endregion

        #region Constructor

        public GeometryManager()
        {
            Geometries = new List<Geometry>();
            AddContents();
        }

        #endregion

        #region Private Methods

        //Dummy data
        private void AddContents()
        {
            Geometries.Add(new Geometry() { Tag = "DistalFemur" });
            Geometries.Add(new Geometry() { Tag = "DistalTibia" });
            Geometries.Add(new Geometry() { Tag = "FemurComponent" });
            Geometries.Add(new Geometry() { Tag = "PatellaComponent" });
        }

        private void OnContentSelected(string tag)
        {
            var normalColor = objectButtons.FirstOrDefault(b => b.name != SelectedTag)?
                                .GetComponent<Image>()?.color;
            if (normalColor != null)
            {
                var selected = objectButtons.FirstOrDefault(b => b.name == SelectedTag);
                if (selected != null)
                {
                    selected.GetComponent<Image>().color = normalColor.Value;
                }

                selected = objectButtons.FirstOrDefault(b => b.name == tag);
                if (selected != null)
                {
                    selected.GetComponent<Image>().color = Color.gray;
                }
            }
            Debug.Log($"Selected : {tag}");
            SelectedTag = tag;
        }

        #endregion

        #region Public Methods

        public void DisplayList()
        {
            buttonPrefab = Resources.Load<GameObject>(buttonPrefabPath);
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

        public void SetSelectedTransform(string transformString)
        {
            if (SelectedGeometry != null)
            {
                SelectedGeometry.EulerTransform = new PositionalData(transformString);
            }
        }

        #endregion
    }
}
