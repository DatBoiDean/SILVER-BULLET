using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Suction : MonoBehaviour
{

    [SerializeField] float suctionDist;
    [SerializeField] float suctionStrength;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            Vector2 distanceToFanHead = transform.position - collision.transform.position; //calculate distance from object to fan

            float distance = distanceToFanHead.magnitude; //calculates distance and force strength

            if (distance < suctionDist)
            {
                Vector3 normalizedDirection = distanceToFanHead.normalized; // Normalize the direction vector

                float forceMagnitude = suctionStrength * (distance * distance); // Calculate force magnitude, stronger when closer

                rb.AddForce(normalizedDirection * forceMagnitude); // Apply force to the rigidbody
            }
        }
    }
}
