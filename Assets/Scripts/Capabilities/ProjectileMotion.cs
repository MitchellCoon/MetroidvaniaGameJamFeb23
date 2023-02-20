using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{

    [SerializeField] AttackData attackData;
    
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        
        transform.Translate( transform.rotation * Vector3.right * attackData.projectileSpeed * Time.deltaTime);
        if ((startPosition - transform.position).magnitude > attackData.projectileRange)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") )
        {
            if(attackData.destroyProjectileOnHit)
            {
                Destroy(gameObject);
            }
            other.GetComponent<Enemy>().TakeDamage(attackData.damage, transform.position, attackData.knockback);
        }
    }
}
