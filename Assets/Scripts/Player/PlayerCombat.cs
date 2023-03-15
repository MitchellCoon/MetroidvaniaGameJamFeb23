using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HealthStatus {Healthy, Hurt, Peril};

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] bool isEnemy = false;
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController healthyAnimator;
    [SerializeField] RuntimeAnimatorController hurtAnimator;
    [SerializeField] RuntimeAnimatorController perilAnimator;
    [SerializeField] ResourceManager resources;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float weightMultiplier = 1f;
    [Space]
    [Space]
    [SerializeField] Resource health;
    [SerializeField] int perilThreshold = 3;
    [SerializeField] int hurtThreshold = 7;
    [Space]
    [Space]
    [SerializeField] Sound hurtSound;
    [SerializeField] Sound deathSound;

    [SerializeField] PossessionManager possessionManager;
    [SerializeField] Animator enemySlimePossessionAnimator;
    [SerializeField] RuntimeAnimatorController healthySlimePossessionAnimator;
    [SerializeField] RuntimeAnimatorController hurtSlimePossessionAnimator;
    [SerializeField] RuntimeAnimatorController perilSlimePossessionAnimator;
    [SerializeField] Attack attack;
    [SerializeField] AttackData healthySlimeProjectileAttackData;
    [SerializeField] AttackData hurtSlimeProjectileAttackData;
    [SerializeField] AttackData perilSlimeProjectileAttackData;

    PlayerMain playerMain;

    bool isAlive;

    public bool IsEnemy => isEnemy;

    void OnEnable()
    {
        GlobalEvent.OnRoomLoaded += OnRoomLoaded;
        GlobalEvent.OnEmergencyPlayerInstakillSomethingWentHorriblyWrong += OnEmergencyPlayerInstakillSomethingWentHorriblyWrong;
    }

    void OnDisable()
    {
        GlobalEvent.OnRoomLoaded -= OnRoomLoaded;
        GlobalEvent.OnEmergencyPlayerInstakillSomethingWentHorriblyWrong -= OnEmergencyPlayerInstakillSomethingWentHorriblyWrong;
    }

    void Awake()
    {
        playerMain = GetComponent<PlayerMain>();
    }

    void Start()
    {
        isAlive = true;
        if (!isEnemy) GlobalEvent.Invoke.OnPlayerSpawn(playerMain);
    }

    void OnRoomLoaded(Vector2 obj)
    {
        // we need to re-broadcast the player's existence to the world, since this is a brand-new room
        if (!isEnemy) GlobalEvent.Invoke.OnPlayerSpawn(playerMain);
    }

    public void TakeDamage(AttackData attackData, Vector3 attackOrigin)
    {
        health.SubtractResource(attackData.damage);

        //animator.SetTrigger("Hurt");

        UpdateSlimeAnimatorAndAttack();

        Vector2 adjustedForce = attackData.knockbackForce * weightMultiplier * (transform.position - attackOrigin).normalized;

        rb.AddForce(adjustedForce, ForceMode2D.Impulse);

        hurtSound.Play();

        if (health.GetCurrentValue() <= 0)
        {
            Die();
        }
    }

    public int GetHealthValue()
    {
        return health.GetCurrentValue();
    }

    public void SetHealthValue(int newValue)
    {
        health.SetCurrentValue(newValue);
        UpdateSlimeAnimatorAndAttack();
    }

    public HealthStatus GetHealthStatus()
    {
        if (health.GetCurrentValue() > hurtThreshold)
        {
            return HealthStatus.Healthy;
        }
        else if (health.GetCurrentValue() > perilThreshold)
        {
            return HealthStatus.Hurt;
        }
        else
        {
            return HealthStatus.Peril;
        }
    }

    public void UpdateSlimeAnimatorAndAttack()
    {
        if(!isEnemy)
        {
            if (health.GetCurrentValue() <= perilThreshold)
            {
                animator.runtimeAnimatorController = perilAnimator;
                attack.SetDefaultAttackData(perilSlimeProjectileAttackData);
            }
            else if (health.GetCurrentValue() <= hurtThreshold)
            {
                animator.runtimeAnimatorController = hurtAnimator;
                attack.SetDefaultAttackData(hurtSlimeProjectileAttackData);
            }
            else
            {
                animator.runtimeAnimatorController = healthyAnimator;
                attack.SetDefaultAttackData(healthySlimeProjectileAttackData);
            }
        }
        else
        {
            if (possessionManager == null || !possessionManager.IsPossessed()) return;
            if (health.GetCurrentValue() <= perilThreshold)
            {
                enemySlimePossessionAnimator.runtimeAnimatorController = perilSlimePossessionAnimator;
            }
            else if (health.GetCurrentValue() <= hurtThreshold)
            {
                enemySlimePossessionAnimator.runtimeAnimatorController = hurtSlimePossessionAnimator;
            }
            else
            {
                enemySlimePossessionAnimator.runtimeAnimatorController = healthySlimePossessionAnimator;
            }
        }
    }

    void OnEmergencyPlayerInstakillSomethingWentHorriblyWrong()
    {
        Die();
    }

    void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        deathSound.Play();
        GlobalEvent.Invoke.OnPlayerDeath();
        GetComponent<Move>().enabled = false;
        GetComponent<Jump>().enabled = false;
        GetComponent<Attack>().enabled = false;
        GetComponent<Dash>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<PlayerMovementController>().enabled = false;
        this.enabled = false;
        // remove player tag so that FindWithTag will find the next player that gets spawned in, not the dead one.
        gameObject.tag = Constants.UNTAGGED;
        gameObject.layer = Layer.Parse(Constants.DEFAULT_LAYER);
        StartCoroutine(DeathFX());
    }

    void Cleanup()
    {
        Destroy(gameObject);
    }

    IEnumerator DeathFX()
    {
        while (deathSound.isPlaying) yield return null;
        Cleanup();
    }
}
