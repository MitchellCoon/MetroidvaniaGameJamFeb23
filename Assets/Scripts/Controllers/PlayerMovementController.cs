using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovementController : MonoBehaviour
{
	[SerializeField] MovementOverride movement;
	[SerializeField] private bool isAirControlEnabled = false;					// Whether or not a player can steer while jumping;
	[SerializeField] private LayerMask groundLayer;						    	// A mask determining what is ground to the character
	[SerializeField] private GroundCheck groundCheck;				    		// A position marking where to check if the player is grounded
	[SerializeField] private Transform ceilingCheck;							// A position marking where to check for ceilings
	[SerializeField] private Collider2D crouchCollider;				            // A collider that will be disabled when crouching

	[SerializeField] BaseEnemyAI enemyAI;

	const float groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool isGrounded;          // Whether or not the player is grounded
	const float ceilingRadius = .2f;  // Radius of the overlap circle to determine if the player can stand up
	private new Rigidbody2D rigidbody2D;
	[SerializeField] bool isFacingRight = true;  // For determining which way the player is currently facing
	private Vector3 velocity = Vector3.zero;

	[Header("Events")]
	[Space]

	public UnityEvent OnLandEvent;

	[System.Serializable]
	public class BoolEvent : UnityEvent<bool> { }

	public BoolEvent OnCrouchEvent;
	private bool wasCrouching = false;

	public bool IsFacingRight()
	{
		return isFacingRight;
	}

	public void SetIsFacingRight(bool sourceIsFacingRight)
	{
		isFacingRight = sourceIsFacingRight;
	}

	private void Awake()
	{
		rigidbody2D = GetComponent<Rigidbody2D>();

		if (OnLandEvent == null)
			OnLandEvent = new UnityEvent();

		if (OnCrouchEvent == null)
			OnCrouchEvent = new BoolEvent();
	}

	private void FixedUpdate()
	{
		bool wasGrounded = isGrounded;
		isGrounded = groundCheck.IsGrounded();
	}

	void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals ("MovingPlatform") || other.gameObject.tag.Equals ("Floor"))
        {
            transform.parent = other.transform;
			rigidbody2D.interpolation = RigidbodyInterpolation2D.None;
        }
    }
 
    void OnCollisionExit2D(Collision2D other)
    {
        if (other.gameObject.tag.Equals ("MovingPlatform")|| other.gameObject.tag.Equals ("Floor")) 
        {
            transform.parent = null;
			rigidbody2D.interpolation = RigidbodyInterpolation2D.Interpolate;
        }
    }
	public void Move(float move, bool isCrouching, bool jump)
	{
		// If crouching, check to see if the character can stand up
		if (!isCrouching)
		{
			// If the character has a ceiling preventing them from standing up, keep them crouching
			if (Physics2D.OverlapCircle(ceilingCheck.position, ceilingRadius, groundLayer))
			{
				isCrouching = true;
			}
		}

		//only control the player if grounded or airControl is turned on
		if (isGrounded || isAirControlEnabled)
		{

			// If crouching
			if (isCrouching)
			{
				if (!wasCrouching)
				{
					wasCrouching = true;
					OnCrouchEvent.Invoke(true);
				}

				// Reduce the speed by the crouchSpeed multiplier
				move *= movement.crouchSpeed;

				// Disable one of the colliders when crouching
				if (crouchCollider != null)
					crouchCollider.enabled = false;
			} else
			{
				// Enable the collider when not crouching
				if (crouchCollider != null)
					crouchCollider.enabled = true;

				if (wasCrouching)
				{
					wasCrouching = false;
					OnCrouchEvent.Invoke(false);
				}
			}

			// Move the character by finding the target velocity
			Vector3 targetVelocity = new Vector2(move * 10f, rigidbody2D.velocity.y);
			// And then smoothing it out and applying it to the character
			rigidbody2D.velocity = Vector3.SmoothDamp(rigidbody2D.velocity, targetVelocity, ref velocity, movement.movementSmoothing);

			// If the input is moving the player right and the player is facing left...
			if (move > 0 && !isFacingRight)
			{
				// ... flip the player.
				Flip();
			}
			// Otherwise if the input is moving the player left and the player is facing right...
			else if (move < 0 && isFacingRight)
			{
				// ... flip the player.
				Flip();
			}
		}
		// If the player should jump...
		if (isGrounded && jump)
		{
			// Add a vertical force to the player.
			isGrounded = false;
			rigidbody2D.AddForce(new Vector2(0f, movement.jumpForce));
		}
	}

	public void Flip()
	{
		// Switch the way the player is labelled as facing.
		isFacingRight = !isFacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 localScale = transform.localScale;
		localScale.x *= -1;
		transform.localScale = localScale;
		if(enemyAI != null)
        {
            enemyAI.SetIsFacingRight(isFacingRight);
        }
	}

	public void ResetDirection()
	{
		if ((isFacingRight && transform.localScale.x < 0) || (!isFacingRight && transform.localScale.x > 0))
		{
			Flip();
			isFacingRight = !isFacingRight;
			if(enemyAI != null)
			{
				enemyAI.SetIsFacingRight(isFacingRight);
			}
		} 
	}

}
