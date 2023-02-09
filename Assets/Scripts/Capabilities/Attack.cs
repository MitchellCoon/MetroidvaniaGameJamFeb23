using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] PlayerMovementController controller;
    [SerializeField] Animator animator;
    [SerializeField] InputManager inputManager;
    [SerializeField] AudioSource playerAudio;
    [SerializeField] Hitbox hitbox;

    [SerializeField] AttackData attackData;
    [SerializeField] AttackData defaultMeleeAttack;
    [SerializeField] AttackData defaultProjectileAttack;
    
    [SerializeField] Transform projectileOrigin;
    [SerializeField] GameObject projectilePrefab;

    private float timeOfLastAttack;
    private float nextAttackTime = 0f;
    private InputManager.Input currentInput;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            inputManager.AddInputRequestToQueue(InputManager.Input.Attack1, Time.time);
            inputManager.SetPreviousPressedTime(InputManager.Input.Attack1, Time.time);
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            inputManager.AddInputRequestToQueue(InputManager.Input.Attack2, Time.time);
            inputManager.SetPreviousPressedTime(InputManager.Input.Attack2, Time.time);
        }
        if (Time.time >= nextAttackTime && inputManager.GetInputRequested(InputManager.Input.Attack1))
        {
            SetAttackData(defaultMeleeAttack);
            hitbox.UpdateAttackData(attackData);
            currentInput = InputManager.Input.Attack1;
            ExecuteAttack();
            
        }
        else if (Time.time >= nextAttackTime && inputManager.GetInputRequested(InputManager.Input.Attack2))
        {
            SetAttackData(defaultProjectileAttack);
            currentInput = InputManager.Input.Attack2;
            ExecuteAttack();
        }
    }

    public void SetAttackData(AttackData newAttackData)
    {
        attackData = newAttackData;
    }

    private void ExecuteAttack()
    {
        timeOfLastAttack = Time.time;
        inputManager.SetPreviousActionTime(InputManager.Action.Attack, Time.time);
        inputManager.RemoveInputRequestFromQueue(currentInput);
        nextAttackTime = Time.time + attackData.duration;
        animator.SetTrigger(attackData.animationName);
        //playerAudio.PlayOneShot(attackData.attackSoundEffect, 1.0f);
    }

    public void SpawnProjectile()
    {
        if(controller.IsFacingRight())
        {
            Instantiate(projectilePrefab, projectileOrigin.transform.position, transform.rotation);
        }
        else
        {
            Instantiate(projectilePrefab, projectileOrigin.transform.position, transform.rotation * Quaternion.Euler(0, 180, 0));
        }
    }
}
