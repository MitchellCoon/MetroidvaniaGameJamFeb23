using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheckTransform;
    [SerializeField] bool isGrounded; // Whether or not the player is grounded
    const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	const float ceilingRadius = .2f;  // Radius of the overlap circle to determine if the player can stand up
    private float friction;

    private void Update()
	{
		bool wasGrounded = isGrounded;
		isGrounded = false;
		Collider2D[] colliders = Physics2D.OverlapCircleAll(groundCheckTransform.position, groundedRadius, groundLayer);
		for (int i = 0; i < colliders.Length; i++)
		{
			if (colliders[i].gameObject != gameObject)
			{
				isGrounded = true;
			}
		}
	}
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EvaluateCollision(collision);
        RetrieveFriction(collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
        friction = 0;
    }

    private void EvaluateCollision(Collision2D collision)
    {
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector2 normal = collision.GetContact(i).normal;
            isGrounded |= normal.y >= 0.9f;
        }
    }

    private void RetrieveFriction(Collision2D collision)
    {
        if (collision.rigidbody == null) return;
        PhysicsMaterial2D material = collision.rigidbody.sharedMaterial;

        friction = 0;

        if (material != null)
        {
            friction = material.friction;
        }
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public float GetFriction()
    {
        return friction;
    }
}
