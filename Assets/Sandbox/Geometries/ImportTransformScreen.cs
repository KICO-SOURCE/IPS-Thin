using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using Assets.Geometries;
using TMPro;
using Assets.CaseFile;

public class ImportTransformScreen : MonoBehaviour
{
    #region Private Members

    private const string buttonPrefabPath = "Prefabs/Button";
    private const string parentTag = "ListParent";
    private const string importButtonName = "ImportTransform";

    private GameObject parent;
    private GameObject buttonPrefab;
    private GeometryManager geometryManager;
    private Button importTransformBtn;

    private string selectedTag = string.Empty;

    #endregion

    #region Public Methods

    // Start is called before the first frame update
    void Start()
    {
        InitializeData();
    }

    // Update is called once per frame
    void Update()
    {
    }

    #endregion

    #region Private Methods

    private void ImportTransform()
    {
        if (string.IsNullOrEmpty(selectedTag)) return;

        string path = EditorUtility.OpenFilePanel("Select the transform file.", "", "csv, CSV");

        if (string.IsNullOrEmpty(path)) return;

        string transformString = System.IO.File.ReadAllText(path);
        transformString = ParseTransformString(transformString);
        Debug.Log(transformString);
        if (transformString == null)
        {
            Debug.Log("Implant transform is not available for the selected component");
        }
        else
        {
            var selected = geometryManager.Geometries.FirstOrDefault(c => c.Tag == selectedTag);
            if (selected != null)
            {
                selected.EulerTransform = new PositionalData(transformString);
            }
        }
    }

    private void InitializeData()
    {
        importTransformBtn = GameObject.Find(importButtonName).GetComponent<Button>();
        importTransformBtn.onClick.AddListener(ImportTransform);

        buttonPrefab = Resources.Load<GameObject>(buttonPrefabPath);
        parent = GameObject.FindGameObjectWithTag(parentTag);
        geometryManager = new GeometryManager();

        foreach (var data in geometryManager.Geometries)
        {
            GameObject goButton = (GameObject)Instantiate(buttonPrefab);
            goButton.transform.SetParent(parent.transform, false);

            goButton.GetComponentInChildren<TextMeshProUGUI>().text = data.Tag;

            Button tempButton = goButton.GetComponent<Button>();
            tempButton.onClick.AddListener(() => OnContentSelected(data.Tag));
        }
    }

    private void OnContentSelected(string tag)
    {
        Debug.Log($"Selected : {tag}");
        selectedTag = tag;
    }

    private static string ParseTransformString(string transformString)
    {
        string result = transformString;

        var splitInput = transformString.Split(',');
        splitInput = splitInput.Where(val => val != splitInput[0] &&
                                      val != splitInput[7]).ToArray();

        result = string.Join(",", splitInput);
        return result;
    }

    #endregion
}