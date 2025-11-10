using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinDetection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D (Collider2D other)
    {
        if (other.gameObject.CompareTag("Win"))
        {
            Debug.Log("Player reached Win trigger ");
                Debug.Log("Loading Win screen fired from " + gameObject.name);
                //Feel free to just comment out the Load Scene thing for testing
                SceneManager.LoadScene(1);
        }
    }
}
