using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoObstacleBehavior : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall") || collision.collider.CompareTag("Player"))
        {
            Destroy(gameObject);
            Debug.Log("Obstacle Destroyed");
        }
    }
}
