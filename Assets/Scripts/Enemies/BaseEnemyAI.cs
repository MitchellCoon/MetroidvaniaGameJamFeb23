using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemyAI : MonoBehaviour
{
    public int currentHealth;

    [SerializeField] Animator animator;
    [SerializeField] MovementOverride movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Resource health;
    [SerializeField] AttackData meleeAttackData;
    [SerializeField] AttackData projectileAttackData;

    [SerializeField] float detectionRadius;
    [SerializeField] float meleeRange;
    [SerializeField] Transform projectileSpawnPoint;

    PlayerMovementController player;
    PlayerCombat playerCombat;

    Vector2 velocity;
    bool isFacingRight = true;
    float horizontalMove = 0.0f;
    float maxSpeedChange;
    float nextMeleeTime = 0f;
    float nextProjectileTime = 0f;

    // Values used for animations:

    [SerializeField] bool isFiringProjectile = false;
    [SerializeField] bool isAttacking = false;

    void FindPlayer()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag(Constants.PLAYER_TAG).GetComponent<PlayerMovementController>();
        }
        if (playerCombat == null)
        {
            playerCombat = GameObject.FindWithTag(Constants.PLAYER_TAG).GetComponent<PlayerCombat>();
        }
    }

    void Update()
    {
        FindPlayer();
        float distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distanceToPlayer < detectionRadius && distanceToPlayer > meleeRange && !isAttacking && !isFiringProjectile)
        {
            Debug.Log("approaching player");
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
                Debug.Log("fire");
                nextProjectileTime = Time.time + projectileAttackData.duration;
                animator.SetTrigger("ProjectileAttack");
            }
        }
        else
        {
            if (distanceToPlayer < meleeRange && Time.time >= nextMeleeTime && !isFiringProjectile)
            {
                Debug.Log("attack");
                nextMeleeTime = Time.time + meleeAttackData.duration;
                animator.SetTrigger("MeleeAttack");
            }
        }
    }

    void FixedUpdate()
    {
        if (isAttacking || isFiringProjectile)
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

    public void SpawnEnemyProjectile()
    {
        if (isFacingRight)
        {
            Instantiate(projectileAttackData.projectilePrefab, projectileSpawnPoint.position, transform.rotation);
        }
        else
        {
            Instantiate(projectileAttackData.projectilePrefab, projectileSpawnPoint.position, transform.rotation * Quaternion.Euler(0, 180, 0));
        }
    }

    void Die()
    {
        GetComponent<BoxCollider2D>().enabled = false;
        rb.velocity = Vector3.zero;
        this.enabled = false;
    }
    
}
