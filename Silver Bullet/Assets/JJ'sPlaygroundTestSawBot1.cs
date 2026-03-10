using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJsPlaygroundTestSawBot1 : MonoBehaviour
{
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

    //To track if this dude is on the ground or not
    [SerializeField] bool Stuck;

    //To track if this dude is currently stuck or not
    [SerializeField] CapsuleCollider2D feet;

    //Controls the delay before SawBot attacks
    [SerializeField] float sawDelay;

    public string patrol;
    public bool waiting = false;
    public Vector2 direction;
    public GameObject sawBotAttackPoint;
    public float radius;
    public LayerMask playerCharacter;

    // Animator reference for Sawbot's animation controller
    [SerializeField] Animator anim;

    // SpriteRenderer reference so we can flip along the X axis to face the player
    [SerializeField] SpriteRenderer spriteRenderer;

    // These are the TWO saw objects.
    // Secondary Blade = left blade
    // Primary = right blade
    [SerializeField] Transform leftSawTransform;   // Secondary Blade
    [SerializeField] Transform rightSawTransform;  // Primary

    // Animator parameter names for the Sawbot controller
    [SerializeField] string isMovingParam = "IsMoving";
    [SerializeField] string inRangeParam = "InRange";
    [SerializeField] string attackTriggerParam = "Attack";

    // Name of the attack state in the Animator
    [SerializeField] string attackStateName = "AttackAnimationSaw";

    // Per-enemy patrol zone limits (set when this enemy hits its PatrolLeft/PatrolRight)
    float leftLimitX = float.NegativeInfinity;
    float rightLimitX = float.PositiveInfinity;

    // Prevents SawAttack from being invoked every FixedUpdate while player is in waitDist
    bool attackScheduled = false;

    // Tracks which way the Sawbot is facing
    bool facingLeft = false;

    // These are the ORIGINAL right-facing attack coordinates you gave me.
    // Right blade = Primary
    // Left blade = Secondary Blade
    readonly Vector3[] rightBladeAttackRightFacing =
    {
        new Vector3(-0.43f,  0.86f, 0f), // Frame 1
        new Vector3(-0.43f,  0.86f, 0f), // Frame 2 hold
        new Vector3(-0.07f,  1.00f, 0f), // Frame 3
        new Vector3( 0.52f, -0.13f, 0f), // Frame 4
        new Vector3( 0.52f, -0.13f, 0f), // Frame 5 hold
        new Vector3( 0.16f, -0.34f, 0f), // Frame 6
        new Vector3( 0.16f, -0.34f, 0f)  // Frame 7 hold
    };

    readonly Vector3[] leftBladeAttackRightFacing =
    {
        new Vector3(-0.22f,  0.88f, 0f), // Frame 1
        new Vector3(-0.22f,  0.88f, 0f), // Frame 2 hold
        new Vector3(-0.04f,  1.06f, 0f), // Frame 3
        new Vector3( 0.59f, -0.02f, 0f), // Frame 4
        new Vector3( 0.59f, -0.02f, 0f), // Frame 5 hold
        new Vector3( 0.26f, -0.29f, 0f), // Frame 6
        new Vector3( 0.26f, -0.29f, 0f)  // Frame 7 hold
    };

    // These are the FLIPPED left-facing attack coordinates I converted for you.
    readonly Vector3[] rightBladeAttackLeftFacing =
    {
        new Vector3( 0.43f,  0.86f, 0f), // Frame 1
        new Vector3( 0.43f,  0.86f, 0f), // Frame 2 hold
        new Vector3( 0.07f,  1.00f, 0f), // Frame 3
        new Vector3(-0.52f, -0.13f, 0f), // Frame 4
        new Vector3(-0.52f, -0.13f, 0f), // Frame 5 hold
        new Vector3(-0.16f, -0.34f, 0f), // Frame 6
        new Vector3(-0.16f, -0.34f, 0f)  // Frame 7 hold
    };

    readonly Vector3[] leftBladeAttackLeftFacing =
    {
        new Vector3( 0.22f,  0.88f, 0f), // Frame 1
        new Vector3( 0.22f,  0.88f, 0f), // Frame 2 hold
        new Vector3( 0.04f,  1.06f, 0f), // Frame 3
        new Vector3(-0.59f, -0.02f, 0f), // Frame 4
        new Vector3(-0.59f, -0.02f, 0f), // Frame 5 hold
        new Vector3(-0.26f, -0.29f, 0f), // Frame 6
        new Vector3(-0.26f, -0.29f, 0f)  // Frame 7 hold
    };

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");

        if (startLeft == true)
        {
            patrol = "GoLeft";
            facingLeft = true;
        }

        if (startLeft == false)
        {
            patrol = "GoRight";
            facingLeft = false;
        }

        // If no Animator was assigned in the Inspector, try to grab the one on this GameObject
        if (anim == null)
        {
            anim = GetComponent<Animator>();
        }

        // Auto-grab SpriteRenderer if not assigned
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        float dist = Vector3.Distance(player.transform.position, transform.position);
        //Measures distance between this object and the player.
        //print("Dist:" + dist);
        //use this for debugging

        // Checks if player is inside enemy patrol zone (between its left/right limits)
        bool playerWithinPatrol = true;
        // default true until we know our limits

        if (player != null)
        {
            float px = player.transform.position.x;

            // if both limits have been set, enforce the zone
            if (leftLimitX != float.NegativeInfinity && rightLimitX != float.PositiveInfinity)
            {
                float minX = Mathf.Min(leftLimitX, rightLimitX);
                float maxX = Mathf.Max(leftLimitX, rightLimitX);
                playerWithinPatrol = (px >= minX && px <= maxX);
            }
        }

        if (Stuck == false)
        {
            if (isChasing && playerTransform != null)
            {
                waiting = false;

                // New approach, made it so it detects where the player's x position is and acts accordingly.
                // This also has a bonus of letting the thing patrol in the same direction as the player, if the player
                // shakes them off.

                // always choose left/right based on player position (no dead zone)
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

                // Keep smooth horizontal slide while preserving vertical velocity
                rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);
            }
            else
            {
                //StopChase();
                //gonna need some more time to think about how to do patrols
                rb.velocity = new Vector2(0f, rb.velocity.y);
                // Stop moving if not chasing

                if (patrol == "GoLeft")
                {
                    rb.velocity = new Vector2(-patrolSpeed, rb.velocity.y);
                }

                if (patrol == "GoRight")
                {
                    rb.velocity = new Vector2(patrolSpeed, rb.velocity.y);
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

            // Update Sawbot movement bool in the Animator
            if (anim != null)
            {
                bool inRangeNow = dist <= waitDist;
                bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.1f;

                // If the player is close enough to trigger attack/rev behavior,
                // do not keep forcing the walking state.
                if (inRangeNow)
                {
                    isMovingHorizontally = false;
                }

                anim.SetBool(isMovingParam, isMovingHorizontally);
                anim.SetBool(inRangeParam, inRangeNow);
            }

            // Flip sprite based on movement direction so it faces where it's sliding
            if (spriteRenderer != null)
            {
                if (Mathf.Abs(rb.velocity.x) > 0.01f)
                {
                    // Assume default art faces RIGHT; flip when moving LEFT
                    facingLeft = rb.velocity.x < 0f;
                    spriteRenderer.flipX = facingLeft;
                }
            }
        }

        // === CHASE TOGGLE, NOW ZONE-AWARE ===

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
                    // left as-is from original code
                }
            }

            // Player is no longer in attack range, so reset scheduled attack
            attackScheduled = false;
        }

        if (dist <= waitDist)
        {
            patrol = "Wait";
            waiting = true;

            // Optional: force horizontal stop when in attack range
            rb.velocity = new Vector2(0f, rb.velocity.y);

            // Only schedule attack once while player remains in range
            if (!attackScheduled)
            {
                Invoke("SawAttack", sawDelay);
                attackScheduled = true;
            }
        }
        //Evan's lazy man's way of making the enemy just not move when theyre too close. i sure hope it actually works

        // Update Animator "InRange" after chase/attack logic is decided
        if (anim != null)
        {
            bool inRange = dist <= waitDist;
            anim.SetBool(inRangeParam, inRange);

            // Optional: if you want the Attack trigger to fire when player first gets into attack range
            if (inRange && attackScheduled)
            {
                // This can be used if your Animator has an Attack trigger transition
                // Comment this out if you are using only InRange + Exit Time transitions
                // anim.SetTrigger(attackTriggerParam);
            }
        }

        // This is the main fix:
        // if we are in the attack animation and facing LEFT,
        // override the two saw transforms using the flipped coordinates frame-by-frame.
        ApplyFlippedAttackSawPositions();
    }

    void ApplyFlippedAttackSawPositions()
    {
        if (anim == null || leftSawTransform == null || rightSawTransform == null)
        {
            return;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Only do the frame override while we are actually in the attack state
        if (!stateInfo.IsName(attackStateName))
        {
            return;
        }

        // If facing right, let the original animation keyframes do their normal job
        if (!facingLeft)
        {
            return;
        }

        // Attack clip has 7 keyed frames in this setup
        int totalFrames = 7;

        // normalizedTime keeps increasing on loops, so use only the fractional cycle part
        float normalized = stateInfo.normalizedTime % 1f;

        int frameIndex = Mathf.FloorToInt(normalized * totalFrames);

        if (frameIndex < 0)
        {
            frameIndex = 0;
        }

        if (frameIndex >= totalFrames)
        {
            frameIndex = totalFrames - 1;
        }

        // Secondary Blade = left blade
        leftSawTransform.localPosition = leftBladeAttackLeftFacing[frameIndex];

        // Primary = right blade
        rightSawTransform.localPosition = rightBladeAttackLeftFacing[frameIndex];
    }

    private void SawAttack()
    {
        // Once the attack actually fires, allow a future attack to be scheduled again
        attackScheduled = false;

        if (sawBotAttackPoint == null)
        {
            Debug.LogWarning("SawBot attack point is missing on " + gameObject.name);
            return;
        }

        Collider2D[] playerHits = Physics2D.OverlapCircleAll(sawBotAttackPoint.transform.position, radius, playerCharacter);

        foreach (Collider2D playerGameObject in playerHits)
        {
            Debug.Log("SawBot Attacks");

            if (playerGameObject.CompareTag("Player"))
            {
                var playerHealthComponent = playerGameObject.GetComponent<PlayerHealth>();

                if (playerHealthComponent != null)
                {
                    playerHealthComponent.PlayerTakeDamage(1);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (sawBotAttackPoint != null)
        {
            Gizmos.DrawWireSphere(sawBotAttackPoint.transform.position, radius);
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

                // Store this patrol edge as this enemy's left patrol limit
                leftLimitX = collision.transform.position.x;
            }

            if (collision.gameObject.CompareTag("RightMax"))
            {
                rb.velocity = Vector2.zero;
                patrol = "Wait";
                Invoke("SwitchToLeft", waitTime);
                waiting = true;

                // Store this patrol edge as this enemy's right patrol limit
                rightLimitX = collision.transform.position.x;
            }
        }

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