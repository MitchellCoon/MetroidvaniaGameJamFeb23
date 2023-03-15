using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleTransitionAnimation : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float transitionTimeThreshold;

    int isMovingID;
    float idleTimer = 0f;
     
    void Start()
    {
        isMovingID = Animator.StringToHash(Constants.IS_MOVING_BOOL);
    }

    void Update()
    {
        if (!animator.GetBool(isMovingID))
        {
            idleTimer += Time.deltaTime;
        }
        else
        {
            idleTimer = 0f;
            animator.SetBool(Constants.IDLE_TRANSITION_ANIMATION, false);
            return;
        }
        if(idleTimer >= transitionTimeThreshold)
        {
            animator.SetTrigger(Constants.IDLE_TRANSITION_ANIMATION);
        }
    }
}
