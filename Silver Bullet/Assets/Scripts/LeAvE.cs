using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeAvE : MonoBehaviour
{
    public Button button;
    // Start is called before the first frame update
    void Start()
    {
        Button buttonbutton = gameObject.GetComponent<Button>();
        buttonbutton.onClick.AddListener(Buttoned);
        //I have decided to pursue 
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    void Buttoned()
    {
        Debug.Log("Quit Function fired from " + gameObject.name);
        Application.Quit();
    }
}
