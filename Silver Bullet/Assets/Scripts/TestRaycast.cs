using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRaycast : MonoBehaviour
{
    public GameObject player;            // target for ai
    public float enemyMoveSpeed;        // adjustable move speed for enemy
    public LayerMask playerCharacter;

    [SerializeField] bool isSpotted = false;
    private void FixedUpdate()
    {
        RaycastHit2D ray = Physics2D.Raycast(transform.position, player.transform.localPosition - transform.position, playerCharacter);
        if (ray.collider != null)
        {
            isSpotted = true;
            if (isSpotted)
            {
                transform.position = Vector2.MoveTowards(transform.position, player.transform.localPosition, enemyMoveSpeed * Time.deltaTime);
                Debug.DrawRay(transform.position, player.transform.localPosition - transform.position, Color.green);
            }
            else
            {
                Debug.DrawRay(transform.position, player.transform.localPosition - transform.position, Color.red);
            }
        }
    }
}
