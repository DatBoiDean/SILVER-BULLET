using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneBehavior : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    [SerializeField] GameObject obstacleToSpawn;
    [SerializeField] float spawnDelay = 1f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PhaseOneRoutine());
        Debug.Log("Phase One Behavior Triggered");

    }

    IEnumerator PhaseOneRoutine()
    {
        while (enemyHealth.currentEnemyHealth == enemyHealth.maxEnemyHealth) // spawn this type of obstacle while boss health is full
        {
            Instantiate(obstacleToSpawn, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}

