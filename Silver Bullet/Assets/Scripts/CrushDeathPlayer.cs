using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CrushDeathPlayer : MonoBehaviour
{
    public bool gettingCrushed = false;
    public bool grounded = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gettingCrushed == true)
        {
            if (grounded == true)
            {
                Debug.Log("Player Killed fired from " + gameObject.name);
                Debug.Log("Loading Lose screen fired from " + gameObject.name);
                //Feel free to just comment out the Load Scene thing for testing
                SceneManager.LoadScene(2);
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Crush"))
        {
            gettingCrushed = true;

        }

    }
    
    void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Crush"))
        {
            gettingCrushed = false;
            
        }

    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            grounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            grounded = false;
        }
    }
}
