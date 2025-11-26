using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltPhaseOneObstacle : MonoBehaviour
{
    [SerializeField] Rigidbody2D obstacleRB;
    [SerializeField] float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        obstacleRB.velocity = (Vector2.up * moveSpeed);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player") || collision.collider.CompareTag("Ceiling"))
        {
            Destroy(gameObject);
            Debug.Log("Obstacle Destroyed");
        }
    }
}
