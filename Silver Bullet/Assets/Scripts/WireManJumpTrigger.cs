using UnityEngine;

public class WireManJumpTrigger : MonoBehaviour
{
    public WireManMovement wireManMovement;
    public Rigidbody2D rb;
    public float jumpForce = 5f;

    private bool hasJumped = false;

    private void Start()
    {
        if (wireManMovement == null)
            wireManMovement = GetComponent<WireManMovement>();

        if (rb == null && wireManMovement != null)
            rb = wireManMovement.rb;

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
{
    Debug.Log("TRIGGERED BY: " + collision.name + " | Tag: " + collision.tag);

    if (!collision.CompareTag("WireManPlatform"))
        return;

    Debug.Log("WireManPlatform detected, jumping now");
    rb.velocity = new Vector2(rb.velocity.x, jumpForce);
}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            hasJumped = false;
            Debug.Log("Grounded again, jump reset");
        }
    }
}