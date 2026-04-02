using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManHealth : MonoBehaviour
{
    public int currentEnemyHealth;
    public int maxEnemyHealth;
    public int amount;
    public Rigidbody2D rb;

    void Start()
    {
        currentEnemyHealth = maxEnemyHealth;
    }

    public void EnemyTakeDamage(int amount)
    {
        currentEnemyHealth -= amount;
        if (currentEnemyHealth <= 0)
        {
            rb.velocity = Vector2.zero;
        }
    }
}
