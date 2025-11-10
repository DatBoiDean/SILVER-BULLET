using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AltRaycast : MonoBehaviour
{
    [SerializeField] GameObject player;          // target for ai 
    [SerializeField] GameObject enemy;          // reference to itself
    public float enemyMoveSpeed;        // adjustable move speed for enemy
    private bool isSpotted;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Update()
    {
        Vector2 disToPlayer = (player.transform.localPosition - enemy.transform.position);
        RaycastHit2D ray = Physics2D.Raycast(transform.position, disToPlayer);     //if collider hits tag, set bool to true, draw ray
        if (ray.collider != null)
        {
            isSpotted = ray.collider.CompareTag("Player");
            if (isSpotted)
            {
                transform.position = Vector2.MoveTowards(enemy.transform.position, player.transform.localPosition, enemyMoveSpeed * Time.deltaTime);
                Debug.DrawRay(transform.position, disToPlayer , Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, disToPlayer, Color.red);
            }
        }
    }
}
