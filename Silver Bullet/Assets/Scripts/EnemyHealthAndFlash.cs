using System.Collections;
using UnityEngine;

public class EnemyHealthAndFlash : MonoBehaviour
{
    // Enemy health
    public int currentHealth = 3;

    // The visible sprite that should flash red
    public SpriteRenderer enemySprite;

    // How long the red flash lasts
    public float flashTime = 0.2f;

    // Store the normal sprite color
    private Color originalColor;

    void Start()
    {
        // If you did not drag in a SpriteRenderer,
        // try to find one on this object automatically
        if (enemySprite == null)
        {
            enemySprite = GetComponent<SpriteRenderer>();
        }

        // Save the normal starting color
        if (enemySprite != null)
        {
            originalColor = enemySprite.color;
        }
        else
        {
            Debug.LogWarning(gameObject.name + " does not have a SpriteRenderer assigned.");
        }
    }

    // Call this when the enemy gets damaged
    public void EnemyTakeDamage(int damageAmount)
    {
        // Lower health
        currentHealth -= damageAmount;

        // Flash red
        FlashRed();

        Debug.Log(gameObject.name + " took damage. Health is now: " + currentHealth);

        // Destroy the enemy if health is 0 or less
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    // Starts the flash effect
    public void FlashRed()
    {
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // Stop if there is no sprite to flash
        if (enemySprite == null)
        {
            yield break;
        }

        // Turn the sprite red
        enemySprite.color = Color.red;

        // Wait a short time
        yield return new WaitForSeconds(flashTime);

        // Change back to the original color
        enemySprite.color = originalColor;
    }
}