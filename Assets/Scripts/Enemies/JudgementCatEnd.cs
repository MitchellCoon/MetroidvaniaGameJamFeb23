using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JudgementCatEnd : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] BaseEnemyAI enemyAI;
    [SerializeField] Rigidbody2D body;

    [SerializeField] float chargeTime = 2f;

    bool isCharging = false;
    float chargeTimer = 0f;
    

    void Update()
    {
        if (MInput.GetKeyDown(KeyCode.J) || MInput.GetPadDown(GamepadCode.ButtonSouth))
        {
            enemyAI.enabled = false;
            body.velocity = Vector3.zero;
            isCharging = true;
            animator.SetTrigger(Constants.JUDGEMENT_CAT_END_CHARGE_ANIMATION);
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
        }
    }
}
