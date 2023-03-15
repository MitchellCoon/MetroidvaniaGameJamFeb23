using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] PlayerMovementController controller;
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController defaultAnimator;
    [SerializeField] InputManager inputManager;
    [SerializeField] Hitbox hitbox;

    [SerializeField] AttackData attackData;
    [SerializeField] AttackData defaultAttack;
    [SerializeField] AttackData possessAttack;
    [SerializeField] AttackData defaultMeleeAttack;
    [SerializeField] AttackData defaultProjectileAttack;

    [SerializeField] Transform projectileOrigin;
    [SerializeField] GameObject projectilePrefab;

    private float timeOfLastAttack;
    private float nextAttackTime = 0f;
    private InputManager.Input currentInput;

    public void Update()
    {
        if (MInput.GetKeyDown(KeyCode.Mouse0) || MInput.GetKeyDown(KeyCode.K) || MInput.GetPadDown(GamepadCode.ButtonWest))
        {
            inputManager.AddInputRequestToQueue(InputManager.Input.Attack1, Time.time);
            inputManager.SetPreviousPressedTime(InputManager.Input.Attack1, Time.time);
        }
        if (MInput.GetKeyDown(KeyCode.Mouse1) || MInput.GetKeyDown(KeyCode.O) || MInput.GetPadDown(GamepadCode.ButtonEast) || MInput.GetPadDown(GamepadCode.BumperRight))
        {
            inputManager.AddInputRequestToQueue(InputManager.Input.Attack2, Time.time);
            inputManager.SetPreviousPressedTime(InputManager.Input.Attack2, Time.time);
        }
        if (Time.time >= nextAttackTime && inputManager.GetInputRequested(InputManager.Input.Attack1))
        {
            SetAttackData(defaultAttack);
            if(defaultAttack.attackType == AttackType.Melee)
            {
                hitbox.UpdateAttackData(attackData);
            }
            currentInput = InputManager.Input.Attack1;
            ExecuteAttack();

        }
        else if (possessAttack != null && Time.time >= nextAttackTime && inputManager.GetInputRequested(InputManager.Input.Attack2))
        {
            SetAttackData(possessAttack);
            if(defaultAttack.attackType == AttackType.Melee)
            {
                hitbox.UpdateAttackData(attackData);
            }
            currentInput = InputManager.Input.Attack2;
            ExecuteAttack();

        }

        // else if (Time.time >= nextAttackTime && inputManager.GetInputRequested(InputManager.Input.Attack2))
        // {
        //     SetAttackData(defaultProjectileAttack);
        //     currentInput = InputManager.Input.Attack2;
        //     ExecuteAttack();
        // }
    }

    public void SetDefaultAttackData(AttackData newAttackData)
    {
        defaultAttack = newAttackData;
        projectilePrefab = newAttackData.projectilePrefab;
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
    }

    public void SpawnProjectile()
    {
        if (controller.IsFacingRight())
        {
            Instantiate(projectilePrefab, projectileOrigin.transform.position, transform.rotation);
        }
        else
        {
            Instantiate(projectilePrefab, projectileOrigin.transform.position, transform.rotation * Quaternion.Euler(0, 180, 0));
        }
    }

    public void ResetAnimator()
    {
        animator.runtimeAnimatorController = defaultAnimator;
    }
}
