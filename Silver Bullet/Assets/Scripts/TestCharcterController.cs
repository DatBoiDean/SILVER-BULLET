using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharcterController : MonoBehaviour
{
    public float speed = 1;
    public float jumpForce = 1;
    public Rigidbody2D testRigidBody;
    bool isGrounded = false; 

    void Start()
    {
        testRigidBody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        //leftward movement
        if (Input.GetKey(KeyCode.A))
        {
           testRigidBody.velocity = Vector2.left * speed;
        }

        //rightward movement
        if (Input.GetKey(KeyCode.D))
        {
            testRigidBody.velocity = Vector2.right * speed;
        }

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
