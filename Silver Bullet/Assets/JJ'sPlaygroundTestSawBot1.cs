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

    [Header("Stuck Turn Detection")]
    // If the SawBot's X position does not change enough for this many seconds
    // while it is trying to patrol, it will force a turn around.
    [SerializeField] float stuckTurnTime = 3f;

    // Tiny X movement threshold so tiny physics wiggles do not count as real movement.
    [SerializeField] float stuckMinXChange = 0.02f;

    // Tracks how long the bot has been trying to move without actually changing X enough.
    float stuckTurnTimer = 0f;

    // Last recorded X position used by the stuck-turn check.
    float lastRecordedX;

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

        // Match the starting visual direction.
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingLeft;
        }

        // Store the starting X position for the stuck-turn backup check.
        lastRecordedX = transform.position.x;

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
                    facingLeft = true;
                }
                else // player is to the right (or exactly aligned)
                {
                    direction = Vector2.right;
                    patrol = "GoRight";
                    facingLeft = false;
                }

                // Keep the sprite synced with the chase direction.
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = facingLeft;
                }

                // Keep smooth horizontal slide while preserving vertical velocity
                rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

                // Reset the backup stuck timer while chasing so patrol logic does not
                // accidentally force a turn during chase behavior.
                ResetStuckTurnCheck();
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

                // Normal patrol turning now comes from the LeftMax and RightMax boundary triggers.
                // This backup check only exists so the bot can recover if it gets jammed on something
                // and never reaches one of those patrol boundary objects.
                HandleStuckTurnCheck();
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

            // Once the SawBot is close enough to stop moving,
            // do not leave it stuck facing its last patrol direction.
            // Explicitly turn it toward the player's current X position.
            FacePlayer();

            // Force horizontal stop when in rev / attack range
            rb.velocity = new Vector2(0f, rb.velocity.y);

            // While sitting in close-range attack mode, clear the patrol stuck timer.
            ResetStuckTurnCheck();
        }
        //Evan's lazy man's way of making the enemy just not move when theyre too close. i sure hope it actually works

        // If we are in the attack animation and facing LEFT,
        // override the two saw transforms using the flipped coordinates frame-by-frame.
        ApplyFlippedAttackSawPositions();

        // Continuously check if the player Hurtbox is overlapping the active attack hitbox
        // This makes damage independent of whether the player is moving or standing still
        CheckAttackHitboxOverlap();
    }

    // This backup check watches whether the bot is actually changing X while patrolling.
    // If it is trying to move but stays in nearly the same spot for too long,
    // force a turn so it can recover from getting hung up on something.
    void HandleStuckTurnCheck()
    {
        // Only use this during active patrol movement, not while waiting or not moving.
        if (waiting || patrol == "Wait")
        {
            ResetStuckTurnCheck();
            return;
        }

        float currentX = transform.position.x;
        float xDifference = Mathf.Abs(currentX - lastRecordedX);
        bool tryingToPatrolMove = Mathf.Abs(rb.velocity.x) > 0.01f;

        if (!tryingToPatrolMove)
        {
            ResetStuckTurnCheck();
            return;
        }

        if (xDifference < stuckMinXChange)
        {
            stuckTurnTimer += Time.fixedDeltaTime;

            if (stuckTurnTimer >= stuckTurnTime)
            {
                // This forced turn is only a fallback for a stuck bot.
                // Normal patrol turns should still come from LeftMax / RightMax.
                ReversePatrolDirection();

                // Reset again after forcing the turn so it does not instantly flip back.
                ResetStuckTurnCheck();
                lastRecordedX = transform.position.x;
            }
        }
        else
        {
            // The bot moved enough on X, so clear the timer and store this new position.
            stuckTurnTimer = 0f;
            lastRecordedX = currentX;
        }
    }

    // Central reset helper for the backup stuck-turn timer.
    void ResetStuckTurnCheck()
    {
        stuckTurnTimer = 0f;
        lastRecordedX = transform.position.x;
    }

    // Reverses patrol direction as a backup recovery when the bot gets stuck.
    void ReversePatrolDirection()
    {
        if (patrol == "GoLeft")
        {
            patrol = "GoRight";
            facingLeft = false;
        }
        else if (patrol == "GoRight")
        {
            patrol = "GoLeft";
            facingLeft = true;
        }

        // Keep the sprite art lined up after an automatic patrol turn.
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingLeft;
        }

        // Reset horizontal velocity so the turn feels cleaner
        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    // When the SawBot is close enough to stop moving,
    // this keeps it visually locked onto the player instead of the old patrol direction.
    void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        float dx = player.transform.position.x - transform.position.x;

        // If the player is almost perfectly centered, keep the current facing
        // so the sprite does not jitter left and right.
        if (Mathf.Abs(dx) < 0.01f)
        {
            return;
        }

        facingLeft = dx < 0f;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingLeft;
        }
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
                facingLeft = false;

                // Keep the sprite art correct after hitting the left boundary.
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = facingLeft;
                }

                leftLimitX = collision.transform.position.x;
            }

            // The enemy definitely reached a new patrol point, so clear the stuck backup timer.
            ResetStuckTurnCheck();
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
                facingLeft = true;

                // Keep the sprite art correct after hitting the right boundary.
                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = facingLeft;
                }

                rightLimitX = collision.transform.position.x;
            }

            // The enemy definitely reached a new patrol point, so clear the stuck backup timer.
            ResetStuckTurnCheck();
            return;
        }
    }

    void SwitchToLeft()
    {
        patrol = "GoLeft";
        waiting = false;
        facingLeft = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingLeft;
        }

        // A delayed patrol switch counts as a fresh movement state, so reset the backup timer.
        ResetStuckTurnCheck();
    }

    void SwitchToRight()
    {
        patrol = "GoRight";
        waiting = false;
        facingLeft = false;

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingLeft;
        }

        // A delayed patrol switch counts as a fresh movement state, so reset the backup timer.
        ResetStuckTurnCheck();
    }
}