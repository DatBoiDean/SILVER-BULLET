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
        player = GameObject.FindGameObjectWithTag("Player"); 
    }

    // Update is called once per frame
    void Update()
    {
        if (isSpotted)  
        {
            transform.position = Vector2.MoveTowards(enemy.transform.position, player.transform.position, enemyMoveSpeed * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        Vector2 disToPlayer = (player.transform.position - enemy.transform.position).normalized;
        RaycastHit2D ray = Physics2D.Raycast(transform.position, disToPlayer);     //if collider hits tag, set bool to true, draw ray
        if (ray.collider != null)
        {
            isSpotted = ray.collider.CompareTag("Player");
            if (isSpotted)
            {
                Debug.DrawRay(transform.position, disToPlayer , Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, disToPlayer, Color.red);
            }
        }
    }
}
