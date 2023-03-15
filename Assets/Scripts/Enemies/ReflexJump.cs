using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflexJump : MonoBehaviour
{
    [SerializeField] Rigidbody2D body;
    [SerializeField] MovementOverride movement;
    [SerializeField] Animator animator;
    [SerializeField] GroundCheck groundCheck;
    [SerializeField] Sound jumpSound;
    [SerializeField] float reflexJumpCooldown = 1f;

    Vector2 velocity;
    JumpState jumpState;
    bool jumpTrigger = false;
    float reflexJumpTimer = 0f;

    void Update()
    {
        reflexJumpTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        velocity = body.velocity;
        if (groundCheck.IsGrounded())
        {
            animator.SetBool(Constants.IS_GROUNDED_BOOL, true);
            if (jumpState != JumpState.Grounded)
            {
                if (animator != null) animator.SetBool(Constants.JUMP_FALL_ANIMATION, false);
                if (animator != null) animator.SetBool(Constants.JUMP_LAND_ANIMATION, true);
            }
            jumpState = JumpState.Grounded;
        }
        else
        {
            animator.SetBool(Constants.IS_GROUNDED_BOOL, false);
        }

        if (jumpTrigger)
        {
            JumpAction();
            jumpTrigger = false;
            reflexJumpTimer = 0f;
        }

        if (velocity.y > 0 && !groundCheck.IsGrounded())
        {
            if (jumpState != JumpState.Rising)
            {
                if (animator != null) animator.SetTrigger(Constants.JUMP_RISE_ANIMATION);
                if (animator != null) animator.SetBool(Constants.JUMP_FALL_ANIMATION, false);
                if (animator != null) animator.SetBool(Constants.JUMP_LAND_ANIMATION, false);
            }
            jumpState = JumpState.Rising;
            body.gravityScale = movement.upwardMovementMultiplier;

        }
        else if (velocity.y < 0 && !groundCheck.IsGrounded())
        {
            if (jumpState != JumpState.Falling)
            {
                if (animator != null) animator.SetBool(Constants.JUMP_FALL_ANIMATION, true);
                if (animator != null) animator.SetBool(Constants.JUMP_LAND_ANIMATION, false);
            }
            jumpState = JumpState.Falling;
            body.gravityScale = movement.downwardMovementMultiplier;
        }
        else
        {
            body.gravityScale = movement.defaultGravityScale;
        }

        // clamp fall speed to terminal velocity
        if (velocity.y < 0)
        {
            float clampedYSpeed = Mathf.Clamp(velocity.y, -Constants.GRAVITY * movement.terminalVelocity, 0);
            velocity = new Vector2(velocity.x, clampedYSpeed);
        }

        body.velocity = velocity;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Hitbox hitbox = other.GetComponent<Hitbox>();
        if (hitbox == null || reflexJumpTimer < reflexJumpCooldown) return;
        PossessionManager targetPossessionManager = hitbox.GetSourcePossessionManager();
        if(jumpState == JumpState.Grounded && targetPossessionManager == null || (targetPossessionManager != null && targetPossessionManager.IsPossessed()))
        {
            jumpTrigger = true;
        }
    }

    void JumpAction()
    {
        float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * movement.jumpHeight);
        if (velocity.y > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
        }
        if (!groundCheck.IsGrounded() && velocity.y < 0)
        {
            velocity.y = 0;
        }
        velocity.y += jumpSpeed;
        if (jumpSound != null) jumpSound.Play();
    }

}
