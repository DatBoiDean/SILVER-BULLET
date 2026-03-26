using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanHeadMovement : MonoBehaviour
{
    [SerializeField] float detectionDist;
    [SerializeField] float waitDist;
    [SerializeField] bool startLeft;
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitTime;
    [SerializeField] bool isChasing = false;
    Rigidbody2D rb;

    public GameObject player;
    public float moveSpeed = 3f;
    public string patrol;
    public bool waiting = false;
    public Vector2 direction;
    private Transform playerTransform;

    [SerializeField] Animator anim;
    [SerializeField] SpriteRenderer spriteRenderer;

    float leftLimitX = float.NegativeInfinity;   // NEW: X of PatrolLeft / LeftMax for THIS enemy
    float rightLimitX = float.PositiveInfinity;  // NEW: X of PatrolRight / RightMax for THIS enemy

    float moveDirection;

    bool isFacingRight = true;
    bool flip;

    // Start is called before the first frame update
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

        // NEW: If no Animator was assigned in the Inspector, try to grab the one on this GameObject
        if (anim == null)
        {
            anim = GetComponent<Animator>(); // NEW: lets this script work as long as an Animator is on the same object
        }

        // NEW: Auto-grab SpriteRenderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float dist = Vector3.Distance(player.transform.position, transform.position);
        //Measures distance between this object and the player.
        //print("Dist:" + dist);
        //use this for debugging

        // NEW: Checks if player is inside enemypatrol zone (between its left/right limits)
        bool playerWithinPatrol = true; // NEW: default true until we know our limits
                                        //what does "until we know our limits" mean???
        if (player != null)
        {
            float px = player.transform.position.x; // NEW

            // NEW: if both limits have been set, enforce the zone
            if (leftLimitX != float.NegativeInfinity && rightLimitX != float.PositiveInfinity) // NEW
            {
                float minX = Mathf.Min(leftLimitX, rightLimitX); // NEW: just in case they get swapped
                float maxX = Mathf.Max(leftLimitX, rightLimitX);
                playerWithinPatrol = (px >= minX && px <= maxX); // NEW: true if player X is within our patrol limits
            }
        }

        if (isChasing && playerTransform != null)
        {
            waiting = false;
            // Calculate the direction vector from the enemy to the player
            // Vector2 direction = (playerTransform.position - transform.position).normalized;



            //New approach, made it so it detects where the player's x position is and acts accordingly.
            //This also has a bonus of letting the thing patrol in the same direction as the player, if the player
            //shakes them off.

            // CHANGED: always choose left/right based on player position (no dead zone)
            float dx = playerTransform.position.x - transform.position.x; // player - enemy X offset

            if (dx < 0f) // player is to the left
            {
                direction = Vector2.left;
                patrol = "GoLeft";
            }
            else // player is to the right (or exactly aligned)
            {
                direction = Vector2.right;
                patrol = "GoRight";
            }

            // Apply a force or set a velocity to move the enemy
            // For simple movement, setting the velocity is most direct
            //Moved this to after the if statements

            // Keep smooth horizontal slide while preserving vertical velocity
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
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
            anim.SetBool("isWalking", isMovingHorizontally);              // NEW: uses your "isWalking" bool in Screwbot
        }                 // NEW

        // NEW: Flip sprite based on movement direction so it faces where it's sliding
        if (spriteRenderer != null) // NEW
        {                           // NEW
            if (Mathf.Abs(rb.velocity.x) > 0.01f) // NEW: only flip when actually moving
            {
                // NEW: Assume default art faces RIGHT; flip when moving LEFT
                //spriteRenderer.flipX = rb.velocity.x > 0f; // NEW
            }
        }                           // NEW
                                    //Do we really need to list every addition as "new"?

        if (!playerWithinPatrol || dist >= detectionDist || dist <= waitDist)
        //If Dist is greater than detection dist OR player is outside this enemy's patrol zone...
        {
            isChasing = false;
            //Stop chasing
        }

        if (playerWithinPatrol && dist <= detectionDist && dist >= waitDist)
        //If dist is less than detection distance AND player is inside this enemy's patrol zone..
        {
            isChasing = true;
            playerTransform = player.transform;

            Vector3 scale = transform.localScale;

            if (player.transform.position.x > transform.position.x) //check if player's position is greater than enemy's position
            {
                scale.x = Mathf.Abs(scale.x) * (flip ? -1 : 1);
                transform.Translate(moveSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                scale.x = Mathf.Abs(scale.x) * -1 * (flip ? -1 : 1);
                transform.Translate(moveSpeed * Time.deltaTime * -1, 0, 0);
            }

            transform.localScale = scale;
            //Start chasing.
        }

        if (dist >= waitDist)
        {
            if (isChasing == true)
            {
                waiting = false;
            }
            if (isChasing == false)
            {
                if (patrol == "GoLeft")
                {

                }
            }
        }

        if (dist <= waitDist)
        {
            patrol = "Wait";
            waiting = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
        FlipCharacter();
    }

    void SwitchToRight()
    {
        patrol = "GoRight";
        waiting = false;
        FlipCharacter();
    }

    void FlipCharacter() // have character face left or right depending on input
    {
        isFacingRight = !isFacingRight;

        Vector2 currentScale = transform.localScale; // get current scale of character
        currentScale.x = -currentScale.x; // flip scale of character
        transform.localScale = currentScale; // set new scale
    }
}
