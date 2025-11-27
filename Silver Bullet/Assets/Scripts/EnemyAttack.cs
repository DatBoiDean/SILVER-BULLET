using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float attackRange;
    public int damageInterval;
    public int damageAmount;
    public float nextDamageTime;
    public GameObject player;
void Start()
    {
        player = GameObject.Find("Player"); 
    }

void Update()
    {
        //This method uses distance measuring to attack the player, instead of simply being in contact with the collider
        float dist = Vector3.Distance(player.transform.position, transform.position);
        if (dist <= attackRange)
        {
            if (Time.time >= nextDamageTime)
            {
                var playerHealthComponent = player.GetComponent<PlayerHealth>();

                if (playerHealthComponent != null)
                {
                    Debug.Log("Player health component found");
                    playerHealthComponent.PlayerTakeDamage(damageAmount);

                    nextDamageTime = Time.time + damageInterval;
                }
            }
        }

        if (dist > attackRange)
        {
            nextDamageTime = 0;
        }
    }

        //Commented all of this out so I can do a distance measure for attacking range,
        //      instead of using triggers. 

    // private void OnTriggerStay2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))  //check if collision has player tag
    //     {
    //         if (Time.time >= nextDamageTime)
    //         {
    //             var playerHealthComponent = other.GetComponent<PlayerHealth>();

    //             if (playerHealthComponent != null)
    //             {
    //                 Debug.Log("Player health component found");
    //                 playerHealthComponent.PlayerTakeDamage(damageAmount);

    //                 nextDamageTime = Time.time + damageInterval;
    //             }
    //         }
    //     }
    // }

    // private void OnTriggerExit2D(Collider2D other)
    // {
    //     if (other.CompareTag("Player"))
    //     {
    //         nextDamageTime = 0;
    //     }
    // }
}
