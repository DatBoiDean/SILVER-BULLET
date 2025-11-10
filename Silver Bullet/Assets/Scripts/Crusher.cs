using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crusher : MonoBehaviour
{
    [SerializeField] bool Crushing = false;
    [SerializeField] float crushSpeed;
    [SerializeField] float returnSpeed;
    public float ypos;
    // Start is called before the first frame update
    void Start()
    {
        ypos = transform.position.y - 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (Crushing == true)
        {
            if (transform.position.y > ypos)
            {
                transform.Translate(0, crushSpeed * -1f, 0);
            }
            if (transform.position.y < ypos)
            {
                Destroy(gameObject,0f);
            }
        }
        // if (Crushing == false)
        // {
        //     if (transform.position.y < ypos)
        //     {
        //         transform.Translate(0, returnSpeed, 0);
        //     }
        //     if(transform.position.y > ypos)
        //     {
        //         transform.Translate(0, returnSpeed, 0);
        //     }
        // }
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
