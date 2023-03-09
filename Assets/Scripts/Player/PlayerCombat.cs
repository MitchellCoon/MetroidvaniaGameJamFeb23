using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] bool isEnemy = false;
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController defaultAnimator;
    [SerializeField] RuntimeAnimatorController hurtAnimator;
    [SerializeField] RuntimeAnimatorController perilAnimator;
    [SerializeField] ResourceManager resources;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float weightMultiplier = 1f;

    [SerializeField] Resource health;
    [SerializeField] int perilThreshold = 3;
    [SerializeField] int hurtThreshold = 7;

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

        if(!isEnemy)
        {
            if (health.GetCurrentValue() <= perilThreshold)
            {
                animator.runtimeAnimatorController = perilAnimator;
            }
            else if (health.GetCurrentValue() <= hurtThreshold)
            {
                animator.runtimeAnimatorController = hurtAnimator;
            }
        }

        Vector2 adjustedForce = attackData.knockbackForce * weightMultiplier * (attackOrigin - transform.position).normalized;

        rb.AddForce(adjustedForce, ForceMode2D.Impulse);

        if (health.GetCurrentValue() <= 0)
        {
            Die();
        }
    }

    public void ResetAnimator()
    {
        animator.runtimeAnimatorController = defaultAnimator;
    }

    void OnEmergencyPlayerInstakillSomethingWentHorriblyWrong()
    {
        Die();
    }

    void Die()
    {
        if (!isAlive) return;
        isAlive = false;
        GlobalEvent.Invoke.OnPlayerDeath();
        GetComponent<Move>().enabled = false;
        GetComponent<Jump>().enabled = false;
        GetComponent<Attack>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<PlayerMovementController>().enabled = false;
        this.enabled = false;
        // remove player tag so that FindWithTag will find the next player that gets spawned in, not the dead one.
        gameObject.tag = Constants.UNTAGGED;
        DeathFX();
    }

    void DeathFX()
    {
        Destroy(gameObject);
    }
}
