using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseOneBehavior : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    [SerializeField] Rigidbody2D obstacle;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Phase 1 Triggered");
        Rigidbody2D spawnedObstacle = Instantiate(obstacle, transform.position, transform.rotation);
        spawnedObstacle.velocity = transform.TransformDirection(Vector2.up * 2);

        Invoke("DestroyOnBossHit", 2f); 
    }

    // Update is called once per frame


    void DestoryOnBossHit(Rigidbody2D spawnedObstacle) 
    {
        if (enemyHealth.currentEnemyHealth == 2)
        {
            Destroy(spawnedObstacle);
        }
    }
}

