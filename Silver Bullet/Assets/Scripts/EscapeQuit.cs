using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 
public class EscapeQuit : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape Key Pressed, fired from " + gameObject.name);
            Debug.Log("Loading Main Menu, fired from "+ gameObject.name);
            SceneManager.LoadScene(0);
            //Debug.Log("Quitting Application, fired from " + gameObject.name);
            //Application.Quit();
        }
    }
}
