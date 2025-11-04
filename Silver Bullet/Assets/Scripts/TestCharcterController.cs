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
    bool isFacingRight = true;
    [SerializeField] Animator _animator; 


    void Start()
    {
        testRigidBody = GetComponent<Rigidbody2D>();
    }


    // Update is called once per frame
    void FixedUpdate() // handles character movement
    {
        move = Input.GetAxisRaw("Horizontal");
        testRigidBody.velocity = new Vector2(move * speed, testRigidBody.velocity.y);

        if (move > 0 && !isFacingRight) // flip if moving chacracter left 
        {
            FlipCharacter();
        }

        else if (move < 0 && isFacingRight)
        {
            FlipCharacter();
            Debug.Log("Facing Right");
        }



        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) // jump
        {
            testRigidBody.velocity = Vector2.up * jumpForce;
        }

        if (move != 0) // animation integration
        {
            _animator.SetBool("isRunning", true);
        }

        else
        {
            _animator.SetBool("isRunning", false);
        }
    }

    void FlipCharacter() // have character face left or right depending on input
    {
        isFacingRight = !isFacingRight;

        Vector2 currentScale = transform.localScale; // get current scale of character

        currentScale.x = -currentScale.x; // flip scale of character

        transform.localScale = currentScale; // set new scale

    }

    private void OnCollisionEnter2D(Collision2D collision) // ground check methods
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
