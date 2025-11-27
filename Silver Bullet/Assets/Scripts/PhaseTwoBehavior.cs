using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoBehavior : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    [SerializeField] GameObject obstacleToSpawn;
    [SerializeField] float spawnDelay = 1f;
    // Start is called before the first frame update

    IEnumerator PhaseOneRoutine()
    {
        while (enemyHealth.currentEnemyHealth == 2) // spawn this type of obstacle while boss health is full
        {
            Instantiate(obstacleToSpawn, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    private void Update()
    {
        StartCoroutine(PhaseOneRoutine());
    }
}
