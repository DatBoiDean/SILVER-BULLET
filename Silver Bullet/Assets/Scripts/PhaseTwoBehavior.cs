using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PhaseTwoBehavior : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    [SerializeField] GameObject obstacleToSpawn;
    [SerializeField] float spawnDelay = 1f;
    // Start is called before the first frame update


    void Start()
    {
        Run();
    }

    void Run()
    {
        if (enemyHealth.currentEnemyHealth == 2)
        {
            Instantiate(obstacleToSpawn, transform.position, Quaternion.identity);
        }
            Invoke("Run",spawnDelay);
        
    }
    // IEnumerator PhaseOneRoutine()
    // {
    //     while (enemyHealth.currentEnemyHealth == 2) // spawn this type of obstacle while boss health is full
    //     {
    //         Instantiate(obstacleToSpawn, transform.position, Quaternion.identity);
    //         yield return new WaitForSeconds(spawnDelay);
    //     }
    // }

    // private void Update()
    // {
    //     StartCoroutine(PhaseOneRoutine());
    // }
}
