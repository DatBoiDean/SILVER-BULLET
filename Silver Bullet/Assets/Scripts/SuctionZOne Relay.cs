using UnityEngine;

public class WireManSuctionZoneRelay : MonoBehaviour
{
    [Header("Parent WireMan")]
    [SerializeField] WireManMovement wireMan;

    private void Reset()
    {
        if (wireMan == null)
        {
            wireMan = GetComponentInParent<WireManMovement>();
        }
    }

    private void Awake()
    {
        if (wireMan == null)
        {
            wireMan = GetComponentInParent<WireManMovement>();
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            // Added: this matches your FanHead relay setup
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (wireMan == null)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            wireMan.SetSuctionActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (wireMan == null)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            wireMan.SetSuctionActive(false);
        }
    }
}