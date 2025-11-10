using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamFling : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float steamFling;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SteamUp"))
        {
            rb.velocity = Vector2.up * steamFling;
        }
    }
    
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag ("SteamUp"))
        {
            rb.velocity = Vector2.up * steamFling;
        }
    }
}
