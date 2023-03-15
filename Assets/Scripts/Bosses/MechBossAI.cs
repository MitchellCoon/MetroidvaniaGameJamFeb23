using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using CyberneticStudios.SOFramework;

public class MechBossAI : MonoBehaviour
{
    public int currentHealth;

    [SerializeField] BoolVariable hasDefeatedBoss;
    [SerializeField] Animator animator;
    [SerializeField] MovementOverride movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] Resource health;
    [SerializeField] AttackData stompData;
    [SerializeField] AttackData missileData;
    [Space]
    [Space]
    [SerializeField] float detectionRadius;
    [SerializeField] float meleeRange;
    [Space]
    [Space]
    [SerializeField] Sound stompSound;
    [SerializeField] Sound stompTelegraphSound;
    [SerializeField] Sound missileLaunchSound;
    [SerializeField] Sound hurtSound;
    [SerializeField] Sound deathSound;
    [Space]
    [Space]
    [SerializeField] BoolVariable isBossDefeated;

    PlayerMovementController player;
    Vector2 velocity;
    bool isFacingRight = true;
    float horizontalMove = 0.0f;
    float maxSpeedChange;
    float nextMeleeTime = 0f;
    float nextProjectileTime = 0f;
    float pollInterval = 5f;
    int phase2Threshold = 70;
    int phase3Threshold = 40;

    // Values used for animations:

    [SerializeField] bool isFiringProjectile = false;
    [SerializeField] bool isStomping = false;
    [SerializeField] GameObject cannon0;
    [SerializeField] GameObject cannon1;
    [SerializeField] float telegraphVerticalOffset;
    float missileOffset;
    Vector3 playerLocation;
    public enum Missile {firstMissile, secondMissile, thirdMissile, fourthMissile};
    [SerializeField] GameObject missileTelegraph;

    // phase change sprites/particles:

    [SerializeField] SpriteRenderer bodySpriteRenderer;
    [SerializeField] SpriteRenderer leg1SpriteRenderer;
    [SerializeField] SpriteRenderer leg2SpriteRenderer;
    [SerializeField] SpriteRenderer cannon1SpriteRenderer;
    [SerializeField] SpriteRenderer cannon2SpriteRenderer;
    [SerializeField] SpriteRenderer fishBowlSpriteRenderer;
    
    [SerializeField] Sprite phase2bodySprite;
    [SerializeField] Sprite phase2leg1Sprite;
    [SerializeField] Sprite phase2leg2Sprite;
    [SerializeField] Sprite phase2cannon1Sprite;
    [SerializeField] Sprite phase2cannon2Sprite;
    [SerializeField] Sprite phase2fishBowlSprite;
    [SerializeField] Sprite phase3bodySprite;
    [SerializeField] Sprite phase3fishBowlSprite;
    [SerializeField] ParticleSystem smokeParticle;
    [SerializeField] ParticleSystem sparkParticle;

    public void AnimStompTelegraphEvent() {
        if (stompTelegraphSound != null) stompTelegraphSound.Play();
    }

    public void AnimStompEvent() {
        if (stompSound != null) stompSound.Play();
    }

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
        horizontalMove = (player.transform.position - transform.position).normalized.x;
        if (distanceToPlayer < detectionRadius && distanceToPlayer > meleeRange && !isStomping && !isFiringProjectile)
        {
            FacePlayer();
            if (Time.time >= nextProjectileTime)
            {
                nextProjectileTime = Time.time + missileData.duration;
                animator.SetTrigger(Constants.PROJECTILE_ATTACK_ANIMATION);
            }
        }
        else if (distanceToPlayer < meleeRange && Time.time >= nextMeleeTime && !isFiringProjectile)
        {
            FacePlayer();
            nextMeleeTime = Time.time + stompData.duration;
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
        if (isStomping || isFiringProjectile || player == null)
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

        if (health.GetCurrentValue() <= phase3Threshold)
        {
            bodySpriteRenderer.sprite = phase3bodySprite;
            fishBowlSpriteRenderer.sprite = phase3fishBowlSprite;
            smokeParticle.Play();
            sparkParticle.Play();
        }
        else if (health.GetCurrentValue() <= phase2Threshold)
        {
            //switch to phase 2 sprites
            bodySpriteRenderer.sprite = phase2bodySprite;
            fishBowlSpriteRenderer.sprite = phase2fishBowlSprite;
            leg1SpriteRenderer.sprite = phase2leg1Sprite;
            leg2SpriteRenderer.sprite = phase2leg2Sprite;
            cannon1SpriteRenderer.sprite = phase2cannon1Sprite;
            cannon2SpriteRenderer.sprite = phase2cannon2Sprite;
        }

        if (health.GetCurrentValue() <= 0)
        {
            Die();
        } else {
            if (hurtSound != null) hurtSound.Play();
        }
    }

    public void SpawnProjectile(Missile missile)
    {
        if (missileLaunchSound != null) missileLaunchSound.Play();
        
        switch (missile)
        {
        case Missile.firstMissile:
            if (isFacingRight)
            {
                missileOffset = -3f;
            }
            else
            {
                missileOffset = 3f;
            }
            Instantiate(missileData.projectilePrefab, GetCannonPosition(0), transform.rotation).GetComponent<MechBossMissileMotion>().SetHorizontalOffset(missileOffset);
            break;
        case Missile.secondMissile:
            if (isFacingRight)
            {
                missileOffset = -1f;
            }
            else
            {
                missileOffset = 1f;
            }
            Instantiate(missileData.projectilePrefab, GetCannonPosition(1), transform.rotation).GetComponent<MechBossMissileMotion>().SetHorizontalOffset(missileOffset);
            break;
        case Missile.thirdMissile:
            if (isFacingRight)
            {
                missileOffset = 1f;
            }
            else
            {
                missileOffset = -1f;
            }
            Instantiate(missileData.projectilePrefab, GetCannonPosition(0), transform.rotation).GetComponent<MechBossMissileMotion>().SetHorizontalOffset(missileOffset);
            break;
        case Missile.fourthMissile:
            if (isFacingRight)
            {
                missileOffset = 3f;
            }
            else
            {
                missileOffset = -3f;
            }
            Instantiate(missileData.projectilePrefab, GetCannonPosition(1), transform.rotation).GetComponent<MechBossMissileMotion>().SetHorizontalOffset(missileOffset);
            break;
        default:
            break;
        }
        playerLocation = player.transform.position;
        Instantiate(missileTelegraph, new Vector3(playerLocation.x + missileOffset, transform.position.y - telegraphVerticalOffset, 0), transform.rotation);
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
        if (deathSound != null) deathSound.Play();
        if (isBossDefeated != null) isBossDefeated.value = true;

        GetComponent<BoxCollider2D>().enabled = false;
        rb.velocity = Vector3.zero;
        this.enabled = false;
        hasDefeatedBoss.value = true;
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
