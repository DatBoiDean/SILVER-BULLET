using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PhaseTwoBehavior : MonoBehaviour
{
    public EnemyHealth1 enemyHealth1;
    [SerializeField] GameObject obstacleToSpawn;
    [SerializeField] float spawnDelay = 1f;

     // Reference to the shooter animation script
    public ObjectShooterAnimation shooterAnimation;

    // Start is called before the first frame update


    void Start()
    {
        Run();
    }

    void Run()
    {
        if (enemyHealth1.currentEnemyHealth == 2)
        {
            // Play the shooter animation before spawning
            shooterAnimation.PlayShootAnimation();
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
