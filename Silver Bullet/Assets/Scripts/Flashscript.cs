using System.Collections;
using UnityEngine;

public class EnemyFlashRed : MonoBehaviour
{
    // The SpriteRenderer that will change color
    public SpriteRenderer enemySprite;

    // How long the enemy stays red
    public float flashTime = 0.15f;

    // Stores the enemy's normal color
    private Color originalColor;

    void Start()
    {
        // If you forget to assign the sprite in the Inspector,
        // Unity will try to find one on this object automatically
        if (enemySprite == null)
        {
            enemySprite = GetComponent<SpriteRenderer>();
        }

        // Saves the enemy's starting color so it can go back after flashing
        if (enemySprite != null)
        {
            originalColor = enemySprite.color;
        }
    }

    // Calls this function whenever the enemy gets hit
    public void FlashRed()
    {
        // Stops any old flash coroutine so the red flash restarts cleanly
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // Safety check in case no SpriteRenderer was found
        if (enemySprite == null)
        {
            yield break;
        }

        // Changes the enemy to red
        enemySprite.color = Color.red;

        // Waits for a short moment
        yield return new WaitForSeconds(flashTime);

        // Changes back to the original color
        enemySprite.color = originalColor;
    }
}