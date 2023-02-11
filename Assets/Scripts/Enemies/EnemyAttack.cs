using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] AttackData activeAttack;
    public float checkRate = 0.5f; 
    public float checkRadiusProjectile = 10f;
    public float checkRadiusMelee = 1f;
     [SerializeField]
    public Vector3 rotationAsVector;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CheckForPlayer", 0f, checkRate);
    }

    void CheckForPlayer () {
         Collider2D[] hitColliders = null ; 
        if( activeAttack.attackType == AttackType.Projectile){
            // Melee attack
             hitColliders = Physics2D.OverlapCircleAll(transform.position, checkRadiusProjectile);
        }
        else if( activeAttack.attackType == AttackType.Melee){
            // Melee attack
             hitColliders = Physics2D.OverlapCircleAll(transform.position, checkRadiusMelee);
        }
       // = Physics2D.OverlapCircleAll(transform.position, checkRadiusProjectile);
        int i = 0;
        while (i < hitColliders.Length)
        {   
            if (hitColliders[i].CompareTag("Player"))
            {
                if( activeAttack.attackType == AttackType.Projectile){
               
               var  rotation =  Quaternion.Euler( rotationAsVector); 
                
                GameObject projectile = Instantiate(activeAttack.projectilePrefab, transform.position, rotation);
                projectile.transform.parent = transform;
                Vector2 fireDirection ; 
                if( (hitColliders[i].transform.position.x - transform.position.x) < 0){
                    fireDirection = new Vector2(-1,-1);
                }
                else{
                    fireDirection = new Vector2(1,1);
                }
                projectile.GetComponent<Rigidbody2D>().velocity = (fireDirection).normalized * 10f;
                projectile.GetComponent<EnemyProjectile>().projectileAttackData = activeAttack;
                }
                else if( activeAttack.attackType == AttackType.Melee){
                    // Melee attack
                }
                break;
            }
            i++;
        }
    }


    // Update is called once per frame
}
