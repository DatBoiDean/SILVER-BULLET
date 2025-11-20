using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhaseTwoBehavior : MonoBehaviour
{
    public EnemyHealth enemyHealth;
    // Start is called before the first frame update
    void Start()
    {
        if (enemyHealth != null)
        {
            Debug.Log("Boss Health Component Found");

        }

    }

        // Update is called once per frame
    void Update()
    {
        if (enemyHealth.currentEnemyHealth == 2) 
        {
            Debug.Log("Phase Two Triggered");
        }
    }
}
