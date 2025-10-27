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
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }

    public void PlayerTakeDamage(int amount)
    {
        currentHealth -= amount;
        healthBar.value = currentHealth;

        if (currentHealth <= 0)
        {
            Debug.Log("Player Killed at " + gameObject.name);
            Debug.Log("Loading Lose screen");
            //Feel free to just comment out the Load Scene thing for testing
            SceneManager.LoadScene(2);
            Destroy(gameObject);
        }
    }
}   
