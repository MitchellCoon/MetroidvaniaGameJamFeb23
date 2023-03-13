using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
public enum AimType
{
    Player,
    Direction,
    Turret

}

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] float hitBoxActivetime = 0.1f;
    [SerializeField] AttackData defaultProjectileAttack;
    public float fireRate = 0.5f;
    public float checkRadiusProjectile = 10f;
    public float checkRadiusMelee = 1f;
    [SerializeField] public Vector3 rotationAsVector;
    [SerializeField] AimType aimType;
    [SerializeField] Vector2 aimAtPlayerDirection = new Vector2(1, 1);
    [SerializeField] List<Vector2> directionsToFire;
    [SerializeField] float projectileSpeed = 10f;
    [Space]
    [Space]
    [SerializeField] Sound shootSound;
    [SerializeField] Sound meleeSound;

    // Later write logic where we get signals to stop firing
    public bool canFire = true;
    // Only enable if we have a melee attack
    public bool canMelee = false;
    [SerializeField]
    Hitbox hitbox;
    [SerializeField]
    AttackData defaultMeleeAttack;
    // Start is called before the first frame update

    [SerializeField] Animator animator;

    void Start()
    {
        InvokeRepeating("ShootLogic", 0f, fireRate);
    }

    void ShootLogic()
    {
        switch (aimType)
        {
            case AimType.Player:
                aimAtPlayer();
                break;
            case AimType.Direction:
                aimInDirection();
                break;
            case AimType.Turret:
                AimTurret();
                //fireProjectile(transform.parent.right);
                break;
            default:
                break;
        }
        // aimAtPlayer();
    }
    void AimTurret()
    {
       
        fireProjectile(new Vector2(0,0));
    }
    void aimInDirection()
    {

        foreach (Vector2 direction in directionsToFire)
        {
            fireProjectile(direction);
        }

    }

    private void aimAtPlayer()
    {
        Collider2D[] hitColliders = null;
        // Melee attack
        hitColliders = Physics2D.OverlapCircleAll(transform.position, checkRadiusProjectile);
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].CompareTag(Constants.PLAYER_TAG))
            {
                // get player distance from enemy
                // prefer melee attack if player is close
                // else fire projectile
                float distanceToPlayer = math.abs(hitColliders[i].transform.position.x - transform.position.x);
                if (distanceToPlayer < checkRadiusMelee && defaultMeleeAttack != null)
                {
                    MeleeAttack();
                }
                else

                {
                    Vector2 fireDirection;
                    if ((hitColliders[i].transform.position.x - transform.position.x) < 0)
                    {
                        fireDirection = aimAtPlayerDirection * -1; //new Vector2(-1, -1);
                    }
                    else
                    {
                        fireDirection = aimAtPlayerDirection; //new Vector2(1, -1);
                    }

                    fireProjectile(fireDirection);
                }

                break;
            }
            i++;
        }
    }
    private void MeleeAttack()
    {
        if (canMelee == false)
        {
            return;
        }

        if (meleeSound != null) meleeSound.Play();

        animator.SetTrigger(Constants.MELEE_ATTACK_ANIMATION);
        hitbox.UpdateAttackData(defaultMeleeAttack);
        hitbox.gameObject.SetActive(true);
        StartCoroutine(DisableHitbox());
        //        hitbox.gameObject.SetActive(false);

    }
    IEnumerator DisableHitbox()
    {
        yield return new WaitForSeconds(hitBoxActivetime);
        hitbox.gameObject.SetActive(false);
    }

    public void fireProjectile(Vector2 fireDirection)
    {
        if (canFire == false)
        {
            return;
        }

        if (shootSound != null) shootSound.Play();

        Quaternion rotation = Quaternion.identity;
        if (aimType == AimType.Turret)
        {
            rotation = transform.rotation;
           // rotation = Quaternion.Euler(transform.right) ;

        }
        else
        {
            rotation = Quaternion.Euler(rotationAsVector);
        }

        GameObject projectile = Instantiate(defaultProjectileAttack.projectilePrefab, transform.position, defaultProjectileAttack.projectilePrefab.transform.rotation * rotation);

        var enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        enemyProjectile.projectileAttackData = defaultProjectileAttack;

        if( aimType == AimType.Turret)
        {
            enemyProjectile.turretProjectile = true ; 
        }   else {
                    projectile.GetComponent<Rigidbody2D>().velocity = (fireDirection).normalized * projectileSpeed;

        }

    }


}
