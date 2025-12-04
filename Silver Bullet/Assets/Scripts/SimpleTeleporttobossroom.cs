using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadBossRoomOnTouch : MonoBehaviour
{
    private void Awake()
    {
        Debug.Log("[Teleport] Script is on: " + gameObject.name);
    }

    private void OnEnable()
    {
        Debug.Log("[Teleport] " + gameObject.name + " is ENABLED");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[Teleport] OnTriggerEnter2D fired on " + gameObject.name + " hit by: " + other.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("[Teleport] Player hit teleport! Loading BossPrototype...");
            SceneManager.LoadScene("BossPrototype");
        }
        else
        {
            Debug.Log("[Teleport] Something hit me, but it was NOT tagged Player. Tag was: " + other.tag);
        }
    }

    // Extra safety: if you accidentally used a non-trigger collider, this will tell us
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("[Teleport] OnCollisionEnter2D fired on " + gameObject.name + " hit by: " + collision.collider.name);
    }
}
