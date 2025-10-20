using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttack : MonoBehaviour
{
    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;
    public int damage;

    //Attack functionality
    void Attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);

        foreach (Collider2D enemyGameObject in enemy)
        {
            Debug.Log("Hit!");
            if (enemyGameObject.CompareTag("Enemy"))
            {
                var enemyHealthComponent = enemyGameObject.GetComponent<EnemyHealth>();

                if (enemyHealthComponent != null)
                {
                    enemyHealthComponent.EnemyTakeDamage(damage);
                }
            }
                    
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
    }
}
