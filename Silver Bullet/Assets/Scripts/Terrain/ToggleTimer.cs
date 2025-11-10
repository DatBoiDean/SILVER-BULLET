using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTimer : MonoBehaviour
{
    [SerializeField] bool start;
    [SerializeField] float timer;
    [SerializeField] PolygonCollider2D target;
    [SerializeField] SpriteRenderer target2;
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
        target.enabled = true;
        target2.enabled = true;
        Invoke("Off", timer);
        
    }
    
    void Off()
    {
        Debug.Log(this + " is Off");
        target.enabled = false;
        target2.enabled = false;
        Invoke("On", timer);
        
    }
}
