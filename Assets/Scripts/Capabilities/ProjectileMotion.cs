using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMotion : MonoBehaviour
{

    [SerializeField] AttackData attackData;
    [SerializeField] Sound impactSound;
    
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(Vector3.right * attackData.projectileSpeed * Time.deltaTime);
        if ((startPosition - transform.position).magnitude > attackData.projectileRange)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy") )
        {
            if (impactSound != null) impactSound.Play();
            if(attackData.destroyProjectileOnHit)
            {
                Destroy(gameObject);
            }
            if (other.TryGetComponent<BaseEnemyAI>(out var enemyAI))
            {
                enemyAI.TakeDamage(attackData, transform.position);
            }
            else if (other.TryGetComponent<MechBossAI>(out var boss))
            {
                boss.TakeDamage(attackData, transform.position);
            }
        }
    }
}
