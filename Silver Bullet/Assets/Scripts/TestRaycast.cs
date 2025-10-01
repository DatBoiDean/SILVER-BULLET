using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
    public Transform player;            // target for ai
    public float minimumDistance;       // ai not to move until player within distance
    public float enemyMoveSpeed;        // adjustable move speed for enemy
    public LayerMask playerCharacter;

    [SerializeField] bool isSpotted = false;

    // Update is called once per frame
    void Update()                       
    {
       if (isSpotted)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, enemyMoveSpeed * Time.deltaTime);                                                                                        
        }
    }

    private void FixedUpdate()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.position - transform.position, playerCharacter);
        if (ray.collider != null)
        {
            isSpotted = ray.collider.CompareTag("Player");
            if (isSpotted)
            {
                Debug.DrawRay(transform.position, player.position - transform.position, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, player.position - transform.position, Color.red);
            }
        }
    }
}
