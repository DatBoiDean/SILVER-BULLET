using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAttackJJSplayground : MonoBehaviour
{
    public GameObject attackPoint;
    public float radius;
    public LayerMask enemies;
    public int damage;
    [SerializeField] Animator _animator;

    // Attack functionality
    void Attack()
    {
        // Play the player's attack animation
        if (_animator != null)
        {
            _animator.SetTrigger("Attack");
        }

        // Find every enemy collider inside the attack circle
        Collider2D[] enemyHits = Physics2D.OverlapCircleAll(
            attackPoint.transform.position,
            radius,
            enemies
        );

        foreach (Collider2D enemyGameObject in enemyHits)
        {
            Debug.Log("Hit collider: " + enemyGameObject.name);

            // This finds the health script on the enemy root,
            // even if the collider we hit belongs to a child object
            EnemyHealthJJSplayground enemyHealthComponent =
                enemyGameObject.GetComponentInParent<EnemyHealthJJSplayground>();

            if (enemyHealthComponent != null)
            {
                Debug.Log("Damaged enemy: " + enemyHealthComponent.gameObject.name);
                enemyHealthComponent.EnemyTakeDamage(damage);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Attack();
        }
    }

    private void OnDrawGizmos()
    {
        if (attackPoint != null)
        {
            Gizmos.DrawWireSphere(attackPoint.transform.position, radius);
        }
    }
}