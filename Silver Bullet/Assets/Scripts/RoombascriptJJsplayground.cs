using UnityEngine;

public class RoombaDamage : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] int damageAmount = 1;
    [SerializeField] float damageCooldown = 1f;

    private float nextDamageTime;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextDamageTime) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthJJsplayground1 playerHealth =
                collision.gameObject.GetComponent<PlayerHealthJJsplayground1>();

            if (playerHealth != null)
            {
                playerHealth.PlayerTakeDamage(damageAmount);
                nextDamageTime = Time.time + damageCooldown;
            }
        }
    }
}