using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManAttack : MonoBehaviour
{
    [SerializeField] float grabRange = 5f;
    [SerializeField] GameObject grabZone;
    [SerializeField] GameObject player;
    [SerializeField] float grabResetTimer;
    [SerializeField] int damageAmount = 1;

    // Added: animator that controls the grab and attack animations
    [SerializeField] Animator anim;

    // Added: keeps the original grab range stored so it can be reused after cooldown
    [SerializeField] float normalGrabRange = 5f;

    // Added: how long the enemy stays in the attack state after catching the player
    [SerializeField] float attackDuration = 3f;

    // Added: how long the enemy waits before it can start grabbing again
    [SerializeField] float attackCooldown = 2f;

    bool playerTookDamage = false;

    // Added: stores when the attack phase started
    float attackStartTime;

    // Added: stores when the cooldown started
    float cooldownStartTime;

    // Added: tells other scripts whether the player is currently inside grab detection range
    bool playerInGrabRange = false;

    // Added: tracks whether the enemy is actively in the attack phase
    bool isAttackActive = false;

    // Added: tracks whether the enemy is waiting for cooldown to finish
    bool isCoolingDown = false;

    // Added: lets the movement script know when the player is close enough for the grab system
    public bool PlayerInGrabRange
    {
        get { return playerInGrabRange; }
    }

    // Added: lets the movement script know when the attack phase is active
    public bool IsAttackActive
    {
        get { return isAttackActive; }
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }

        float dist = Vector2.Distance(player.transform.position, transform.position);

        // Added: while cooling down, disable the grab zone and do not let the attack restart yet
        if (isCoolingDown)
        {
            grabZone.SetActive(false);
            playerInGrabRange = false;

            // Added: once cooldown finishes, allow the grab loop to begin again
            if (Time.time >= cooldownStartTime + attackCooldown)
            {
                isCoolingDown = false;
                playerTookDamage = false;
                grabRange = normalGrabRange;
            }

            return;
        }

        // Added: while the attack is active, keep AttackGrab playing and do not let range logic interrupt it
        if (isAttackActive)
        {
            FacePlayer();

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("AttackGrab"))
            {
                anim.Play("AttackGrab");
            }

            // Added: end the attack after the set duration, then begin cooldown
            if (Time.time >= attackStartTime + attackDuration)
            {
                isAttackActive = false;
                isCoolingDown = true;
                cooldownStartTime = Time.time;
                grabZone.SetActive(false);
                playerInGrabRange = false;
            }

            return;
        }

        // CHANGED MAJOR LOGIC:
        // Older versions could still affect the animator too aggressively during patrol.
        /*
        if (dist <= grabRange)
        {
            grabZone.SetActive(true);
            playerInGrabRange = true;
            FacePlayer();

            if (!playerTookDamage && !anim.GetCurrentAnimatorStateInfo(0).IsName("Grabstart"))
            {
                anim.Play("Grabstart");
            }
        }
        else
        {
            grabZone.SetActive(false);
            playerInGrabRange = false;

            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("IdleAnimation"))
            {
                anim.Play("IdleAnimation");
            }
        }
        */

        // Added: only enter grab mode if the player is actually inside grab range
        if (dist <= grabRange)
        {
            grabZone.SetActive(true);
            playerInGrabRange = true;
            FacePlayer();

            // Added: only play Grabstart while waiting for the actual hit to happen
            if (!playerTookDamage && !anim.GetCurrentAnimatorStateInfo(0).IsName("Grabstart"))
            {
                anim.Play("Grabstart");
            }
        }
        else
        {
            // Added: outside of grab range, this script stops controlling the animator
            // so patrol / chase can keep using Walk normally
            grabZone.SetActive(false);
            playerInGrabRange = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartGrabAttack(collision.collider);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartGrabAttack(collision.collider);
        }
    }

    void StartGrabAttack(Collider2D collision)
    {
        // Added: do not restart the attack if it is already active or cooling down
        if (isAttackActive || isCoolingDown)
        {
            return;
        }

        // Added: only allow the real attack to start if the player was already inside detection range
        if (!playerInGrabRange)
        {
            return;
        }

        var playerHealthComponent = player.GetComponent<PlayerHealthJJsplayground1>();

        if (playerHealthComponent == null)
        {
            playerHealthComponent = player.GetComponentInParent<PlayerHealthJJsplayground1>();
        }

        FacePlayer();

        // CHANGED MAJOR LOGIC:
        // Older versions could keep replaying AttackGrab every collision frame.
        /*
        anim.Play("AttackGrab");
        */

        // Added: only switch to AttackGrab if it is not already playing
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("AttackGrab"))
        {
            anim.Play("AttackGrab");
        }

        // Added: begin the timed attack phase
        isAttackActive = true;
        attackStartTime = Time.time;

        // Added: damage happens once at the beginning of the attack phase
        if (!playerTookDamage)
        {
            if (playerHealthComponent != null)
            {
                playerHealthComponent.PlayerTakeDamage(damageAmount);
            }

            playerTookDamage = true;
        }
    }

    // Added: this now tells the movement script to handle facing and suction-zone side
    void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        // Added: let the movement script control both sprite direction and suction zone position
        WireManMovement movementScript = GetComponent<WireManMovement>();

        if (movementScript != null)
        {
            if (player.transform.position.x > transform.position.x)
            {
                // Added: sends the "face right" update to the movement script
                SendMessage("UpdateFacingAndSuctionZone", true, SendMessageOptions.DontRequireReceiver);
            }
            else
            {
                // Added: sends the "face left" update to the movement script
                SendMessage("UpdateFacingAndSuctionZone", false, SendMessageOptions.DontRequireReceiver);
            }

            return;
        }

        // CHANGED MAJOR LOGIC:
        // Old fallback directly changed localScale and could fight with the movement script.
        /*
        Vector3 localScale = transform.localScale;

        if (player.transform.position.x > transform.position.x)
        {
            localScale.x = Mathf.Abs(localScale.x);
        }
        else
        {
            localScale.x = -Mathf.Abs(localScale.x);
        }

        transform.localScale = localScale;
        */
    }
}