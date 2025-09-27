using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharcterController : MonoBehaviour
{
    public float speed = 1;
    public float jumpForce = 1;
    private float move;
    public Rigidbody2D testRigidBody;
    bool isGrounded = false; 

    void Start()
    {
        testRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");
        testRigidBody.velocity = new Vector2(move * speed, testRigidBody.velocity.y);

        //jump
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            testRigidBody.velocity = Vector2.up * jumpForce;
        }
    }

    //ground check methods
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
