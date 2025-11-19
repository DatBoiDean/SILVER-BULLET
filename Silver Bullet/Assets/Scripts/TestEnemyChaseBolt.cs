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
    
    // NEW: Animator reference to control Screwbot's animations (Idle / Walk lean / Sink)
    [SerializeField] Animator anim;  // NEW: drag the Animator here in the Inspector, or it will auto-find in Start

    // NEW: Name of the Trigger parameter in the Animator that plays the sink animation
    [SerializeField] string sinkTriggerName = "Sink"; // NEW: matches your "Sink" trigger in the Screwbot controller

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

        // NEW: If no Animator was assigned in the Inspector, try to grab the one on this GameObject
        if (anim == null)               // NEW
        {                                // NEW
            anim = GetComponent<Animator>(); // NEW: lets this script work as long as an Animator is on the same object
        }                                // NEW
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

            // NEW: Update the isWalking bool in the Animator based on horizontal velocity
            // NEW: When Screwbot is moving left/right, play the lean/glide (Walk) animation; when stopped, go back to Idle
            if (anim != null) // NEW
            {                 // NEW
                bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.01f; // NEW: small threshold to avoid jitter
                anim.SetBool("IsWalking", isMovingHorizontally);              // NEW: uses your "isWalking" bool in Screwbot
            }                 // NEW
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

                        // NEW: When Screwbot sinks / gets stuck, fire the Sink trigger to play the sink animation
                        if (anim != null && !string.IsNullOrEmpty(sinkTriggerName)) // NEW
                        {                                                           // NEW
                            anim.SetTrigger(sinkTriggerName); // NEW: plays the "Sink" animation in the Screwbot controller
                        }                                                           // NEW
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
