using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] AttackData attackData;
    public bool isEnemyHitbox = false;
    public void UpdateAttackData(AttackData newAttackData)
    {
        attackData = newAttackData;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!isEnemyHitbox &&  other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().TakeDamage(attackData.damage, transform.position, attackData.knockback);
        }

        if( isEnemyHitbox && other.CompareTag("Player"))
        {
            other.GetComponent<PlayerCombat>().TakeDamage(attackData, transform.position);
        }
        if (other.CompareTag("Interactable"))
        {
            if (other.TryGetComponent<IInteractable>(out var interactable))
            {
                interactable.Use();
            }
            else
            {
                Debug.LogError($"{gameObject.name} has \"Interactable\" tag but needs a component that implements the Interactable interface");
            }
        }
    }
}
