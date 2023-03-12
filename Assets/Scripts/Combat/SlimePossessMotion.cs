using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlimePossessMotion : MonoBehaviour
{

    [SerializeField] float speed = 10f;
    [SerializeField] float minimumDistance = 0.001f;
    SpriteRenderer spriteRenderer;
    Vector3 target;


    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < minimumDistance)
        {
            spriteRenderer.enabled = true;
            Destroy(gameObject);
        }
    }

    public void SetTarget(SpriteRenderer slimePossessionSpriteRenderer, Vector3 enemyAttachPoint, bool isFacingRight)
    {
        target = enemyAttachPoint;
        spriteRenderer = slimePossessionSpriteRenderer;
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
