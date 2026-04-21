using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttack : MonoBehaviour
{
    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;
    public int damage;
    [SerializeField] Animator _animator; 

    // Attack functionality
    void Attack()
    {
        Collider2D[] enemy = Physics2D.OverlapCircleAll(attackPoint.transform.position, radius, enemies);
        _animator.SetTrigger("Attack");

        foreach (Collider2D enemyGameObject in enemy)
        {
            Debug.Log("Hit!");

            if (enemyGameObject.CompareTag("Enemy"))
            {
                // This was changed to use EnemyHealth1 because making a separate flash script caused a duplicate class/name conflict in the project.
                // Using the health script that already exists on the enemy lets the attack still find the right component and run damage plus the red flash together.
                var enemyHealthComponent = enemyGameObject.GetComponent<EnemyHealth1>();

                if (enemyHealthComponent != null)
                {
                    enemyHealthComponent.EnemyTakeDamage(damage);
                }
                else
                {
                    Debug.LogWarning(enemyGameObject.name + " is missing EnemyHealth1.");
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