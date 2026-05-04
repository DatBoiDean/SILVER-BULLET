using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buttonpress : MonoBehaviour
{
    [SerializeField] GameObject target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
        Destroy(target);
        Destroy(gameObject);
        }
    }
}
