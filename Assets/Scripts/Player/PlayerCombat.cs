using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] ResourceManager resources;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] float weightMultiplier = 1f;

    [SerializeField] Resource health;

    public void TakeDamage(AttackData attackData, Vector3 attackOrigin)
	{
        health.SubtractResource(attackData.damage);

        animator.SetTrigger("Hurt");

        Vector2 adjustedForce = attackData.knockbackForce * weightMultiplier * (attackOrigin - transform.position).normalized; 

        rb.AddForce(adjustedForce, ForceMode2D.Impulse);

        if(health.GetCurrentValue() <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        GetComponent<Move>().enabled = false;
        GetComponent<Jump>().enabled = false;
        GetComponent<Attack>().enabled = false;
        GetComponent<CircleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().gravityScale = 0;
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        GetComponent<PlayerMovementController>().enabled = false;
        this.enabled = false;
    }

}
