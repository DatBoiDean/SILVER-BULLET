using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField] bool Crushing = false;
    public float ypos;
    // Start is called before the first frame update
    void Start()
    {
        ypos = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if (Crushing == true)
        {
            transform.Translate(0, -0.75f, 0);
        }
        if (Crushing == false)
        {
            if (transform.position.y < ypos)
            {
                transform.Translate(0, 0.25f, 0);
            }
            if(transform.position.y > ypos)
            {
                transform.Translate(0, 0.25f, 0);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (Crushing == false)
        {
            Crushing = true;
            Invoke("StopCrush", 1f);
        }
    }
    void StopCrush()
    {
        Crushing = false;
    }
}
