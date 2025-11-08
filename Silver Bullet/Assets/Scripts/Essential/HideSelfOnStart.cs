using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideSelfOnStart : MonoBehaviour
{
    SpriteRenderer render;
    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<SpriteRenderer>();
        render.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
