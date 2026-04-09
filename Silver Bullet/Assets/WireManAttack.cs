using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManAttack : MonoBehaviour
{
    [SerializeField] float grabRange = 5f;
    [SerializeField] GameObject grabZone;
    [SerializeField] GameObject player;
    [SerializeField] float grabResetTimer;
    [SerializeField] int damageAmount = 1;

    bool playerTookDamage = false;

    // Update is called once per frame
    void Update()
    {
        float dist = Vector2.Distance(player.transform.position, transform.position);

        if (dist <= grabRange)
        {
            grabZone.SetActive(true);
            Debug.Log("Grabbing player");
        }

        else if (dist >= grabRange)
        {
            grabZone.SetActive(false);
            Debug.Log("Player not in range");
        }

        resetOnDamage();

    }

    private void OnCollisionEnter2D(Collision2D collision) //damages player on contact with wire man
    {

        var playerHealthComponent = player.GetComponent<PlayerHealth>();

        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealthComponent.PlayerTakeDamage(damageAmount);

            playerTookDamage = true;
        }
    }

    void resetOnDamage() //resets the grab so the player can escape the grab zone
    {
        if (playerTookDamage)
        {
            grabRange = -10f; //prevents grab zone from being active next to player
            grabResetTimer = Time.time;

            if (grabResetTimer >= 10f)
            {
                grabRange = 5f;
            }
        }
    }
}
