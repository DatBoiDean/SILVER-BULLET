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
        player = GameObject.FindWithTag("Player");

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

        // Checks if player is inside enemy patrol zone (between its left/right limits)
        bool playerWithinPatrol = true;

        float px = player.transform.position.x;

        // If both limits have been set, enforce the zone
        if (leftLimitX != float.NegativeInfinity && rightLimitX != float.PositiveInfinity)
        {
            float minX = Mathf.Min(leftLimitX, rightLimitX);
            float maxX = Mathf.Max(leftLimitX, rightLimitX);
            playerWithinPatrol = (px >= minX && px <= maxX);
        }

        if (dist <= waitDist)
        {
            isChasing = false;
        }
        else if (playerWithinPatrol && dist <= detectionDist)
        {
            isChasing = true;
            playerTransform = player.transform;
        }
        else
        {
            isChasing = false;
        }

        if (Stuck == false)
        {
            if (isChasing && playerTransform != null)
            {
                waiting = false;

                float dx = playerTransform.position.x - transform.position.x;

                if (dx < 0f)
                {
                    direction = Vector2.left;
                    patrol = "GoLeft";
                    facingLeft = true;
                }
                else
                {
                    direction = Vector2.right;
                    patrol = "GoRight";
                    facingLeft = false;
                }

                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = facingLeft;
                }

                rb.velocity = new Vector2(direction.x * moveSpeed, rb.velocity.y);

                ResetStuckTurnCheck();
            }
            else
            {
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

                HandleStuckTurnCheck();
            }

            if (anim != null)
            {
                bool inRangeNow = dist <= waitDist;
                bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.1f;

                if (inRangeNow)
                {
                    isMovingHorizontally = false;
                }

                anim.SetBool(isMovingParam, isMovingHorizontally);
                anim.SetBool(inRangeParam, inRangeNow);
            }

            if (spriteRenderer != null && Mathf.Abs(rb.velocity.x) > 0.01f)
            {
                facingLeft = rb.velocity.x < 0f;
                spriteRenderer.flipX = facingLeft;
            }
        }

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
                isChasing = true;
                playerTransform = player.transform;
            }
            else
            {
                isChasing = false;

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

            FacePlayer();

            rb.velocity = new Vector2(0f, rb.velocity.y);

            ResetStuckTurnCheck();
        }

        ApplyFlippedAttackSawPositions();
        CheckAttackHitboxOverlap();
    }

    void HandleStuckTurnCheck()
    {
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
                ReversePatrolDirection();

                ResetStuckTurnCheck();
                lastRecordedX = transform.position.x;
            }
        }
        else
        {
            stuckTurnTimer = 0f;
            lastRecordedX = currentX;
        }
    }

    void ResetStuckTurnCheck()
    {
        stuckTurnTimer = 0f;
        lastRecordedX = transform.position.x;
    }

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

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = facingLeft;
        }

        rb.velocity = new Vector2(0f, rb.velocity.y);
    }

    void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        float dx = player.transform.position.x - transform.position.x;

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

    public void BeginAttackHitWindow()
    {
        attackHitWindowActive = true;
        hasHitPlayerThisSwing = false;

        if (attackHitboxTrigger != null)
        {
            attackHitboxTrigger.enabled = true;
        }

        Debug.Log("Attack hit window started");
    }

    public void EndAttackHitWindow()
    {
        attackHitWindowActive = false;

        if (attackHitboxTrigger != null)
        {
            attackHitboxTrigger.enabled = false;
        }

        Debug.Log("Attack hit window ended");
    }

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

            if (hit.gameObject.name != "Hurtbox")
            {
                continue;
            }

            Debug.Log("SawBot overlap check found Hurtbox.");

            // CHANGED: use "hit" because this function is looping through overlap results
            PlayerHealth playerHealth = hit.GetComponentInParent<PlayerHealth>();

            // CHANGED: use the same variable name that was just declared above
            if (playerHealth != null)
            {
                Debug.Log("SawBot hit Hurtbox for damage: " + damageAmount);
                playerHealth.PlayerTakeDamage(damageAmount);

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

        if (!stateInfo.IsName(attackStateName) && !stateInfo.IsName("Base Layer." + attackStateName))
        {
            return;
        }

        if (!facingLeft)
        {
            return;
        }

        int totalFrames = 7;

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

        leftSawTransform.localPosition = leftBladeAttackLeftFacing[frameIndex];
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
        if (collision.gameObject.CompareTag("LeftMax"))
        {
            if (isChasing == false)
            {
                rb.velocity = Vector2.zero;
                patrol = "Wait";
                Invoke(nameof(SwitchToRight), waitTime);
                waiting = true;

                leftLimitX = collision.transform.position.x;
            }
            else
            {
                patrol = "GoRight";
                facingLeft = false;

                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = facingLeft;
                }

                leftLimitX = collision.transform.position.x;
            }

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

                rightLimitX = collision.transform.position.x;
            }
            else
            {
                patrol = "GoLeft";
                facingLeft = true;

                if (spriteRenderer != null)
                {
                    spriteRenderer.flipX = facingLeft;
                }

                rightLimitX = collision.transform.position.x;
            }

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

        ResetStuckTurnCheck();
    }
}