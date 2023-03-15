using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePossessMotion : MonoBehaviour
{

    [SerializeField] float speed = 10f;
    [SerializeField] float minimumDistance = 0.001f;
    [SerializeField] RuntimeAnimatorController slimeAnimator;
    SpriteRenderer spriteRenderer;
    Animator animator;
    Vector3 target;


    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < minimumDistance)
        {
            spriteRenderer.enabled = true;
            animator.runtimeAnimatorController = slimeAnimator;
            Destroy(gameObject);
        }
    }

    public void SetTarget(SpriteRenderer slimePossessionSpriteRenderer, Animator slimePossessionAnimator, Vector3 enemyAttachPoint, bool isFacingRight)
    {
        target = enemyAttachPoint;
        spriteRenderer = slimePossessionSpriteRenderer;
        animator = slimePossessionAnimator;
        if (!isFacingRight)
        {
            Flip();
        }

    }

    private void Flip()
	{
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
	}
}
