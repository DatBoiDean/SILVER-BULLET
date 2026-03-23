using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealthJJsplayground1 : MonoBehaviour
{
    public int currentHealth = 3;
    public int maxHealth;
    public Slider healthBar;
    [SerializeField] float invulnTime;
    public float invulnWait;
    public bool invuln;
    public float fear;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
        fear = 0;
    }

    void Update()
    {
        if (fear < 0)
        {
            fear = 0f;
        }

        // Reduces invulnerability wait as fear increases,
        // but keeps a tiny minimum so the player cannot get instantly multi-hit
        invulnWait = Mathf.Max(0.1f, invulnTime - fear);

        // put stuff in here to interact with the fear meter
    }

    public void PlayerTakeDamage(int damageAmount)
    {
        Debug.Log("PlayerTakeDamage called with damage: " + damageAmount);

        if (invuln == false)
        {
            currentHealth -= damageAmount;
            Debug.Log("Player health is now: " + currentHealth);

            healthBar.value = currentHealth;

            if (currentHealth <= 0)
            {
                Debug.Log("Player Killed fired from " + gameObject.name);
                Debug.Log("Loading Lose screen fired from " + gameObject.name);
                //Feel free to just comment out the Load Scene thing for testing
                SceneManager.LoadScene(2);
                Destroy(gameObject);
            }

            InvulnWait();
        }
        else
        {
            Debug.Log("Damage ignored because player is invulnerable.");
        }
    }

    void InvulnWait()
    {
        invuln = true;
        Debug.Log("Invuln On for " + invulnWait + " seconds");
        Invoke(nameof(InvulnOff), invulnWait);
    }

    void InvulnOff()
    {
        invuln = false;
        Debug.Log("Invuln Off");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Spikes"))
        {
            PlayerTakeDamage(1);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fear"))
        {
            fear = fear + 0.1f;
            Debug.Log("Fear at " + fear);
        }

        if (collision.gameObject.CompareTag("Win"))
        {
            PlayerTakeDamage(-1);
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fear"))
        {
            if (fear > 0)
            {
                fear = fear - 0.1f;
                Debug.Log("Fear at " + fear);
            }

            if (fear <= 0)
            {
                fear = 0;
                Debug.Log("Fear would underflow to 0, setting fear to 0.");
            }
        }
    }
}