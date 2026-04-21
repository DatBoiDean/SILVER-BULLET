using System.Collections;
using UnityEngine;

public class EnemyFlashRed : MonoBehaviour
{
    // Drag the visible enemy sprite here
    public SpriteRenderer enemySprite;

    // Make this longer for testing so it is easy to see
    public float flashTime = 1f;

    // Save the normal color
    private Color originalColor;

    void Start()
    {
        if (enemySprite == null)
        {
            enemySprite = GetComponent<SpriteRenderer>();
        }

        if (enemySprite != null)
        {
            originalColor = enemySprite.color;
        }

        Debug.Log(gameObject.name + " flash script started");
    }

    public void FlashRed()
    {
        Debug.Log(gameObject.name + " FlashRed() was called");

        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        if (enemySprite == null)
        {
            Debug.LogWarning(gameObject.name + " has no SpriteRenderer assigned");
            yield break;
        }

        Debug.Log(gameObject.name + " turning red now");

        enemySprite.color = Color.red;

        yield return new WaitForSeconds(flashTime);

        enemySprite.color = originalColor;

        Debug.Log(gameObject.name + " changed back to original color");
    }
}