using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoamingEnemy : MonoBehaviour
{


    public float moveSpeed = 2f;
    public float roamRadius = 5f; // How far the enemy can roam from its starting point
    public float changeDirectionInterval = 3f; // How often the enemy changes direction
    public Rigidbody2D rb;

    private Vector2 startPosition;
    private Vector2 currentTargetPosition;
    private float timer;

    void Start()
    {
        startPosition = transform.position;
        SetNewRoamingTarget();
    }

    void Update()
    {
        // Move towards the current target
        Vector2 direction = (currentTargetPosition - startPosition).normalized;
        rb.velocity = direction * moveSpeed;

        // Check if reached target or time to change direction
        if (Vector2.Distance(transform.position, currentTargetPosition) < 0.1f || timer <= 0f)
        {
            SetNewRoamingTarget();
            timer = changeDirectionInterval; // Reset timer
        }

        timer -= Time.deltaTime;
    }

    void SetNewRoamingTarget()
    {
        // Generate a random point within the roam range from the starting position
        Vector2 randomDirection = Random.insideUnitCircle * roamRadius;
        currentTargetPosition = startPosition + randomDirection;
    }
}

