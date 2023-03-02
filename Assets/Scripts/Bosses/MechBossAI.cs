using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MechBossAI : MonoBehaviour
{
    public int currentHealth;

    [SerializeField] Animator animator;
    [SerializeField] MovementOverride movement;
    [SerializeField] PlayerMovementController player;
    [SerializeField] PlayerCombat playerCombat;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Resource health;
    [SerializeField] AttackData stompData;
    [SerializeField] AttackData missileData;

    [SerializeField] float detectionRadius;
    [SerializeField] float meleeRange;

    Vector2 velocity;
    bool isFacingRight = true;
    float horizontalMove = 0.0f;
    float maxSpeedChange;
    float nextMeleeTime = 0f;
    float nextProjectileTime = 0f;

    // Values used for animations:

    [SerializeField] bool isFiringProjectile = false;
    [SerializeField] bool isStomping = false;
    [SerializeField] GameObject cannon0;
    [SerializeField] GameObject cannon1;

    void Update()
    {
        float distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distanceToPlayer < detectionRadius && distanceToPlayer > meleeRange && !isStomping && !isFiringProjectile)
        {
            horizontalMove = (player.transform.position - transform.position).normalized.x;
            if (horizontalMove > 0 && !isFacingRight)
            {
                Flip();
            }
            else if (horizontalMove < 0 && isFacingRight)
            {
                Flip();
            }
            if (Time.time >= nextProjectileTime)
            {
                nextProjectileTime = Time.time + missileData.duration;
                animator.SetTrigger("ProjectileAttack");
            }
        }
        else
        {
            if (distanceToPlayer < meleeRange && Time.time >= nextMeleeTime && !isFiringProjectile)
            {
                nextMeleeTime = Time.time + stompData.duration;
                animator.SetTrigger("MeleeAttack");
            }
        }
    }

    void FixedUpdate()
    {
        if (isStomping || isFiringProjectile)
        {
            horizontalMove = 0f;
        }
        Move(horizontalMove * movement.maxSpeed * Time.fixedDeltaTime);
    }

    void Move(float move)
    {
        velocity = rb.velocity;
        maxSpeedChange = movement.maxAcceleration * Time.fixedDeltaTime;
		Vector3 desiredVelocity = new Vector2(move * 10f, rb.velocity.y);
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        rb.velocity = velocity;
    }

    private void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
	}

    public void TakeDamage(AttackData attackData, Vector3 attackOrigin)
    {
        health.SubtractResource(attackData.damage);

        if (health.GetCurrentValue() <= 0)
        {
            Die();
        }
    }

    public void SpawnProjectile(int cannonIndex)
    {
        Instantiate(missileData.projectilePrefab, GetCannonPosition(cannonIndex), transform.rotation);
    }

    public Vector3 GetCannonPosition(int cannonIndex)
    {
        if (cannonIndex == 0)
        {
            return cannon0.transform.position;
        }
        else
        {
            return cannon1.transform.position;
        }
    }

    void Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        rb.velocity = Vector3.zero;
        this.enabled = false;
    }

}
