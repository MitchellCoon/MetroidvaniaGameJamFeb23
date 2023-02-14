using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public  AttackData projectileAttackData;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if(projectileAttackData.destroyProjectileOnHit)
            {
                Destroy(gameObject);
            }
            other.GetComponent<PlayerCombat>().TakeDamage(projectileAttackData, transform.position);
        }
    }
 
}
