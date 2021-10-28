using Assets.CaseFile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// 0. if starting from new script - must include UnityEngine.UI, think of it as akin to 'tidyverse' on package load (but for UI controls)
using UnityEngine.UI;
using Assets.Geometries;

public class SimpleLoad : MonoBehaviour
{
    // 1. Here we Declare a Button Variable (a C# object) that will act as a handler/delivery tool for a function to the button by assosciating with the function & then 'Listening' for a button click
    public Button SimpleLoadSTL_ButtonVariable;
    public Button SimpleLoadLandmarks_ButtonVariable;

    // 2. Super simple loader - here we create the expected behaviour on button press by means of a function
    void SimpleLoadSTL_Function()
    {
        Debug.Log("You pressed the button!");
        //Assets.Geometries.GeometryManager.LoadMesh(string "Femur", string "femur", string "");
        GeometryManager.Instance.LoadMesh("FemurName", "Femur2", "//360ks.local/Fileserver/KIC_Internal/3.KNEE_RnD/Kanekasu/2021/Aug21/HAL_DP_13119K/HAL_DP_13119K_OA_MA_PS_FB_012_model/HAL_DP_13119K_DistalTibia.stl");
    }

    // 3. But runs on start - assosciating our function and button behaviour by means of the handler/delivery tool (Populating the Button Variable) 
    void Start()
    {
        // Quoted here is the name of the Unity object to attach the C# scripted listener too, doesn't HAVE to be related to the internal C# names at all - we will need to lock a convention
        SimpleLoadSTL_ButtonVariable = GameObject.Find("SimpleLoadSTL").GetComponent<Button>();
        // 3.5 we have just attached Unity GameObject Button <--> Button Variable (the handler/delivery tool), now we are Button Variable <--> Function
        SimpleLoadSTL_ButtonVariable.onClick.AddListener(SimpleLoadSTL_Function);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
