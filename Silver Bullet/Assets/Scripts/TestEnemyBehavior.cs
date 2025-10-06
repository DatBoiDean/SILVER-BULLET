using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemyBehavior : MonoBehaviour
{
    public GameObject target;            // target for ai
    public float minimumDistance;       // ai not to move until target within distance
    public float enemyMoveSpeed;        // adjustable move speed for enemy

    // Update is called once per frame
    void Update()                       
    {
        if (Vector2.Distance(transform.position, target.transform.position) < minimumDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.transform.position, enemyMoveSpeed * Time.deltaTime);
        }
       
    }

}
