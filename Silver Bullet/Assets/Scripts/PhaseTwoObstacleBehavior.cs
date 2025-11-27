using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoObstacleBehavior : MonoBehaviour
{
    [SerializeField] Rigidbody2D obstacleRB;
    [SerializeField] float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        obstacleRB.velocity = Vector2.left * moveSpeed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log("Obstacle Destroyed");
        }
    }
}
