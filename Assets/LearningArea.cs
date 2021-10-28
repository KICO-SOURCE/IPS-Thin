using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LearningArea : MonoBehaviour
{
    string[] myStringArray = new string[5];
    // Start is called before the first frame update

    void Start()
    {
        myStringArray[0] = "Max";
        myStringArray[3] = "Max2";
        myStringArray[2] = "Max1";
        myStringArray[1] = "Max1";
        Debug.Log(myStringArray[0]);

        int dynamicInt = 231231;
        float[] array = new float[dynamicInt];

        // List
        List<Vector3> myList = new List<Vector3>() {
            new Vector3(0,5,0),
            new Vector3(0,32,0),
            new Vector3(0,352,3),
            new Vector3(0,12,0),
            new Vector3(0,45,0),
            new Vector3(0,1,0),
            new Vector3(0,2,0),
            new Vector3(0,6,0),
        };
        myList = myList.OrderBy(o => o.y).ToList();
        foreach (Vector3 v in myList)
        {
            Debug.Log(v);
        }

        Dictionary<int, string> myIntDict = new Dictionary<int, string>() {
            { 3, "String" },
            { 4, "String" },
            { 5, "String3" },
            { 6, "String" },
            { 7, "String2" },
        };
        foreach (KeyValuePair<int, string> pair in myIntDict)
        {
            if (pair.Key == 3)
            {
                Debug.Log(pair.Value);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AnyTimeFunction()
    {
        myStringArray[2] = "Max1";   
    }
}
