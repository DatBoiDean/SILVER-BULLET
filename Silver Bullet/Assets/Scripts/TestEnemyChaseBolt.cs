using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyChaseBolt : MonoBehaviour
{
    [SerializeField] float detectionDist;
    public GameObject player;
    public float moveSpeed = 3f;
    public Rigidbody2D rb;
    private Transform playerTransform;
    [SerializeField] bool isChasing = false;
    [SerializeField] bool Grounded;
    //To track if this dude is on the ground or not
    [SerializeField] bool Stuck;
    //To track if this dude is currently stuck or not
    [SerializeField] CircleCollider2D feet;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
    }

    void FixedUpdate()
    {

        float dist = Vector3.Distance(player.transform.position, transform.position);
        //Measures distance between this object and the player.
        //print("Dist:" + dist);
        //ONLY enable this line of code for debugging!!!
        if (Stuck == false)
        {
            if (isChasing && playerTransform != null)
            {
                // Calculate the direction vector from the enemy to the player
                Vector2 direction = (playerTransform.position - transform.position).normalized;

                // Apply a force or set a velocity to move the enemy
                // For simple movement, setting the velocity is most direct
                rb.velocity = direction * moveSpeed;
            }
            else
            {
                // Stop moving if not chasing
                rb.velocity = Vector2.zero;
            }
        }

        if (dist >= detectionDist)
            //If Dist is greater than detection dist...
            {
                isChasing = false;
                //Stop chasing
            }

            if (dist <= detectionDist)
            //If dist is less than detection distance..
            {
            isChasing = true;
            playerTransform = player.transform;
                //Start chasing.
            }
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
    //changed to a trigger effect since it could mess with things
    //  when I was making it all trigger based.
    {
        if (collision.gameObject.CompareTag("PlayerFeet"))
        {
            if (Grounded == true) //If the thing is grounded...
            {
                if (Stuck == false) //... AND isn't already stuck...
                //This is to basically maximize the odds of Unity not continuously firing off these scripts
                //  if it's already done the thing this script needs to do
                {
                    feet.enabled = false;
                    Stuck = true;
                    //Find and disable the Circle Collider
                    //Find and disable the movement script (external?)
                }
            }
        }
    }
}

