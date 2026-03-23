using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyChaseBolt : MonoBehaviour
{
    [Header("Basic Behavior")]
    [SerializeField] bool canBolt;
    [SerializeField] float gravity;
    [SerializeField] float detectionDist;
    [SerializeField] float waitDist;
    [SerializeField] bool startLeft;
    public GameObject player;
    public float moveSpeed = 3f;
    [SerializeField] float patrolSpeed;
    [SerializeField] float waitTime;

    Rigidbody2D rb;
    private Transform playerTransform;

    [SerializeField] bool isChasing = false;
    [SerializeField] bool Grounded;
    // To track if this dude is on the ground or not

    [SerializeField] bool Stuck;
    // To track if this dude is currently stuck or not

    [SerializeField] CapsuleCollider2D feet;
    Vector2 gravitypull;

    public string patrol;
    public bool waiting = false;
    public Vector2 direction;

    [Header("Animation")]
    // Animator reference for Screwbot's animations
    [SerializeField] Animator anim;

    // SpriteRenderer reference so we can flip along the X axis
    [SerializeField] SpriteRenderer spriteRenderer;

    // Name of the drop animation state in the Animator
    // This state is assumed to be on Base Layer
    [SerializeField] string dropStateName = "DropAnimation";

    [Header("Sink Settings")]
    // How far down the Screwbot should sink after being stomped
    [SerializeField] float sinkDistance = 0.4f;

    // Roughly how long it takes to ease into the sunk position
    [SerializeField] float sinkSmoothTime = 0.15f;

    // Internal values used by SmoothDamp
    float sinkTargetY;
    float sinkVelocityY = 0f;
    bool sinkStarted = false;

    [Header("Per-Enemy Patrol Limits")]
    // These store the patrol limits for THIS specific Screwbot
    float leftLimitX = float.NegativeInfinity;
    float rightLimitX = float.PositiveInfinity;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");

        if (startLeft)
        {
            patrol = "GoLeft";
        }
        else
        {
            patrol = "GoRight";
        }

        gravitypull = Vector2.down * gravity;

        // Auto-find Animator if one was not assigned
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        // Auto-find SpriteRenderer if one was not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        // Only apply custom gravity pull if not grounded and not stuck
        if (Grounded == false && Stuck == false)
        {
            rb.AddForce(gravitypull);
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        // If the Screwbot has been stomped, completely stop movement
        // and smoothly sink into the ground.
        // IMPORTANT: Do NOT keep replaying DropAnimation here,
        // or it will restart every frame and never visibly play through.
        if (Stuck == true)
        {
            rb.velocity = Vector2.zero;

            // Smoothly ease the Screwbot downward into the ground
            if (sinkStarted)
            {
                float newY = Mathf.SmoothDamp(
                    transform.position.y,
                    sinkTargetY,
                    ref sinkVelocityY,
                    sinkSmoothTime
                );

                transform.position = new Vector3(
                    transform.position.x,
                    newY,
                    transform.position.z
                );
            }

            if (anim != null)
            {
                anim.SetBool("isWalking", false);
            }

            return;
        }

        float dist = Vector3.Distance(player.transform.position, transform.position);
        // Measures distance between this object and the player

        // Checks if player is inside this enemy's patrol zone
        bool playerWithinPatrol = true;

        float px = player.transform.position.x;

        // If both patrol limits have been discovered, enforce them
        if (leftLimitX != float.NegativeInfinity && rightLimitX != float.PositiveInfinity)
        {
            float minX = Mathf.Min(leftLimitX, rightLimitX);
            float maxX = Mathf.Max(leftLimitX, rightLimitX);
            playerWithinPatrol = (px >= minX && px <= maxX);
        }

        // === CHASE TOGGLE, NOW ZONE-AWARE ===
        if (!playerWithinPatrol || dist >= detectionDist || dist <= waitDist)
        {
            isChasing = false;
        }

        if (playerWithinPatrol && dist <= detectionDist && dist >= waitDist)
        {
            isChasing = true;
            playerTransform = player.transform;
        }

        if (isChasing && playerTransform != null)
        {
            waiting = false;

            // Always choose left/right based on player position
            float dx = playerTransform.position.x - transform.position.x;

            if (dx < 0f)
            {
                direction = Vector2.left;
                patrol = "GoLeft";
            }
            else
            {
                direction = Vector2.right;
                patrol = "GoRight";
            }

            // Move horizontally while preserving current vertical velocity
            rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
        }
        else
        {
            // Patrol mode
            if (patrol == "GoLeft")
            {
                rb.velocity = new Vector2(-patrolSpeed, rb.velocity.y);
            }

            if (patrol == "GoRight")
            {
                rb.velocity = new Vector2(patrolSpeed, rb.velocity.y);
            }

            if (patrol == "Wait")
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);

                if (waiting == false)
                {
                    if (startLeft)
                    {
                        patrol = "GoRight";
                    }
                    else
                    {
                        patrol = "GoLeft";
                    }
                }
            }
        }

        // If player is too close, stop the Screwbot
        if (dist <= waitDist)
        {
            patrol = "Wait";
            waiting = true;
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }

        // Update walking animation
        if (anim != null)
        {
            bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.01f;
            anim.SetBool("isWalking", isMovingHorizontally);
        }

        // Flip sprite based on movement direction
        if (spriteRenderer != null)
        {
            if (Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                // Assume default art faces right
                spriteRenderer.flipX = rb.velocity.x < 0f;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Grounded = true;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
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
    {
        // If the player feet jump on the Screwbot's head, make it drop
        if (collision.gameObject.CompareTag("PlayerFeet"))
        {
            if (canBolt == true && Grounded == true && Stuck == false)
            {
                if (feet != null)
                {
                    feet.enabled = false;
                }

                Stuck = true;

                // Start the smooth sink from current position down to a target Y
                sinkTargetY = transform.position.y - sinkDistance;
                sinkStarted = true;
                sinkVelocityY = 0f;

                // Stop all leftover movement immediately
                rb.velocity = Vector2.zero;

                // Force walking off and directly play the drop animation ONCE
                if (anim != null)
                {
                    anim.SetBool("isWalking", false);
                    anim.Play("Base Layer." + dropStateName, 0, 0f);
                    Debug.Log("Screwbot directly played DropAnimation on Base Layer");
                }
            }
        }

        // Patrol boundary handling while NOT chasing
        if (isChasing == false)
        {
            if (collision.gameObject.CompareTag("LeftMax"))
            {
                rb.velocity = Vector2.zero;
                patrol = "Wait";
                Invoke(nameof(SwitchToRight), waitTime);
                waiting = true;

                // Store this as this enemy's left patrol limit
                leftLimitX = collision.transform.position.x;
            }

            if (collision.gameObject.CompareTag("RightMax"))
            {
                rb.velocity = Vector2.zero;
                patrol = "Wait";
                Invoke(nameof(SwitchToLeft), waitTime);
                waiting = true;

                // Store this as this enemy's right patrol limit
                rightLimitX = collision.transform.position.x;
            }
        }

        // Patrol boundary handling while chasing
        if (isChasing == true)
        {
            if (collision.gameObject.CompareTag("LeftMax"))
            {
                patrol = "GoRight";
                leftLimitX = collision.transform.position.x;
            }

            if (collision.gameObject.CompareTag("RightMax"))
            {
                patrol = "GoLeft";
                rightLimitX = collision.transform.position.x;
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