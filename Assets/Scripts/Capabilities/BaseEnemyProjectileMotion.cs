using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyProjectileMotion : MonoBehaviour
{
    [SerializeField] AttackData attackData;
    
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
}