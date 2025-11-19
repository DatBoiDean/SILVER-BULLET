using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyChaseBolt : MonoBehaviour
{
    [SerializeField] bool canBolt;
    [SerializeField] float gravity;
    Vector2 gravitypull;
    [SerializeField] float detectionDist;
    [SerializeField] bool startLeft;
    public GameObject player;
    public float moveSpeed = 3f;
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitTime;
    Rigidbody2D rb;
    private Transform playerTransform;
    [SerializeField] bool isChasing = false;
    [SerializeField] bool Grounded;
    //To track if this dude is on the ground or not
    [SerializeField] bool Stuck;
    //To track if this dude is currently stuck or not
    [SerializeField] CapsuleCollider2D feet;

    public string patrol;
    public bool waiting = false;
    public Vector2 direction;

    // NEW: Reference to the Animator that controls Screwbot's animations (Idle, lean-walk, Sink)
    [SerializeField] Animator anim;
    // NEW: Name of the Trigger in the Animator that plays the sink animation (you use "Sink")
    [SerializeField] string sinkTriggerName = "Sink";

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
        gravitypull = Vector2.down * gravity;

        // NEW: Auto-grab Animator if not assigned in Inspector (as long as Animator is on this object)
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }
    }

    void Update()
    {
        rb.AddForce(gravitypull);
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

                // UPDATED: Use a horizontal distance value with a small "dead zone" to avoid jitter when very close
                float dx = playerTransform.position.x - transform.position.x; // NEW: horizontal offset to player

                if (Mathf.Abs(dx) < 0.1f) // NEW: if almost exactly on top horizontally, stop sliding to prevent flip-flopping
                {
                    direction = Vector2.zero; // NEW: no movement when close enough
                }
                else if (dx < 0f) // NEW: player is to the left
                {
                    direction = Vector2.left;
                    patrol = "GoLeft";
                }
                else // NEW: player is to the right
                {
                    direction = Vector2.right;
                    patrol = "GoRight";
                }

                // Apply a force or set a velocity to move the enemy
                // For simple movement, setting the velocity is most direct
                //Moved this to after the if statements
                rb.velocity = direction * moveSpeed; // CHASE: slide towards player using moveSpeed
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

            // NEW: Drive the isWalking bool in the Animator based on horizontal velocity
            // NEW: When Screwbot is moving left/right, play the lean/glide "Walk" animation; when stopped, go Idle
            if (anim != null)
            {
                bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.01f; // NEW: small threshold avoids micro-jitter
                anim.SetBool("IsWalking", isMovingHorizontally);              // NEW: uses your "isWalking" bool parameter
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
            if (canBolt == true)
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

                        // NEW: When the enemy gets bolted into the ground, play the Sink animation once
                        if (anim != null && !string.IsNullOrEmpty(sinkTriggerName))
                        {
                            anim.SetTrigger(sinkTriggerName); // NEW: fires the "Sink" Trigger on the Screwbot Animator
                        }
                    }
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
