using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgementCatEnd : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] BaseEnemyAI enemyAI;
    [SerializeField] Rigidbody2D body;
    [SerializeField] PossessionManager possessionManager;
    [SerializeField] Move move;
    [SerializeField] Jump jump;
    [SerializeField] Attack attack;

    [SerializeField] float chargeTime = 2f;
    [SerializeField] float napTime = 4f;

    bool isCharging = false;
    bool isNapping = false;
    float chargeTimer = 0f;
    float napTimer = 0f;
    

    void Update()
    {
        if (MInput.GetKeyDown(KeyCode.J) || MInput.GetPadDown(GamepadCode.ButtonSouth))
        {
            if (possessionManager.IsPossessed())
            {
                move.enabled = false;
                jump.enabled = false;
                attack.enabled = false;
                possessionManager.enabled = false;
            }
            else
            {
                enemyAI.enabled = false;
            }
            body.velocity = Vector3.zero;
            chargeTimer = 0f;
            isCharging = true;
            animator.SetTrigger(Constants.JUDGEMENT_CAT_END_CHARGE_ANIMATION);
            animator.SetBool(Constants.JUDGEMENT_CAT_END_SLASH_ANIMATION, false);
            animator.SetBool(Constants.IS_MOVING_BOOL, false);
        }
        if (isCharging)
        {
            chargeTimer += Time.deltaTime;
        }
        if(chargeTimer > chargeTime)
        {
            isCharging = false;
            chargeTimer = 0f;
            animator.SetTrigger(Constants.JUDGEMENT_CAT_END_SLASH_ANIMATION);
            napTimer = 0f;
            isNapping = true;
        }
        if(isNapping)
        {
            napTimer += Time.deltaTime;
            body.velocity = Vector3.zero;
            animator.SetBool(Constants.IS_MOVING_BOOL, false);
        }
        if(napTimer > napTime)
        {
            if (possessionManager.IsPossessed())
            {
                move.enabled = true;
                jump.enabled = true;
                attack.enabled = true;
                possessionManager.enabled = true;
            }
            else
            {
                enemyAI.enabled = true;
            }
            isNapping = false;
            napTimer = 0f;
        }
    }
}
