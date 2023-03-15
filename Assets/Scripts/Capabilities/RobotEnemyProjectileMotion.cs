using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotEnemyProjectileMotion : BaseEnemyProjectileMotion
{
    [SerializeField] float acceleration = -10f;
    [SerializeField] float extraProjectileCooldown = 0.3f;
    [SerializeField] GameObject extraProjectilePrefab;

    float adjustedSpeed;
    float extraProjectileTimer = 0f;
    Quaternion randomRotation;


    void Start()
    {
        startPosition = transform.position;
        adjustedSpeed = attackData.projectileSpeed;
    }

    void Update()
    {

        adjustedSpeed += acceleration*Time.deltaTime;

        transform.Translate(Vector3.right * adjustedSpeed * Time.deltaTime);

        extraProjectileTimer += Time.deltaTime;

        if (extraProjectileTimer >= extraProjectileCooldown)
        {
            randomRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
            Instantiate(extraProjectilePrefab, transform.position, randomRotation);
            extraProjectileTimer = 0f;
        }


        if ((startPosition - transform.position).magnitude > attackData.projectileRange)
        {
            Destroy(gameObject);
        }
    }

}
