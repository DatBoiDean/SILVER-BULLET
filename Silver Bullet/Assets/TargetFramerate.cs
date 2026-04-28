using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class TargetFramerate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 50;
        Screen.SetResolution(1920, 1080, true); 
        //I wonder
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
