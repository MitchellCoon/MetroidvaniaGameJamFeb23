using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] InputController input = null;
    [SerializeField] PlayerMovementController controller;

    [SerializeField] MovementOverride movement;
    
    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D body;
    private GroundCheck groundCheck;

    private float maxSpeedChange;
    private float acceleration;
    private bool isGrounded;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
        controller = GetComponent<PlayerMovementController>();
    }

    void Update()
    {
        direction.x = input.RetrieveMoveInput();
        desiredVelocity = new Vector2(direction.x, 0f) * Mathf.Max(movement.maxSpeed - groundCheck.GetFriction(), 0f);
    }

    private void FixedUpdate()
    {
        
        velocity = body.velocity;
        isGrounded = groundCheck.IsGrounded();
        acceleration = isGrounded ? movement.maxAcceleration : movement.maxAirAcceleration;
        maxSpeedChange = acceleration * Time.fixedDeltaTime;
        velocity.x = Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);

        body.velocity = velocity;

        // If the input is moving the player right and the player is facing left...
			if (velocity.x > 0 && !controller.IsFacingRight())
			{
				// ... flip the player.
				controller.Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (velocity.x < 0 && controller.IsFacingRight())
			{
				// ... flip the player.
				controller.Flip();
			}

    }

}
