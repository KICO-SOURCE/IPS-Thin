using Assets.CaseFile;
using Assets.Sandbox.Import;
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
        private const string containerTag = "ListContainer";
        private const string meshMaterialPath = "Materials/BoneMaterial";
        private const string transMaterialPath = "Materials/BoneTransparent";
        private const string opaqueSelectedMaterialPath = "Materials/OpaqueSelectedColor";
        private const string transSelectedMaterialPath = "Materials/TransparentSelectedColor";

        private GameObject container;
        private GameObject buttonPrefab;
        private Color normalColor;
        private List<int> selectedIndices;
        private List<Geometry> geometries;

        #region Materials

        private Material opaqueMaterial;
        private Material transMaterial;
        private Material opaqueSelectedMaterial;
        private Material transSelectedMaterial;

        #endregion

        #endregion

        #region Properties

        public List<Button> objectButtons;
        public static GeometryManager Instance
        {
            get { return _instance.Value; }
        }

        public bool EnableLoad => selectedIndices.Count == 1;

        public bool EnableView => selectedIndices.Count > 0;

        public bool Transparent { get; private set; } = false;

        #endregion

        #region Constructor

        private GeometryManager()
        {
            geometries = new List<Geometry>();
            selectedIndices = new List<int>();

            buttonPrefab = Resources.Load<GameObject>(buttonPrefabPath);
            Button tempButton = buttonPrefab.GetComponent<Button>();
            normalColor = tempButton.GetComponent<Image>().color;

            opaqueMaterial = Resources.Load<Material>(meshMaterialPath);
            transMaterial = Resources.Load<Material>(transMaterialPath);
            opaqueSelectedMaterial = Resources.Load<Material>(opaqueSelectedMaterialPath);
            transSelectedMaterial = Resources.Load<Material>(transSelectedMaterialPath);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Add item to the display list
        /// </summary>
        /// <param name="index"></param>
        /// <param name="tag"></param>
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

        /// <summary>
        /// Handle content selected
        /// </summary>
        /// <param name="index"></param>
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

        /// <summary>
        /// Update the highlights of the mesh
        /// </summary>
        private void UpdateHighlights()
        {
            for (int index = 0; index < geometries.Count; index++)
            {
                var selected = selectedIndices.Contains(index);
                geometries[index].UpdateMeshMaterial(GetMaterial(selected));
            }
        }

        /// <summary>
        /// validate landmark data format.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool CheckFormat(string[] value)
        {
            if (IsChar(value[0]) && IsNumeric(value[1]) &&
                IsNumeric(value[2]) && IsNumeric(value[3]))
                return true;
            return false;
        }

        /// <summary>
        /// Checks the string contains numeric value.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool IsNumeric(string val)
        {
            foreach (var str in val)
            {
                if (Char.IsDigit(str))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Check the string contains char data.
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        private bool IsChar(string val)
        {
            foreach (var str in val)
            {
                if (!(Char.IsDigit(str)))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Parse transform from string data.
        /// </summary>
        /// <param name="transformString"></param>
        /// <returns></returns>
        private string ParseTransformString(string transformString)
        {
            string result = transformString;

            var splitInput = transformString.Split(',');
            splitInput = splitInput.Where(val => val != splitInput[0] &&
                                          val != splitInput[7]).ToArray();

            result = string.Join(",", splitInput);
            return result;
        }

        /// <summary>
        /// Get object type.
        /// </summary>
        /// <param name="type"></param>
        private ObjectType GetObjectType(string typeName)
        {
            var objectTypes = Enum.GetValues(typeof(ObjectType)).Cast<ObjectType>();

            var type = objectTypes.FirstOrDefault(ob =>
                            ob.ToString().ToLower() == typeName?.ToLower());

            return type;
        }

        private Material GetMaterial(bool isSelected)
        {
            var material = Transparent ?
                isSelected ? transSelectedMaterial : transMaterial :
                isSelected ? opaqueSelectedMaterial : opaqueMaterial;
            return material;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Display the list of all objects
        /// </summary>
        public void DisplayList()
        {
            container = GameObject.FindGameObjectWithTag(containerTag);
            objectButtons = new List<Button>();
            for (int index = 0; index < geometries.Count; index++)
            {
                AddToDisplayList(index, geometries[index].Tag);
            }
        }

        /// <summary>
        /// Load mesh and add to geometry list
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="type"></param>
        /// <param name="filePath"></param>
        public void LoadMesh(string tag, string type, string filePath)
        {
            var mesh = MeshGeometryFunctions.ReadStl(filePath);

            Geometry geometry = new Geometry()
            {
                Tag = tag,
                ObjectType = GetObjectType(type),
                Mesh = mesh?.ToMesh()
            };

            Debug.Log("Tag :" + geometry.Tag);
            Debug.Log("Object Type : " + geometry.ObjectType);

            var index = geometries.Count;
            geometries.Add(geometry);
            AddToDisplayList(index, tag);
        }

        /// <summary>
        /// Load transform for selected geometry
        /// </summary>
        /// <param name="transformFile"></param>
        public void LoadTransform(string transformFile)
        {
            if (!EnableLoad) return;

            string transformString = System.IO.File.ReadAllText(transformFile);
            transformString = ParseTransformString(transformString);

            var log = transformString == null ? "Invalid Transform" : transformString;
            Debug.Log(log);

            var selectedGeometry = geometries[selectedIndices.First()];
            selectedGeometry?.UpdateTransform(transformString);
        }

        /// <summary>
        /// Load landmarks for selected geometry
        /// </summary>
        /// <param name="landmarks file"></param>
        public void LoadLandmarks(string landmarksFile)
        {
            if (!EnableLoad) return;

            var landmarks = new List<Landmark>();
            var reader = new System.IO.StreamReader(landmarksFile);
            try
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var value = line.Split(',');

                    if (!(value.Length > 4) &&
                        !value.Contains("Name") &&
                        CheckFormat(value))
                    {
                        string type = value[0];
                        float x;
                        float.TryParse(value[1], out x);
                        float y;
                        float.TryParse(value[2], out y);
                        float z;
                        float.TryParse(value[3], out z);

                        landmarks.Add(new Landmark
                        {
                            Type = type,
                            Position = new Vector3(x, y, z)
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
            }

            var selectedGeometry = geometries[selectedIndices.First()];
            selectedGeometry?.UpdateLandmarks(landmarks);
        }

        /// <summary>
        /// Display all geometries
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="layer"></param>
        public void DisplayAllObjects(Transform parent, int layer)
        {
            for (int index = 0; index < geometries.Count; index++)
            {
                var selected = selectedIndices.Contains(index);
                geometries[index].DisplayObjects(parent, layer,
                                        GetMaterial(selected));
            }
        }

        /// <summary>
        /// Returns the main geometry object
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public GameObject GetMainObject(int index = -1)
        {
            if(index > 0 && index < geometries.Count)
            {
                return geometries[index]?.Object;
            }
            else
            {
                return geometries.FirstOrDefault()?.Object;
            }
        }

        /// <summary>
        /// Toggle Mesh transparency
        /// </summary>
        public void ToggleTransparency()
        {
            Transparent = !Transparent;
            UpdateHighlights();
        }

        /// <summary>
        /// Check if tag is existing
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool IsExistingTag(string tag)
        {
            return geometries.Any(g => g.Tag == tag);
        }

        #endregion
    }
}
