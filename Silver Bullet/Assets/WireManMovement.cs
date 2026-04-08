using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce;
    [SerializeField] float stunTimer;

    public Rigidbody2D rb;
    [SerializeField] Animator animator;

    public GameObject player;

    
    // Start is called before the first frame update
    void Start()
    {
        stunTimer = Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 playerDist = (player.transform.position - transform.position).normalized; //calculate distance between player and enemy

        if (player != null)
        {
            rb.velocity = playerDist * moveSpeed;
        }

        var enemyHealthComponent = GetComponent<EnemyHealth>();

        if (enemyHealthComponent.currentEnemyHealth <= 0)
        {
            rb.velocity = Vector2.zero;
            stunTimer += 1f;
            if (stunTimer >= 300f) 
            {
                rb.velocity = playerDist * moveSpeed;
                enemyHealthComponent.currentEnemyHealth = enemyHealthComponent.maxEnemyHealth;
                stunTimer = 0f;
            }
        }

        
    }
}
