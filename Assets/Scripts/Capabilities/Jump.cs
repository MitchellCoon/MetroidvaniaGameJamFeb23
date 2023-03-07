using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jump : MonoBehaviour
{
    [SerializeField] InputManager inputManager;
    [SerializeField] private InputController inputController = null;
    [SerializeField] MovementOverride movement;

    private Rigidbody2D body;
    private GroundCheck groundCheck;
    private Vector2 velocity;
    private int jumpPhase;
    private bool desiredJump;
    private bool isGrounded;
    private bool jumpButtonReleased;
    private float jumpBufferCounter;
    private bool jumpBufferTimeStarted = false;
    private float coyoteTimeCounter;

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        groundCheck = GetComponent<GroundCheck>();
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
        isGrounded = groundCheck.IsGrounded();
        if (isGrounded)
        {
            jumpPhase = 0;
            coyoteTimeCounter = inputManager.GetInputBufferTime(InputManager.Input.CoyoteJump);
            inputManager.SetPreviousActionTime(InputManager.Action.Grounded, Time.time);
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        bool isJumpButtonHeld = inputController.RetrieveJumpButtonHeld();

        if (inputManager.GetInputRequested(InputManager.Input.Jump))
        {
            JumpAction();
        }

        if (velocity.y > 0 && isJumpButtonHeld)
        {
            body.gravityScale = movement.upwardMovementMultiplier;
        }
        else if (velocity.y > 0 & !isJumpButtonHeld)
        {
            body.gravityScale = movement.upwardMovementShortJumpMultiplier;
        }
        else if (velocity.y < 0)
        {
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
            isGrounded = false;
            float jumpSpeed = Mathf.Sqrt(-2f * Physics2D.gravity.y * movement.jumpHeight);
            if (velocity.y > 0f)
            {
                jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
            }
            if (!isGrounded && velocity.y < 0)
            {
                // first zero out y velocity
                velocity.y = 0;
            }
            velocity.y += jumpSpeed;
        }
    }
}
