using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
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
        
        if (fear >= 0)
        {
            if (fear >= invulnTime)
            {
                invulnWait = invulnTime - invulnTime;
                //Basically reduces invuln time to zero without underflowing
                //put stuff in here to interact with the fear meter
            }
            else
            {
                invulnWait = invulnTime - fear;
                //Otherwise, reduces invuln wait by the normal value
                //put stuff in here to interact with the fear meter
            }
        }
    }

    public void PlayerTakeDamage(int amount)
    {
        if (invuln == false)
        {
            currentHealth -= amount;
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
    }

    void InvulnWait()
    {
        invuln = true;
        Debug.Log("Invuln On");
        Invoke("InvulnOff", invulnWait);
    }

    void InvulnOff()
    {
        invuln = false;
        Debug.Log("Invuln Off");
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Fear"))
        {
            fear = fear + 0.1f;
            Debug.Log("Fear at " + fear);
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
