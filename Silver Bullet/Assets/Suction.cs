using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suction : MonoBehaviour
{
    public GameObject fanHead;
    Rigidbody2D rb;
    [SerializeField] float suctionStrength;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SuctionZone"))
        {
            float speed = suctionStrength * Time.deltaTime;
            rb.velocity = Vector2.MoveTowards(transform.position, fanHead.transform.position, speed);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("SuctionZone"))
        {
            float speed = suctionStrength * Time.deltaTime;
            rb.velocity = Vector2.MoveTowards(transform.position, fanHead.transform.position, speed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
