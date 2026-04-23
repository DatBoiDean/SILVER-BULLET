using System.Collections;
using UnityEngine;

public class EnemyHealth1 : MonoBehaviour
{
    // How much health the enemy starts with
    public int currentEnemyHealth = 0;

    public int maxEnemyHealth = 3;

    // The visible sprite that should flash red when hit
    public SpriteRenderer enemySprite;

    // How long the red flash lasts
    public float flashTime = 0.2f;

    // Stores the enemy's normal color
    private Color originalColor;

    [SerializeField] bool isWire;

    private Rigidbody2D rb;

    void Start()
    {
        currentEnemyHealth = maxEnemyHealth;
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
        currentEnemyHealth -= damageAmount;

        // Flash the enemy red when hit
        StopAllCoroutines();
        StartCoroutine(FlashRed());

        Debug.Log(gameObject.name + " took damage. Health now: " + currentEnemyHealth);

        // Destroy the enemy when health reaches 0
        if (currentEnemyHealth <= 0)
        {
           if (isWire == false)
           {
               Destroy(gameObject);
           }

           else if (isWire == true)
           {
                rb.velocity = Vector2.zero;
           }

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
