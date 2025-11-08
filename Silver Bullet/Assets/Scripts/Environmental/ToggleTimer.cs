using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTimer : MonoBehaviour
{
    [SerializeField] bool start;
    [SerializeField] float timer;
    [SerializeField] BoxCollider2D target;
    // Start is called before the first frame update
    void Start()
    {
        if (start == true)
        {
            Invoke("On", 0f);
        }
        if (start == false)
        {
            Invoke("Off", 0f);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void On()
    {
        Debug.Log(this + " is On");
        Invoke("Off", timer);
        target.enabled = true;
    }
    
    void Off()
    {
        Debug.Log(this + " is Off");
        Invoke("On", timer);
        target.enabled = false;
    }
}
