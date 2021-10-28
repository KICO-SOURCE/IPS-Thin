using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//p_load(tidyverse)
//p_load(lubridate)
//p_load(fs)
//p_load(readxl)
//p_load(rgl)
//p_load(Rvcg)
//p_load(Morpho)
//p_load(sjmisc)

public class TEST : MonoBehaviour
{
    string[] LandmarkExclusionList = new string[]{
        "femCMPImportREF",
        "tibCMPImportREF",
        "patCMPImportREF",
        "FemurHead2CTTransform",
        "FemurStem2CTTransform",
        "FemurComponentCT2APPTransform",
        "FemurComponentCT2SETransform",
        "FemurComponentCT2STTransform",
        "FemurComponentCT2SUPTransform",
        "Cup2CTTransform",
        "Liner2CTTransform",
        "CupComponentCT2APPTransform",
        "CupComponentCT2SETransform",
        "CupComponentCT2STTransform",
        "CupComponentCT2SUPTransform",
        "femBoneExtensionDistractedREF",
        "femBoneFlexionDistractedREF",
        "femBoneExtensionFunctionalREF",
        "femBoneFlexionFunctionalREF",
        "tibBoneExtensionDistractedREF",
        "tibBoneFlexionDistractedREF",
        "tibBoneExtensionFunctionalREF",
        "tibBoneFlexionFunctionalREF"
    };


    // Start is called before the first frame update
    // break, continue, while
    void Start()
    {
        foreach(string name in LandmarkExclusionList) {
            if(name.ToLower().Contains("bone")){
                Debug.Log("Found a bone!");
                break;
            }
            if (name.ToLower().Contains("bone"))
            {
                Debug.Log("Found a bone!");
                continue;
            }
        }

        for (int i = 0; i < 10; i++)
        {
            Debug.Log(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
