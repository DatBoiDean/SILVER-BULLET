using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCharcterController : MonoBehaviour
{
    public float speed = 1;
    public float jumpForce = 1;
    private float move;
    public Rigidbody2D testRigidBody;

    public bool isGrounded = true;
    bool isFacingRight = true;
    [SerializeField] Animator _animator; 
    int crouching = 1;
    //ERA: Rudimentary "no move while preparing crouch jump" thing please god i want this to work

    void Start()
    {
        testRigidBody = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //so help me god if this fucking works.
        if (Input.GetKeyDown(KeyCode.Space)) // jump
        {
            if (isGrounded == true)
            {
                if(Input.GetKey(KeyCode.S))
                //ERA: Implementing a rudimentary crouch jump to get higher jumps
                {
                testRigidBody.velocity = Vector2.up * jumpForce * 1.5f;
                Debug.Log("Jump Key pressed");

                // REMOVED: Invoke to delayed jump bool setter (caused flapping/lag and stuck states)
                // Invoke("jumpAnim", 0.5f); //add variable for time (0.5f for now)

                // NEW (minimal): fire a one-shot trigger to enter Jump state without needing isJumping bool
                _animator.SetTrigger("JumpTrigger");
                }

                else
                {
                testRigidBody.velocity = Vector2.up * jumpForce;
                //ERA: Normal Jump, without pressing S
                Debug.Log("Jump Key pressed");

                
                _animator.SetTrigger("JumpTrigger");
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate() // handles character movement
    {
        if (Input.GetKey(KeyCode.S))
        {
            crouching = 0;
        }
        else
        {
            crouching = 1;
        }
        move = Input.GetAxisRaw("Horizontal");
        testRigidBody.velocity = new Vector2(move * speed * crouching, testRigidBody.velocity.y);

        if (move > 0 && !isFacingRight) // flip if moving chacracter left 
        {
            FlipCharacter();
        }
        else if (move < 0 && isFacingRight)
        {
            FlipCharacter();
            Debug.Log("Facing Right");
        }

        if (Input.GetAxis("Horizontal") != 0) // animation integration
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

            // NOTE: With trigger-based jump, no need to set isJumping=false here.
            // Animator leaves Jump via Exit Time to Idle/Run based on isRunning.
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Spikes"))
        {
            testRigidBody.velocity = Vector2.up * jumpForce * 1.5f;
        }
    }
    void OnCollisionStay2D(Collision2D collision)
    {
        //theres some real fucky stuff happening w the collision stuff
        //so this is my attempt at solving it
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;

            // NOTE: With trigger-based jump, no need to set isJumping=false here.
            // Animator leaves Jump via Exit Time to Idle/Run based on isRunning.
        }
        if (collision.gameObject.CompareTag("Enemy"))
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            isGrounded = false;
        }
    }


    // REMOVED: jumpAnim() method (was toggling isJumping via delayed velocity check and caused state ping-pong)
    /*
    void jumpAnim()
    {
        if (testRigidBody.velocity.y != 0)
        {
            _animator.SetBool("isJumping", true);
        }
        else
        {
            _animator.SetBool("isJumping", false);
        }
    }
    */
}
