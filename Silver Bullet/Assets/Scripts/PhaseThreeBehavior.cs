using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PhaseThreeBehavior : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    [SerializeField] GameObject groundObstacle;
    [SerializeField] float spawnDelay = 1f;

    void Start()
    {
        GroundObstacleSpawn();
    }

    void GroundObstacleSpawn()
    {
        if (enemyHealth.currentEnemyHealth == 1)
        {
            Debug.Log("Spawning " + groundObstacle);
            Instantiate(groundObstacle, transform.position, Quaternion.identity);
        }

            Invoke("GroundObstacleSpawn", spawnDelay);
    }
}
