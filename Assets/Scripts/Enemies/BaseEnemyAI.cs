using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BaseEnemyAI : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] MovementOverride movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Resource healthScriptableObject;
    [SerializeField] AttackData attackData;
    // [SerializeField] AttackData meleeAttackData;
    // [SerializeField] AttackData projectileAttackData;
    [SerializeField] PossessionManager possessionManager;
    [SerializeField] PlayerMovementController possessedMovementController;
    [Space]
    [Space]
    [SerializeField] float detectionRadius;
    [SerializeField] float meleeRange;
    [SerializeField] Transform projectileSpawnPoint;
    [Space]
    [Space]
    [SerializeField] Sound shootSound;
    [SerializeField] Sound hurtSound;
    [SerializeField] Sound deathSound;

    PlayerMovementController player;
    Vector2 velocity;
    Resource health;
    bool isFacingRight = true;
    bool isMoving;
    float horizontalMove = 0.0f;
    float distanceToPlayer;
    float maxSpeedChange;
    float nextMeleeTime = 0f;
    float nextProjectileTime = 0f;
    [SerializeField] float findPlayerPollInterval = 2f;

    // Values used for animations:

    [SerializeField] bool isFiringProjectile = false;
    [SerializeField] bool isAttacking = false;

    void Awake()
    {
        Assert.IsNotNull(animator);
    }

    void Start()
    {
        health = Instantiate(healthScriptableObject);
        health.Init();
    }

    void OnEnable()
    {
        StartCoroutine(FindingPlayer(findPlayerPollInterval));
    }

    void Update()
    {
        if (player == null || player == GetComponent<PlayerMovementController>())
        {
            player = null;
            return;
        }
        distanceToPlayer = Mathf.Abs(player.transform.position.x - transform.position.x);
        horizontalMove = (player.transform.position - transform.position).normalized.x;
        if(distanceToPlayer > detectionRadius) return;
        if (attackData.attackType == AttackType.Projectile && distanceToPlayer <= detectionRadius && distanceToPlayer > meleeRange && !isAttacking && !isFiringProjectile)
        {
            FacePlayer();
            if (Time.time >= nextProjectileTime)
            {
                nextProjectileTime = Time.time + attackData.duration;
                animator.SetTrigger(Constants.PROJECTILE_ATTACK_ANIMATION);
            }
        }
        else if (attackData.attackType == AttackType.Melee && distanceToPlayer < meleeRange && Time.time >= nextMeleeTime && !isFiringProjectile)
        {
            FacePlayer();
            nextMeleeTime = Time.time + attackData.duration;
            animator.SetTrigger(Constants.MELEE_ATTACK_ANIMATION);
        }
    }

    void FacePlayer()
    {
        if (horizontalMove > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (horizontalMove < 0 && isFacingRight)
        {
            Flip();
        }
    }

    void FixedUpdate()
    {
        if(distanceToPlayer > detectionRadius) return;
        if (isAttacking || isFiringProjectile || player == null)
        {
            horizontalMove = 0f;
        }
        FacePlayer();
        Move(horizontalMove * movement.maxSpeed * Time.fixedDeltaTime);
    }

    void Move(float move)
    {
        velocity = rb.velocity;
        maxSpeedChange = movement.maxAcceleration * Time.fixedDeltaTime;
		Vector3 desiredVelocity = new Vector2(move * 10f, rb.velocity.y);
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
        rb.velocity = velocity;
        isMoving = Mathf.Abs(velocity.x) > Mathf.Epsilon ? true : false;
        animator.SetBool("isMoving", isMoving);
    }

    public bool IsFacingRight()
	{
		return isFacingRight;
	}

    public void SetIsFacingRight(bool sourceIsFacingRight)
	{
		isFacingRight = sourceIsFacingRight;
	}

    public void Flip()
	{
		isFacingRight = !isFacingRight;
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
        if(possessedMovementController != null)
        {
            possessedMovementController.SetIsFacingRight(isFacingRight);
        }
	}

    public void ResetDirection()
	{
		if ((isFacingRight && transform.localScale.x < 0) || (!isFacingRight && transform.localScale.x > 0))
		{
			Flip();
            isFacingRight = !isFacingRight;
            if(possessedMovementController != null)
            {
                possessedMovementController.SetIsFacingRight(isFacingRight);
            }
		} 
	}

    public void TakeDamage(AttackData incomingAttackData, Vector3 attackOrigin)
    {
        health.SubtractResource(incomingAttackData.damage);

        if (health.GetCurrentValue() <= 0)
        {
            Die();
        } else {
            if (hurtSound != null) hurtSound.Play();
        }
    }

    public void SpawnEnemyProjectile()
    {
        if (shootSound != null) shootSound.Play();

        if (possessionManager.IsPossessed())
        {
            isFacingRight = possessedMovementController.IsFacingRight();
        }
        if (isFacingRight)
        {
            Instantiate(attackData.projectilePrefab, projectileSpawnPoint.position, transform.rotation).GetComponent<Hitbox>().SetPossessionManager(possessionManager);
        }
        else
        {
            Instantiate(attackData.projectilePrefab, projectileSpawnPoint.position, transform.rotation * Quaternion.Euler(0, 180, 0)).GetComponent<Hitbox>().SetPossessionManager(possessionManager);
        }
    }

    void Die()
    {
        if (deathSound != null) deathSound.Play();
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
