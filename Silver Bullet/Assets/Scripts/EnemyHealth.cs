using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int currentEnemyHealth;
    public int maxEnemyHealth;
    public int amount;
    [SerializeField] bool isWire;

    private Rigidbody2D rb;
    // Start is called before the first frame update
    void Start()
    {
        currentEnemyHealth = maxEnemyHealth;
        rb = GetComponent<Rigidbody2D>();
        Debug.Log("rb found");
    }

    public void EnemyTakeDamage(int amount)
    {
        currentEnemyHealth -= amount;
        if (currentEnemyHealth <= 0) 
        {
            if (isWire == false)
            {
                Destroy(gameObject);
            }

            else if (isWire)
            {
                rb.velocity = Vector2.zero; 
            }
        }
    }
}
