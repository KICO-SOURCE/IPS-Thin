using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using Assets.Geometries;
using UnityEngine.SceneManagement;

public class ImportTransformScreen : MonoBehaviour
{
    #region Private Members

    private const string importButtonName = "ImportTransform";
    private const string viewButtonName = "View";

    private GeometryManager geometryManager;
    private Button importTransformBtn;
    private Button viewBtn;

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
        if (string.IsNullOrEmpty(geometryManager.SelectedTag)) return;

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
            geometryManager.SetSelectedTransform(transformString);
        }
    }

    private void InitializeData()
    {
        importTransformBtn = GameObject.Find(importButtonName).GetComponent<Button>();
        importTransformBtn.onClick.AddListener(ImportTransform);
        viewBtn = GameObject.Find(viewButtonName).GetComponent<Button>();
        viewBtn.onClick.AddListener(ViewButtonClicked);
        geometryManager = new GeometryManager();
        geometryManager.DisplayList();
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

    private void ViewButtonClicked()
    {
        //SceneManager.LoadScene("Sandbox");
    }

    #endregion
}