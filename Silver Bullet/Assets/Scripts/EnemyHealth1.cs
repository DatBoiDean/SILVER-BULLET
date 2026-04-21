using System.Collections;
using UnityEngine;

public class EnemyHealth1 : MonoBehaviour
{
    // How much health the enemy starts with
    public int currentHealth = 3;

    // The visible sprite that should flash red when hit
    public SpriteRenderer enemySprite;

    // How long the red flash lasts
    public float flashTime = 0.2f;

    // Stores the enemy's normal color
    private Color originalColor;

    void Start()
    {
        // If no sprite was assigned in the Inspector,
        // try to grab one from this object automatically
        if (enemySprite == null)
        {
            enemySprite = GetComponent<SpriteRenderer>();
        }

        // Save the original color so we can change back after flashing
        if (enemySprite != null)
        {
            originalColor = enemySprite.color;
        }
        else
        {
            Debug.LogWarning(gameObject.name + " does not have a SpriteRenderer assigned.");
        }
    }

    // This is the function your TestAttack script is already calling
    public void EnemyTakeDamage(int damageAmount)
    {
        // Lower the enemy's health
        currentHealth -= damageAmount;

        // Flash the enemy red when hit
        StopAllCoroutines();
        StartCoroutine(FlashRed());

        Debug.Log(gameObject.name + " took damage. Health now: " + currentHealth);

        // Destroy the enemy when health reaches 0
        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    IEnumerator FlashRed()
    {
        // Stop if there is no sprite renderer
        if (enemySprite == null)
        {
            yield break;
        }

        // Turn the enemy red
        enemySprite.color = Color.red;

        // Wait a short time
        yield return new WaitForSeconds(flashTime);

        // Change back to the original color
        enemySprite.color = originalColor;
    }
}
