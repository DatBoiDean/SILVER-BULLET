using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JJsPlaygroundTestSawBot1 : MonoBehaviour
{
    [Header("Detection / Chase")]
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

    [Header("Ground / Patrol Detection")]
    // This is the main collider used to position the patrol raycasts.
    // Assign the SawBot's main body collider in the Inspector.
    [SerializeField] Collider2D bodyCollider;

    // This layer mask should include your solid terrain:
    // ground, walls, tilemap colliders, ledges, etc.
    [SerializeField] LayerMask groundLayer;

    // How far forward the bot checks for a wall directly ahead
    [SerializeField] float wallCheckDistance = 0.15f;

    // How far downward the bot checks for ground ahead
    [SerializeField] float ledgeCheckDistance = 0.35f;

    // How far ahead of the bot the ledge ray begins
    [SerializeField] float ledgeForwardOffset = 0.35f;

    // Small cooldown so the bot does not jitter and rapidly turn every frame
    [SerializeField] float turnCooldown = 0.2f;
    float lastTurnTime = -999f;

    [Header("Attack")]
    // How much damage the SawBot deals when the attack successfully hits the player
    [SerializeField] int damageAmount = 1;

    // This is the trigger collider that becomes active only during the real hit frames
    // Put this on a child object near the saw blade / attack point
    [SerializeField] Collider2D attackHitboxTrigger;

    // This layer mask should include the player's layer
    public LayerMask playerCharacter;

    public string patrol;
    public bool waiting = false;
    public Vector2 direction;
    public GameObject sawBotAttackPoint;
    public float radius;

    [Header("Animation")]
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

    // Exact state name for the real damage-dealing attack animation
    [SerializeField] string attackStateName = "AttackAnimationSaw";

    // Per-enemy patrol zone limits (set when this enemy hits its PatrolLeft/PatrolRight)
    float leftLimitX = float.NegativeInfinity;
    float rightLimitX = float.PositiveInfinity;

    // Tracks which way the Sawbot is facing
    bool facingLeft = false;

    // Prevents one attack animation from damaging the player multiple times
    bool hasHitPlayerThisSwing = false;

    // Tracks whether the attack hit window is currently active
    bool attackHitWindowActive = false;

    // Reusable list for overlap checks during the active attack window
    readonly List<Collider2D> attackOverlapResults = new List<Collider2D>();

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
        else
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

        // If no body collider was assigned in the Inspector,
        // try grabbing one from this GameObject automatically.
        if (bodyCollider == null)
        {
            bodyCollider = GetComponent<Collider2D>();
        }

        // The attack hitbox should start disabled.
        // It is only turned on during the exact strike frames by animation events.
        if (attackHitboxTrigger != null)
        {
            attackHitboxTrigger.enabled = false;
        }
    }

    // FixedUpdate is used here because this script moves a Rigidbody2D
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

        float px = player.transform.position.x;

        // If both limits have been set, enforce the zone
        if (leftLimitX != float.NegativeInfinity && rightLimitX != float.PositiveInfinity)
        {
            float minX = Mathf.Min(leftLimitX, rightLimitX);
            float maxX = Mathf.Max(leftLimitX, rightLimitX);
            playerWithinPatrol = (px >= minX && px <= maxX);
        }

        // === CHASE TOGGLE, NOW ZONE-AWARE ===
        // Order matters here:
        // 1. If player is too close, SawBot stops to rev / attack
        // 2. If player is in chase band, SawBot chases
        // 3. Otherwise, SawBot patrols

        if (dist <= waitDist)
        {
            // Player is close enough for rev / attack behavior
            isChasing = false;
        }
        else if (playerWithinPatrol && dist <= detectionDist)
        {
            // Player is inside the chase zone, but not close enough to attack yet
            isChasing = true;
            playerTransform = player.transform;
        }
        else
        {
            // Player is outside detection zone or outside patrol zone
            isChasing = false;
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
                // Default patrol movement
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
                }

                // This is the main patrol fix:
                // if the SawBot is patrolling and NOT waiting,
                // it checks for a wall ahead or a ledge ahead.
                // If either one is found, it turns around and keeps patrolling.
                if (!waiting && patrol != "Wait" && Time.time >= lastTurnTime + turnCooldown)
                {
                    if (ShouldTurnAround())
                    {
                        ReversePatrolDirection();
                    }
                }
            }

            // Update Sawbot movement bool in the Animator
            if (anim != null)
            {
                bool inRangeNow = dist <= waitDist;
                bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.1f;

                // If the player is close enough to trigger rev / attack behavior,
                // do not keep forcing the walking state.
                if (inRangeNow)
                {
                    isMovingHorizontally = false;
                }

                anim.SetBool(isMovingParam, isMovingHorizontally);
                anim.SetBool(inRangeParam, inRangeNow);
            }

            // Flip sprite based on movement direction so it faces where it's sliding
            if (spriteRenderer != null && Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                // Assume default art faces RIGHT; flip when moving LEFT
                facingLeft = rb.velocity.x < 0f;
                spriteRenderer.flipX = facingLeft;
            }
        }

        // If the player leaves the close-range attack zone,
        // the SawBot must leave its waiting state and become active again.
        if (dist > waitDist)
        {
            waiting = false;
            hasHitPlayerThisSwing = false;
            attackHitWindowActive = false;

            if (attackHitboxTrigger != null)
            {
                attackHitboxTrigger.enabled = false;
            }

            if (playerWithinPatrol && dist <= detectionDist)
            {
                // Resume chasing if the player is still in detection range
                isChasing = true;
                playerTransform = player.transform;
            }
            else
            {
                // Otherwise go back to patrol
                isChasing = false;

                // If the bot got left in Wait state, restore patrol direction
                if (patrol == "Wait")
                {
                    patrol = facingLeft ? "GoLeft" : "GoRight";
                }
            }
        }

        if (dist <= waitDist)
        {
            patrol = "Wait";
            waiting = true;

            // Force horizontal stop when in rev / attack range
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
        //Evan's lazy man's way of making the enemy just not move when theyre too close. i sure hope it actually works

        // If we are in the attack animation and facing LEFT,
        // override the two saw transforms using the flipped coordinates frame-by-frame.
        ApplyFlippedAttackSawPositions();

        // Continuously check if the player Hurtbox is overlapping the active attack hitbox
        // This makes damage independent of whether the player is moving or standing still
        CheckAttackHitboxOverlap();
    }

    // Checks whether the SawBot should turn around while patrolling.
    // It turns if:
    // 1. there is a wall directly ahead
    // 2. there is no ground ahead (ledge / drop)
    bool ShouldTurnAround()
    {
        if (bodyCollider == null)
        {
            return false;
        }

        // Determine which direction the bot is patrolling
        float moveDir = patrol == "GoLeft" ? -1f : 1f;

        Bounds bounds = bodyCollider.bounds;

        // This ray checks for a wall directly in front of the bot
        Vector2 wallRayOrigin = new Vector2(
            moveDir < 0 ? bounds.min.x : bounds.max.x,
            bounds.center.y
        );

        RaycastHit2D wallHit = Physics2D.Raycast(
            wallRayOrigin,
            Vector2.right * moveDir,
            wallCheckDistance,
            groundLayer
        );

        // This ray checks if there is still ground in front of the bot
        Vector2 ledgeRayOrigin = new Vector2(
            bounds.center.x + (ledgeForwardOffset * moveDir),
            bounds.min.y + 0.05f
        );

        RaycastHit2D groundHit = Physics2D.Raycast(
            ledgeRayOrigin,
            Vector2.down,
            ledgeCheckDistance,
            groundLayer
        );

        bool wallAhead = wallHit.collider != null;
        bool noGroundAhead = groundHit.collider == null;

        return wallAhead || noGroundAhead;
    }

    // Reverses patrol direction after hitting a wall or detecting a ledge
    void ReversePatrolDirection()
    {
        lastTurnTime = Time.time;

        if (patrol == "GoLeft")
        {
            patrol = "GoRight";
        }
        else if (patrol == "GoRight")
        {
            patrol = "GoLeft";
        }

        // Reset horizontal velocity so the turn feels cleaner
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    // Called by an Animation Event at the exact frame the blade should become dangerous
    public void BeginAttackHitWindow()
    {
        // Start of a brand-new swing, so allow one fresh hit
        attackHitWindowActive = true;
        hasHitPlayerThisSwing = false;

        if (attackHitboxTrigger != null)
        {
            attackHitboxTrigger.enabled = true;
        }

        Debug.Log("Attack hit window started");
    }

    // Called by an Animation Event when the dangerous strike window ends
    public void EndAttackHitWindow()
    {
        attackHitWindowActive = false;

        if (attackHitboxTrigger != null)
        {
            attackHitboxTrigger.enabled = false;
        }

        Debug.Log("Attack hit window ended");
    }

    // This checks the attack hitbox every physics step while the hit window is active.
    // That way, the player can still get hit while standing still inside the attack range.
    void CheckAttackHitboxOverlap()
    {
        if (!attackHitWindowActive)
        {
            return;
        }

        if (hasHitPlayerThisSwing)
        {
            return;
        }

        if (attackHitboxTrigger == null)
        {
            return;
        }

        if (anim == null)
        {
            return;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        bool inAttackState =
            stateInfo.IsName(attackStateName) ||
            stateInfo.IsName("Base Layer." + attackStateName);

        if (!inAttackState)
        {
            return;
        }

        attackOverlapResults.Clear();

        // Query every collider currently overlapping the active attack hitbox
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(playerCharacter);
        filter.useLayerMask = true;
        filter.useTriggers = true;

        attackHitboxTrigger.OverlapCollider(filter, attackOverlapResults);

        for (int i = 0; i < attackOverlapResults.Count; i++)
        {
            Collider2D hit = attackOverlapResults[i];

            if (hit == null)
            {
                continue;
            }

            // ONLY allow the dedicated player Hurtbox to count for damage
            if (hit.gameObject.name != "Hurtbox")
            {
                continue;
            }

            Debug.Log("SawBot overlap check found Hurtbox.");

            PlayerHealthJJsplayground1 playerHealthComponent =
                hit.GetComponentInParent<PlayerHealthJJsplayground1>();

            if (playerHealthComponent != null)
            {
                Debug.Log("SawBot hit Hurtbox for damage: " + damageAmount);
                playerHealthComponent.PlayerTakeDamage(damageAmount);

                // Only allow one hit per swing
                hasHitPlayerThisSwing = true;
                return;
            }
        }
    }

    void ApplyFlippedAttackSawPositions()
    {
        if (anim == null || leftSawTransform == null || rightSawTransform == null)
        {
            return;
        }

        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        // Only do the frame override while we are actually in the attack state
        if (!stateInfo.IsName(attackStateName) && !stateInfo.IsName("Base Layer." + attackStateName))
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

    private void OnDrawGizmos()
    {
        if (sawBotAttackPoint != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(sawBotAttackPoint.transform.position, radius);
        }

        // Draw patrol detection rays in Scene view so you can tune them easier
        if (bodyCollider != null)
        {
            Bounds bounds = bodyCollider.bounds;
            float moveDir = patrol == "GoLeft" ? -1f : 1f;

            Vector2 wallRayOrigin = new Vector2(
                moveDir < 0 ? bounds.min.x : bounds.max.x,
                bounds.center.y
            );

            Vector2 ledgeRayOrigin = new Vector2(
                bounds.center.x + (ledgeForwardOffset * moveDir),
                bounds.min.y + 0.05f
            );

            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                wallRayOrigin,
                wallRayOrigin + Vector2.right * moveDir * wallCheckDistance
            );

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(
                ledgeRayOrigin,
                ledgeRayOrigin + Vector2.down * ledgeCheckDistance
            );
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Keep patrol boundary logic first
        if (collision.gameObject.CompareTag("LeftMax"))
        {
            if (isChasing == false)
            {
                rb.velocity = Vector2.zero;
                patrol = "Wait";
                Invoke(nameof(SwitchToRight), waitTime);
                waiting = true;

                // Store this patrol edge as this enemy's left patrol limit
                leftLimitX = collision.transform.position.x;
            }
            else
            {
                patrol = "GoRight";
                leftLimitX = collision.transform.position.x;
            }

            return;
        }

        if (collision.gameObject.CompareTag("RightMax"))
        {
            if (isChasing == false)
            {
                rb.velocity = Vector2.zero;
                patrol = "Wait";
                Invoke(nameof(SwitchToLeft), waitTime);
                waiting = true;

                // Store this patrol edge as this enemy's right patrol limit
                rightLimitX = collision.transform.position.x;
            }
            else
            {
                patrol = "GoLeft";
                rightLimitX = collision.transform.position.x;
            }

            return;
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