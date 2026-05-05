using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneBehavior : MonoBehaviour
{
    public EnemyHealth1 enemyHealth1;
    [SerializeField] GameObject obstacleToSpawn;
    [SerializeField] float spawnDelay = 1f;
   

       // This connects to the animation script
    public ObjectShooterAnimation shooterAnimation; 

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PhaseOneRoutine());
        Debug.Log("Phase One Behavior Triggered");

    }

    IEnumerator PhaseOneRoutine()
    {
        while (enemyHealth1.currentEnemyHealth == enemyHealth1.maxEnemyHealth) // spawn this type of obstacle while boss health is full
        {
            // Play shooter animation before spawning the object
            shooterAnimation.PlayShootAnimation();
            Instantiate(obstacleToSpawn, transform.position, Quaternion.identity);
            yield return new WaitForSeconds(spawnDelay);
        }
    }
}

