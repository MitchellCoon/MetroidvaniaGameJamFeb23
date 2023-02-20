using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretRotate : MonoBehaviour
{
    GameObject playerGameObject;
    public float maxDistance ; 
    public EnemyAttack enemyAttack;
    Quaternion initalRotation ;
    void Start()
    {
    playerGameObject =    GameObject.FindWithTag("Player");
   enemyAttack =  GetComponentInChildren<EnemyAttack>(); 
   initalRotation = transform.rotation; 
        
    }

    // Update is called once per frame
    void Update()
    {   // get distance between player and turret

        Vector3.Distance(playerGameObject.transform.position, transform.position);
        if (Vector3.Distance(playerGameObject.transform.position, transform.position) < maxDistance)
        {
            enemyAttack.canFire = true;
        }else {
           enemyAttack.canFire = false;
           return ; 
        }
         Vector3 dir = playerGameObject.transform.position - transform.position;

        float angle = Mathf.Atan2(dir.y,dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward); 
        
    }
}
