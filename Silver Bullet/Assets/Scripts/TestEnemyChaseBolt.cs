using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyChaseBolt : MonoBehaviour
{
    [SerializeField] float detectionDist;
    [SerializeField] bool startLeft;
    public GameObject player;
    public float moveSpeed = 3f;
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitTime;
    public Rigidbody2D rb;
    private Transform playerTransform;
    [SerializeField] bool isChasing = false;
    [SerializeField] bool Grounded;
    //To track if this dude is on the ground or not
    [SerializeField] bool Stuck;
    //To track if this dude is currently stuck or not
    [SerializeField] CircleCollider2D feet;

    public string patrol;
    public bool waiting = false;
    public Vector2 direction;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        if (startLeft == true)
        {
            patrol = "GoLeft";
        }
        if (startLeft == false)
        {
            patrol = "GoRight";
        }
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
                waiting = false;
                // Calculate the direction vector from the enemy to the player
                // Vector2 direction = (playerTransform.position - transform.position).normalized;



                //New approach, made it so it detects where the player's x position is and acts accordingly.
                //This also has a bonus of letting the thing patrol in the same direction as the player, if the player
                //shakes them off.
                if (player.transform.position.x < gameObject.transform.position.x)
                {
                    direction = Vector2.left;
                    patrol = "GoLeft";
                }

                if (player.transform.position.x > gameObject.transform.position.x)
                {
                    direction = Vector2.right;
                    patrol = "GoRight";
                }

                if (player.transform.position.x == gameObject.transform.position.x)
                {
                    direction = Vector2.zero;
                }

                
                // Apply a force or set a velocity to move the enemy
                // For simple movement, setting the velocity is most direct
                //Moved this to after the if statements
                rb.velocity = direction * moveSpeed;

            }
            else
            {
                //StopChase();
                //gonna need some more time to think about how to do patrols
                rb.velocity = Vector2.zero;
                // Stop moving if not chasing
                if (patrol == "GoLeft")
                {
                    rb.velocity = Vector2.left * patrolSpeed;
                }

                if (patrol == "GoRight")
                {
                    rb.velocity = Vector2.right * patrolSpeed;
                }

                if (patrol == "Wait" && waiting == false)
                {
                    if (startLeft == true)
                    {
                        patrol = "GoRight";
                    }
                    if (startLeft == false)
                    {
                        patrol = "GoLeft";
                    }
                }
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
    
    // void StopChase()
    // {
    //     rb.velocity = Vector2.zero;
    // }

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
        if (isChasing == false)
        {
            if (collision.gameObject.CompareTag("LeftMax"))
            {
                rb.velocity = Vector2.zero;
                patrol = "Wait";
                Invoke("SwitchToRight", waitTime);
                waiting = true;
            }
            if (collision.gameObject.CompareTag("RightMax"))
            {
                rb.velocity = Vector2.zero;
                patrol = "Wait";
                Invoke("SwitchToLeft", waitTime);
                waiting = true;
            }
        }

        if (isChasing == true)
        {
            if (collision.gameObject.CompareTag("LeftMax"))
            {
                patrol = "GoRight";
            } 

            if (collision.gameObject.CompareTag("RightMax"))
            {
                patrol = "GoLeft";
            }
        }
    }

    void SwitchToLeft()
    {
        patrol = "GoLeft";
        waiting = false;
    }
    
    void SwitchToRight()
    {
        patrol = "GoRight";
        waiting = false;
    }
}

