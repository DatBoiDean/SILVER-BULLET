using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int currentEnemyHealth;
    public int maxEnemyHealth;
    public int amount;
    // Start is called before the first frame update
    void Start()
    {
        currentEnemyHealth = maxEnemyHealth;
    }

    public void EnemyTakeDamage(int amount)
    {
        currentEnemyHealth -= amount;
        if (currentEnemyHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
