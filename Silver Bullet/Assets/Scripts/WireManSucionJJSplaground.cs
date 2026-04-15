using System.Collections;
using UnityEngine;

public class WireManSuction : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody2D wireManRb;          // Robot Rigidbody2D the player gets pulled toward
    [SerializeField] Rigidbody2D playerRb;           // Player Rigidbody2D
    [SerializeField] string playerTag = "Player";    // Player tag check for the grab zone trigger

    [Header("Suction Settings")]
    [SerializeField] float suctionForce = 8f;        // How strong the pull is
    [SerializeField] float suctionDuration = 2f;     // How long the suction lasts
    [SerializeField] float suctionCooldown = 2f;     // How long before suction can happen again

    // Added: tracks whether the player is currently inside the grab zone
    bool playerInGrabZone = false;

    // Added: stops the suction from starting over and over while it is already active
    bool suctionActive = false;

    // Added: stops the suction from being reused until the cooldown finishes
    bool onCooldown = false;

    private void Reset()
    {
        // Added: tries to auto-find the robot Rigidbody2D from the parent object
        if (wireManRb == null)
        {
            wireManRb = GetComponentInParent<Rigidbody2D>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag))
        {
            return;
        }

        playerInGrabZone = true;

        // Added: auto-grab the player's Rigidbody2D if it was not set in the inspector
        if (playerRb == null)
        {
            playerRb = collision.attachedRigidbody;
        }

        // Added: start suction only if it is not already running and not cooling down
        if (!suctionActive && !onCooldown)
        {
            StartCoroutine(SuctionRoutine());
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag))
        {
            return;
        }

        playerInGrabZone = true;

        // Added: keep the player Rigidbody2D reference updated if needed
        if (playerRb == null)
        {
            playerRb = collision.attachedRigidbody;
        }

        // Added: if the player stays in the zone, suction can restart after cooldown
        if (!suctionActive && !onCooldown)
        {
            StartCoroutine(SuctionRoutine());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag(playerTag))
        {
            return;
        }

        playerInGrabZone = false;
    }

    IEnumerator SuctionRoutine()
    {
        // Added: safety checks so the script does not run with missing Rigidbody2D references
        if (wireManRb == null || playerRb == null)
        {
            yield break;
        }

        suctionActive = true;

        float startTime = Time.time;

        // Added: pull the player for the full suction duration,
        // but only while the player is still inside the grab zone
        while (Time.time < startTime + suctionDuration && playerInGrabZone)
        {
            // Added: calculate the direction from the player to the robot
            Vector2 directionToWireMan = (wireManRb.position - playerRb.position).normalized;

            // CHANGED MAJOR LOGIC:
            // Instead of teleporting or directly snapping the player,
            // this applies force every physics step for a simple pull effect.
            /*
            playerRb.position = Vector2.MoveTowards(playerRb.position, wireManRb.position, suctionForce * Time.deltaTime);
            */

            // Added: apply force toward the robot
            playerRb.AddForce(directionToWireMan * suctionForce, ForceMode2D.Force);

            yield return new WaitForFixedUpdate();
        }

        suctionActive = false;
        onCooldown = true;

        // Added: wait for the inspector cooldown before suction can happen again
        yield return new WaitForSeconds(suctionCooldown);

        onCooldown = false;
    }
}