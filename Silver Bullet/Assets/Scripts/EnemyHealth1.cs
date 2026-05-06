using System.Collections;
using UnityEngine;

public class EnemyHealth1 : MonoBehaviour
{
    public int maxEnemyHealth = 3;
    public int currentEnemyHealth;

    public SpriteRenderer enemySprite;
    public float flashTime = 0.2f;

    private Color originalColor;
    private Rigidbody2D rb;

    [SerializeField] bool isWire;

    public bool bossTookDamage;

    void Start()
    {
        currentEnemyHealth = maxEnemyHealth;
        rb = GetComponent<Rigidbody2D>(); // ← FIXED: assign rb

        if (enemySprite == null)
            enemySprite = GetComponentInChildren<SpriteRenderer>(); // ← searches children too

        if (enemySprite != null)
            originalColor = enemySprite.color;
        else
            Debug.LogWarning(gameObject.name + " has no SpriteRenderer assigned.");
    }

    public void EnemyTakeDamage(int damageAmount)
    {
        currentEnemyHealth -= damageAmount;

        bossTookDamage = true;

        StopAllCoroutines();
        StartCoroutine(FlashRed());

        Debug.Log(gameObject.name + " took damage. Health now: " + currentEnemyHealth);

        if (currentEnemyHealth <= 0)
        {
            if (!isWire)
            {
                Destroy(gameObject);
            }
            else if (rb != null) // ← FIXED: null check before using rb
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    IEnumerator FlashRed()
    {
        if (enemySprite == null) yield break;

        enemySprite.color = Color.red;
        yield return new WaitForSeconds(flashTime);
        enemySprite.color = originalColor;
    }
}