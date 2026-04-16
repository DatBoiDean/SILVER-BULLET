using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireManAttack : MonoBehaviour
{
    [SerializeField] float grabRange = 5f;
    [SerializeField] GameObject grabZone;
    [SerializeField] GameObject player;
    
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

    }

    private void OnCollisionEnter2D(Collision2D collision) //damages player on contact with wire man
    {

        var playerHealthComponent = player.GetComponent<PlayerHealth>();

        if (collision.gameObject.CompareTag("Player"))
        {
            playerHealthComponent.PlayerTakeDamage(damageAmount);

            playerTookDamage = true;

            if (playerTookDamage)
            {
                StartCoroutine(resetOnDamage());
            }
        }
    }

    IEnumerator resetOnDamage()
    {
        Debug.Log("Reset on damage started");
        grabRange = -10f;
        yield return new WaitForSeconds(5f);
        grabRange = 5f;
    }
}
