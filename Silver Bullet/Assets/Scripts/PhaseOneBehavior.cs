using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneBehavior : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    [SerializeField] Rigidbody2D obstacle;
    [SerializeField] float riseSpeed; //assign in inspector
    [SerializeField] float riseTimer;
    private Rigidbody2D spawnedObstacle;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Phase 1 Triggered");
        spawnedObstacle = Instantiate(obstacle, transform.position, transform.rotation); //create clone of object since Unity doesn't like destroying prefabs
        spawnedObstacle.velocity = transform.TransformDirection(Vector2.up * riseSpeed);
    }

    private void Update()
    {
        riseTimer += Time.deltaTime;

        if (riseTimer >= 1f && spawnedObstacle != null) //stop upward movement to create object rising effect
        {
            spawnedObstacle.velocity = Vector2.zero;

        }

        if (enemyHealth.currentEnemyHealth == 2)
        {
            Destroy(obstacle);
            Debug.Log("Spawned Obstacle destroyed");
        }
    }
}

