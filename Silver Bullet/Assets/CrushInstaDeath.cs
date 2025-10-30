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

    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Crush"))
        {
            gettingCrushed = true;
            //evan make it so that when the COLLIDER is touching the ground its also true, then when both are true the dude explodes violently or smth idk
            
        }

    }
}
