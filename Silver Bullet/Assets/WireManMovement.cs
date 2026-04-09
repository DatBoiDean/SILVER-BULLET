using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float stunTimer;

    [Header("Detection")]
    [SerializeField] float detectionRange = 5f;

    [Header("Ground Check")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] LayerMask groundLayer;

    [Header("Pit Recovery")]
    [SerializeField] float jumpCooldown = 1f;
    float lastJumpTime;

    [Header("References")]
    public Rigidbody2D rb;
    [SerializeField] Animator animator;
    public GameObject player;
    [SerializeField] WireManAttack wireManAttack;

    [Header("Visual / Suction Side")]
    // Added: this should be the child object that holds the visible animated sprite
    [SerializeField] Transform visualRoot;

    // Added: this should be the actual SpriteRenderer on the visible art
    [SerializeField] SpriteRenderer spriteRenderer;

    // Added: this is the suction zone child
    [SerializeField] Transform suctionZone;

    // Added: horizontal offset for the suction zone
    [SerializeField] float suctionOffsetX = 1.5f;

    [Header("Suction State")]
    [SerializeField] bool suctionActive = false;

    bool isFacingRight = true;

    Transform leftPatrolPoint;
    Transform rightPatrolPoint;

    bool movingRight = true;
    bool isGrounded;

    void Start()
    {
        stunTimer = Time.deltaTime;

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        if (wireManAttack == null)
        {
            wireManAttack = GetComponent<WireManAttack>();
        }

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        FindClosestPatrolPoints();

        // Added: set starting visual side and suction side
        UpdateFacingAndSuctionZone(isFacingRight);
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        float distToPlayer = Vector2.Distance(player.transform.position, transform.position);

        bool playerInAttackRange = wireManAttack != null && wireManAttack.PlayerInGrabRange;
        bool attackIsActive = wireManAttack != null && wireManAttack.IsAttackActive;

        var enemyHealthComponent = GetComponent<EnemyHealth>();

        if (enemyHealthComponent != null && enemyHealthComponent.currentEnemyHealth <= 0)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);
            stunTimer += 1f;

            if (stunTimer >= 300f)
            {
                enemyHealthComponent.currentEnemyHealth = enemyHealthComponent.maxEnemyHealth;
                stunTimer = 0f;
            }

            return;
        }

        if (suctionActive)
        {
            FacePlayer();
            rb.velocity = new Vector2(0f, rb.velocity.y);

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }

            return;
        }

        if (playerInAttackRange || attackIsActive)
        {
            FacePlayer();
            rb.velocity = new Vector2(0f, rb.velocity.y);
            return;
        }

        if (distToPlayer <= detectionRange)
        {
            if (isGrounded)
            {
                float xDirection = Mathf.Sign(player.transform.position.x - transform.position.x);

                rb.velocity = new Vector2(xDirection * moveSpeed, rb.velocity.y);

                if (xDirection < 0f && isFacingRight)
                {
                    UpdateFacingAndSuctionZone(false);
                }
                else if (xDirection > 0f && !isFacingRight)
                {
                    UpdateFacingAndSuctionZone(true);
                }
            }

            TryJumpTowardTarget(player.transform.position);
        }
        else
        {
            PatrolBetweenPoints();
        }

        if (animator != null)
        {
            bool isMovingHorizontally = Mathf.Abs(rb.velocity.x) > 0.01f;
            animator.SetBool("IsMoving", isMovingHorizontally);
        }
    }

    void PatrolBetweenPoints()
    {
        if (leftPatrolPoint == null || rightPatrolPoint == null)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }

            return;
        }

        float xDirection = movingRight ? 1f : -1f;

        if (isGrounded)
        {
            rb.velocity = new Vector2(xDirection * moveSpeed, rb.velocity.y);
        }

        if (movingRight && !isFacingRight)
        {
            UpdateFacingAndSuctionZone(true);
        }
        else if (!movingRight && isFacingRight)
        {
            UpdateFacingAndSuctionZone(false);
        }

        Vector3 patrolTarget = movingRight ? rightPatrolPoint.position : leftPatrolPoint.position;
        TryJumpTowardTarget(patrolTarget);

        // Added: flip immediately when patrol touches a boundary
        if (movingRight && transform.position.x >= rightPatrolPoint.position.x)
        {
            movingRight = false;
            UpdateFacingAndSuctionZone(false);
        }
        else if (!movingRight && transform.position.x <= leftPatrolPoint.position.x)
        {
            movingRight = true;
            UpdateFacingAndSuctionZone(true);
        }
    }

    void TryJumpTowardTarget(Vector3 targetPosition)
    {
        if (!isGrounded)
        {
            return;
        }

        if (targetPosition.y > transform.position.y + 0.5f)
        {
            if (Time.time >= lastJumpTime + jumpCooldown)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f);
                rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                lastJumpTime = Time.time;
            }
        }
    }

    void FindClosestPatrolPoints()
    {
        GameObject[] leftPoints = GameObject.FindGameObjectsWithTag("LeftMax");
        GameObject[] rightPoints = GameObject.FindGameObjectsWithTag("RightMax");

        float closestLeftDistance = Mathf.Infinity;
        float closestRightDistance = Mathf.Infinity;

        foreach (GameObject point in leftPoints)
        {
            float distance = transform.position.x - point.transform.position.x;

            if (distance >= 0f && distance < closestLeftDistance)
            {
                closestLeftDistance = distance;
                leftPatrolPoint = point.transform;
            }
        }

        foreach (GameObject point in rightPoints)
        {
            float distance = point.transform.position.x - transform.position.x;

            if (distance >= 0f && distance < closestRightDistance)
            {
                closestRightDistance = distance;
                rightPatrolPoint = point.transform;
            }
        }
    }

    void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        if (player.transform.position.x > transform.position.x)
        {
            UpdateFacingAndSuctionZone(true);
        }
        else
        {
            UpdateFacingAndSuctionZone(false);
        }
    }

    // Added: one place that flips both the visible art and the suction zone
    public void UpdateFacingAndSuctionZone(bool faceRight)
    {
        isFacingRight = faceRight;

        // CHANGED MAJOR LOGIC:
        // Do not rely on Virtual Root here. Flip the visual child directly instead.
        /*
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = faceRight;
        }
        */

        // Added: flip the visible art by localScale
        if (visualRoot != null)
        {
            Vector3 visualScale = visualRoot.localScale;
            visualScale.x = faceRight ? Mathf.Abs(visualScale.x) : -Mathf.Abs(visualScale.x);
            visualRoot.localScale = visualScale;
        }
        else if (spriteRenderer != null)
        {
            // Fallback if you do not use a separate visual root
            // If backwards, swap to !faceRight
            spriteRenderer.flipX = faceRight;
        }

        UpdateSuctionZoneSide();
    }

    void UpdateSuctionZoneSide()
    {
        if (suctionZone == null)
        {
            return;
        }

        Vector3 localPos = suctionZone.localPosition;

        if (isFacingRight)
        {
            localPos.x = Mathf.Abs(suctionOffsetX);
        }
        else
        {
            localPos.x = -Mathf.Abs(suctionOffsetX);
        }

        suctionZone.localPosition = localPos;
    }

    public void SetSuctionActive(bool active)
    {
        suctionActive = active;

        if (suctionActive)
        {
            rb.velocity = new Vector2(0f, rb.velocity.y);

            if (animator != null)
            {
                animator.SetBool("IsMoving", false);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}