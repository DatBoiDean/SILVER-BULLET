using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ScrewFloor : MonoBehaviour
{
    [SerializeField] bool Grounded;
    [SerializeField] bool Stuck;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = true;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = false;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Grounded == true) //If the thing is grounded...
            {
                if (Stuck == false) //... AND isn't already stuck...
                //This is to basically maximize the odds of Unity not continuously firing off these scripts
                //  if it's already done the thing this script needs to do
                { 
                    //Find and disable the Circle Collider
                    //Find and disable the movement script (external?)
                }
            }
        }
    }
}
