using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [SerializeField] InputController input = null;
    [SerializeField] PlayerMovementController controller;
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController defaultAnimator;
    [SerializeField] MovementOverride movement;
    [Space]
    [Space]
    [SerializeField] Sound moveSound;
    
    private Vector2 direction;
    private Vector2 desiredVelocity;
    private Vector2 velocity;
    private Rigidbody2D body;
    private GroundCheck groundCheck;

    private float maxSpeedChange;
    private float acceleration;
    private bool isGrounded;
    private bool isMoving;

    private float possessionTimer = 0.5f;
	private float possessionCooldown = 0.5f;

    public void AnimMoveEvent() {
        if (moveSound != null) moveSound.Play();
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
        controller = GetComponent<PlayerMovementController>();
    }

    void Update()
    {
        possessionTimer += Time.deltaTime;
        if (possessionTimer <= possessionCooldown)
        {
            desiredVelocity = Vector2.zero;
            return;   
        }
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

        isMoving = Mathf.Abs(velocity.x) > Mathf.Epsilon ? true : false;
        if (animator != null) animator.SetBool("isMoving", isMoving);

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

    public void ResetAnimator()
    {
        if (animator != null) animator.runtimeAnimatorController = defaultAnimator;
    }

    public void ResetPossessionTimer()
	{
		possessionTimer = 0f;
	}

}
