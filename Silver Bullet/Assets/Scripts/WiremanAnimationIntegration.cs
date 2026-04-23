using UnityEngine;

public class WireManAnimationController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Drag the WireManMovement script here.")]
    public WireManMovement wireManMovement;

    [Tooltip("Drag the Animator here.")]
    public Animator animator;

    [Header("Animation Distances")]
    [Tooltip("If the player is within this distance, the walk animation will play.")]
    public float detectionDist = 6f;

    [Header("Grab Detection")]
    [Tooltip("True while the player is inside the grab zone.")]
    public bool playerInGrabZone = false;

    private void Start()
    {
        // Auto-find references if they are on the same object
        if (wireManMovement == null)
        {
            wireManMovement = GetComponent<WireManMovement>();
        }

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    private void Update()
    {
        if (wireManMovement == null || animator == null || wireManMovement.player == null)
        {
            return;
        }

        // Horizontal distance between WireMan and player
        float xDistanceToPlayer = Mathf.Abs(
            wireManMovement.player.transform.position.x - transform.position.x
        );

        // Walk animation plays whenever the player is in detection range
        // but not while the player is in the grab zone
        bool isMoving = xDistanceToPlayer <= detectionDist && !playerInGrabZone;

        animator.SetBool("IsMoving", isMoving);
        animator.SetBool("PlayerInGrabZone", playerInGrabZone);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Detect the player entering the grab zone through any child collider
        if (wireManMovement != null && wireManMovement.player != null)
        {
            if (collision.gameObject == wireManMovement.player || collision.transform.IsChildOf(wireManMovement.player.transform))
            {
                playerInGrabZone = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Detect the player leaving the grab zone through any child collider
        if (wireManMovement != null && wireManMovement.player != null)
        {
            if (collision.gameObject == wireManMovement.player || collision.transform.IsChildOf(wireManMovement.player.transform))
            {
                playerInGrabZone = false;
            }
        }
    }
}