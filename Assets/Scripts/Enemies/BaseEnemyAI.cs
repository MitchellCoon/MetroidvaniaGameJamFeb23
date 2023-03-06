using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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
    Vector2 velocity;
    bool isFacingRight = true;
    float horizontalMove = 0.0f;
    float maxSpeedChange;
    float nextMeleeTime = 0f;
    float nextProjectileTime = 0f;
    float pollInterval = 5f;

    // Values used for animations:

    [SerializeField] bool isFiringProjectile = false;
    [SerializeField] bool isAttacking = false;

    void Awake()
    {
        Assert.IsNotNull(animator);
    }

    void Start()
    {
        StartCoroutine(FindingPlayer(pollInterval));
    }

    void Update()
    {
        if (player == null)
        {
            return;
        }
        float distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        if (distanceToPlayer < detectionRadius && distanceToPlayer > meleeRange && !isAttacking && !isFiringProjectile)
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
                nextProjectileTime = Time.time + projectileAttackData.duration;
                animator.SetTrigger(Constants.PROJECTILE_ATTACK_ANIMATION);
            }
        }
        else
        {
            if (distanceToPlayer < meleeRange && Time.time >= nextMeleeTime && !isFiringProjectile)
            {
                nextMeleeTime = Time.time + meleeAttackData.duration;
                animator.SetTrigger(Constants.MELEE_ATTACK_ANIMATION);
            }
        }
    }

    void FixedUpdate()
    {
        if (isAttacking || isFiringProjectile || player == null)
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

    IEnumerator FindingPlayer(float pollInterval) {
        while (true)
        {
            player = FindPlayer();
            yield return new WaitForSeconds(pollInterval);
        }
    }

    PlayerMovementController FindPlayer()
    {
        if (player != null)
        {
            return player;
        }
        GameObject playerObj = GameObject.FindWithTag(Constants.PLAYER_TAG);
        if (playerObj == null)
        {
            return null;
        }
        return playerObj.GetComponent<PlayerMovementController>();
    }
    
}