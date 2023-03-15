using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpState { Grounded, Rising, Falling };

public class Jump : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] private InputController inputController = null;
    [SerializeField] MovementOverride movement;
    [SerializeField] Sound jumpSound;
    [SerializeField] Sound jumpSoundPossessed;
    [SerializeField] Animator animator;
    [SerializeField] RuntimeAnimatorController defaultAnimator;

    PossessionManager possessionManager;

    private Rigidbody2D body;
    private GroundCheck groundCheck;
    private Vector2 velocity;
    private int jumpPhase;
    private bool desiredJump;
    private bool jumpButtonReleased;
    private float jumpBufferCounter;
    private bool jumpBufferTimeStarted = false;
    private float coyoteTimeCounter;
    private JumpState jumpState;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
        possessionManager = GetComponent<PossessionManager>();
    }

    void Update()
    {
        desiredJump |= inputController.RetrieveJumpInput();
        if (desiredJump)
        {
            if (!inputManager.GetInputRequested(InputManager.Input.Jump))
            {
                inputManager.AddInputRequestToQueue(InputManager.Input.Jump, Time.time);
            }
            if (!jumpBufferTimeStarted)
            {
                jumpBufferCounter = inputManager.GetInputBufferTime(InputManager.Input.Jump);
                jumpBufferTimeStarted = true;
            }
            else
            {
                jumpBufferCounter -= Time.deltaTime;
                if (jumpBufferCounter <= 0f)
                {
                    jumpBufferCounter = 0f;
                    desiredJump = false;
                    jumpBufferTimeStarted = false;
                }
            }
        }
        if (inputController.RetrieveJumpInput())
        {
            inputManager.SetPreviousPressedTime(InputManager.Input.Jump, Time.time);
        }
        jumpButtonReleased |= inputController.RetrieveJumpButtonReleased();
    }

    private void FixedUpdate()
    {
        velocity = body.velocity;
        if (groundCheck.IsGrounded())
        {
            animator.SetBool(Constants.IS_GROUNDED_BOOL, true);
            jumpPhase = 0;
            if (jumpState != JumpState.Grounded)
            {
                if (animator != null) animator.SetBool(Constants.JUMP_FALL_ANIMATION, false);
                if (animator != null) animator.SetBool(Constants.JUMP_LAND_ANIMATION, true);
            }
            jumpState = JumpState.Grounded;
            coyoteTimeCounter = inputManager.GetInputBufferTime(InputManager.Input.CoyoteJump);
            inputManager.SetPreviousActionTime(InputManager.Action.Grounded, Time.time);
        }
        else
        {
            animator.SetBool(Constants.IS_GROUNDED_BOOL, false);
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        bool isJumpButtonHeld = inputController.RetrieveJumpButtonHeld();

        if (inputManager.GetInputRequested(InputManager.Input.Jump))
        {
            JumpAction();
        }

        if (velocity.y > 0 && isJumpButtonHeld && !groundCheck.IsGrounded())
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
        else if (velocity.y > 0 && !isJumpButtonHeld && !groundCheck.IsGrounded())
        {
            if (jumpState != JumpState.Rising)
            {
                if (animator != null) animator.SetTrigger(Constants.JUMP_RISE_ANIMATION);
                if (animator != null) animator.SetBool(Constants.JUMP_FALL_ANIMATION, false);
                if (animator != null) animator.SetBool(Constants.JUMP_LAND_ANIMATION, false);
            }
            jumpState = JumpState.Rising;
            body.gravityScale = movement.upwardMovementShortJumpMultiplier;
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

    private void JumpAction()
    {
        if ((!(inputManager.GetPreviousActionTime(InputManager.Action.Grounded) == -1) && Time.time - inputManager.GetPreviousActionTime(InputManager.Action.Grounded) <= inputManager.GetInputBufferTime(InputManager.Input.Jump)) || jumpPhase < movement.maxAirJumps)
        {
            if ((coyoteTimeCounter < 0f || jumpBufferCounter < 0f))
            {
                jumpPhase += 1;
            }
            jumpBufferCounter = 0f;
            coyoteTimeCounter = 0f;
            desiredJump = false;
            jumpBufferTimeStarted = false;
            inputManager.SetPreviousActionTime(InputManager.Action.Grounded, -1);
            inputManager.SetPreviousPressedTime(InputManager.Input.Jump, -1);
            inputManager.RemoveInputRequestFromQueue(InputManager.Input.Jump);
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * movement.jumpHeight);
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            if (!groundCheck.IsGrounded() && velocity.y < 0)
            {
                // first zero out y velocity
                velocity.y = 0;
            }
            velocity.y += jumpSpeed;
            PlayJumpSound();
        }
    }

    void PlayJumpSound()
    {
        if (possessionManager != null)
        {
            if (jumpSoundPossessed != null) jumpSoundPossessed.Play();
        }
        else
        {
            if (jumpSound != null) jumpSound.Play();
        }
    }

    public void ResetAnimator()
    {
        if (animator != null) animator.runtimeAnimatorController = defaultAnimator;
    }
}
