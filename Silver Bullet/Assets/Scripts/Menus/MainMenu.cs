using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
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
        Debug.Log("Main Menu fired from " + gameObject.name);
        SceneManager.LoadScene(0);
    }
}
