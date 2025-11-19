using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damageInterval;
    public int damageAmount;
    public float nextDamageTime;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))  //check if collision has player tag
        {
            if (Time.time >= nextDamageTime)
            {
                var playerHealthComponent = other.GetComponent<PlayerHealth>();

                if (playerHealthComponent != null)
                {
                    Debug.Log("Player health component found");
                    playerHealthComponent.PlayerTakeDamage(damageAmount);

                    nextDamageTime = Time.time + damageInterval;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            nextDamageTime = 0;
        }
    }
}
