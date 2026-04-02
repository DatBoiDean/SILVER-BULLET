using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce;
    [SerializeField] float attackRange;

    public Rigidbody2D rb;
    [SerializeField] Animator animator;

    public GameObject player;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 playerDist = (player.transform.position - transform.position).normalized; //calculate distance between player and enemy

        if (player != null)
        {
            rb.velocity = playerDist * moveSpeed;
        }
    }
}
