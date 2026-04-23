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

    bool isFacingRight;

    
    // Start is called before the first frame update
    void Start()
    {
        stunTimer = Time.deltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 playerDist = new Vector2(player.transform.position.x - transform.position.x, 0).normalized; //calculate distance between player and enemy

        if (player != null)
        {
            rb.velocity = playerDist * moveSpeed;

            // If player is to the left and enemy is facing right
            if (player.transform.position.x < transform.position.x && isFacingRight)
            {
                FlipCharacter();
                isFacingRight = false;
            }
            // If player is to the right and enemy is facing left
            else if (player.transform.position.x > transform.position.x && !isFacingRight)
            {
                FlipCharacter();
                isFacingRight = true;
            }

            var enemyHealthComponent = GetComponent<EnemyHealth1>();

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

        void FlipCharacter() // have character face left or right depending on input
        {
            Vector2 currentScale = transform.localScale; // get current scale of character
            currentScale.x = -currentScale.x; // flip scale of character
            transform.localScale = currentScale; // set new scale
        }

    }
}
