using System.Collections;
using UnityEngine;

public class WireEnemyPatrolGrab : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 2f;
    [SerializeField] private float idleTimeAtBoundary = 1f;

    [Header("Player Detection")]
    [SerializeField] private float attractionDistance = 3f;
    [SerializeField] private string playerTag = "Player";

    [Header("Damage")]
    [SerializeField] private int contactDamage = 1;
    [SerializeField] private float damageCooldown = 1f;

    [Header("Boundary Tags Already In Scene")]
    [SerializeField] private string leftBoundaryTag = "LeftMax";
    [SerializeField] private string rightBoundaryTag = "RightMax";

    private Animator animator;
    private Transform player;

    private Transform leftBoundary;
    private Transform rightBoundary;

    private bool facingRight = true;
    private bool isIdlingAtBoundary;
    private bool isPlayerInRange;
    private bool isAttacking;
    private float lastDamageTime;

    private const string IDLE_ANIM = "IdleAnimation";
    private const string WALK_ANIM = "Walk";
    private const string GRABSTART_ANIM = "Grabstart";
    private const string ATTACKGRAB_ANIM = "AttackGrab";

    void Start()
    {
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        FindClosestPatrolBoundaries();

        // Start in idle so the enemy does not instantly move on spawn.
        PlayAnimation(IDLE_ANIM);
        StartCoroutine(StartInitialPatrol());
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        isPlayerInRange = distanceToPlayer <= attractionDistance;

        // If the enemy is currently in the attack state, do not let patrol override it.
        if (isAttacking)
        {
            return;
        }

        // If the player is close enough, stop patrolling and play the grab start animation.
        if (isPlayerInRange)
        {
            PlayAnimation(GRABSTART_ANIM);
            return;
        }

        // If the enemy is waiting at a patrol boundary, keep it idle.
        if (isIdlingAtBoundary)
        {
            PlayAnimation(IDLE_ANIM);
            return;
        }

        Patrol();
    }

    private void Patrol()
    {
        if (leftBoundary == null || rightBoundary == null)
        {
            PlayAnimation(IDLE_ANIM);
            return;
        }

        PlayAnimation(WALK_ANIM);

        float moveDirection = facingRight ? 1f : -1f;
        transform.Translate(Vector2.right * moveDirection * walkSpeed * Time.deltaTime);

        // Stop at patrol limits and begin the idle-turn-walk cycle.
        if (facingRight && transform.position.x >= rightBoundary.position.x)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = rightBoundary.position.x;
            transform.position = clampedPosition;

            StartCoroutine(HandleBoundaryTurn());
        }
        else if (!facingRight && transform.position.x <= leftBoundary.position.x)
        {
            Vector3 clampedPosition = transform.position;
            clampedPosition.x = leftBoundary.position.x;
            transform.position = clampedPosition;

            StartCoroutine(HandleBoundaryTurn());
        }
    }

    private IEnumerator StartInitialPatrol()
    {
        isIdlingAtBoundary = true;
        yield return new WaitForSeconds(1f);
        isIdlingAtBoundary = false;
    }

    private IEnumerator HandleBoundaryTurn()
    {
        if (isIdlingAtBoundary)
        {
            yield break;
        }

        isIdlingAtBoundary = true;
        PlayAnimation(IDLE_ANIM);

        // Wait at the edge for about a second before turning around.
        yield return new WaitForSeconds(idleTimeAtBoundary);

        FlipCharacter();
        isIdlingAtBoundary = false;
    }

    private void FlipCharacter()
    {
        facingRight = !facingRight;

        Vector3 localScale = transform.localScale;
        localScale.x = Mathf.Abs(localScale.x) * (facingRight ? 1f : -1f);
        transform.localScale = localScale;
    }

    private void PlayAnimation(string stateName)
    {
        if (animator == null)
        {
            return;
        }

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
        {
            animator.Play(stateName);
        }
    }

    private void FindClosestPatrolBoundaries()
    {
        GameObject[] leftBounds = GameObject.FindGameObjectsWithTag(leftBoundaryTag);
        GameObject[] rightBounds = GameObject.FindGameObjectsWithTag(rightBoundaryTag);

        float closestLeftDistance = Mathf.Infinity;
        float closestRightDistance = Mathf.Infinity;

        foreach (GameObject bound in leftBounds)
        {
            float xDifference = transform.position.x - bound.transform.position.x;

            // Only use left boundaries that are actually to the left of this enemy.
            if (xDifference >= 0f && xDifference < closestLeftDistance)
            {
                closestLeftDistance = xDifference;
                leftBoundary = bound.transform;
            }
        }

        foreach (GameObject bound in rightBounds)
        {
            float xDifference = bound.transform.position.x - transform.position.x;

            // Only use right boundaries that are actually to the right of this enemy.
            if (xDifference >= 0f && xDifference < closestRightDistance)
            {
                closestRightDistance = xDifference;
                rightBoundary = bound.transform;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(playerTag))
        {
            return;
        }

        StartAttack(collision.collider);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(playerTag))
        {
            return;
        }

        StartAttack(collision.collider);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(playerTag))
        {
            return;
        }

        isAttacking = false;

        // If the player leaves the hitbox but is still nearby, go back to grab start.
        if (isPlayerInRange)
        {
            PlayAnimation(GRABSTART_ANIM);
        }
        else
        {
            PlayAnimation(IDLE_ANIM);
        }
    }

    private void StartAttack(Collider2D playerCollider)
    {
        isAttacking = true;
        PlayAnimation(ATTACKGRAB_ANIM);

        // This is only the visual attack + damage trigger.
        // You said you will send the real dragging physics next, so that can be added here later.
        if (Time.time >= lastDamageTime + damageCooldown)
        {
            PlayerHealthJJsplayground1 playerHealth = playerCollider.GetComponent<PlayerHealthJJsplayground1>();

            if (playerHealth == null)
            {
                playerHealth = playerCollider.GetComponentInParent<PlayerHealthJJsplayground1>();
            }

            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(contactDamage);
                lastDamageTime = Time.time;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionDistance);
    }
}