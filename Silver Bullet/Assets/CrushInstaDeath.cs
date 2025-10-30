using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CrushInstaDeath : MonoBehaviour
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
                Destroy(this);
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
