using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyChase : MonoBehaviour
{
    public float moveSpeed = 3f;
    public Rigidbody2D rb;
    private Transform playerTransform;
    private bool isChasing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (isChasing && playerTransform != null)
        {
            // Calculate the direction vector from the enemy to the player
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            // Apply a force or set a velocity to move the enemy
            // For simple movement, setting the velocity is most direct
            rb.velocity = direction * moveSpeed;
        }
        else
        {
            // Stop moving if not chasing
            rb.velocity = Vector2.zero;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = true;
            playerTransform = other.transform;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isChasing = false;
        }
    }
}
