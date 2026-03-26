using UnityEngine;

public class FanHeadSuctionZoneRelay : MonoBehaviour
{
    [Header("Parent FanHead")]
    [SerializeField] FanHeadMovementJJedition fanHead;

    private void Reset()
    {
        if (fanHead == null)
        {
            fanHead = GetComponentInParent<FanHeadMovementJJedition>();
        }
    }

    private void Awake()
    {
        if (fanHead == null)
        {
            fanHead = GetComponentInParent<FanHeadMovementJJedition>();
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fanHead == null)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            fanHead.SetSuctionActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (fanHead == null)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            fanHead.SetSuctionActive(false);
        }
    }
}