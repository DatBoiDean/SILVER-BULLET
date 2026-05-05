using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PhaseThreeBehavior : MonoBehaviour
{
    public EnemyHealth1 enemyHealth1;
    [SerializeField] GameObject groundObstacle;
    [SerializeField] float spawnDelay = 1f;

    void Start()
    {
        GroundObstacleSpawn();
    }

    void GroundObstacleSpawn()
    {
        if (enemyHealth1.currentEnemyHealth == 1)
        {
            Debug.Log("Spawning " + groundObstacle);
            Instantiate(groundObstacle, transform.position, Quaternion.identity);
        }

            Invoke("GroundObstacleSpawn", spawnDelay);
    }
}
