using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] InputController input = null;
    [SerializeField] MovementOverride movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerMovementController controller;
    [SerializeField] GroundCheck groundCheck;
    [Space]
    [Space]
    [SerializeField] Sound dashSound;
    [SerializeField] float dashCooldown = 0.75f;
    
    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashSpeed;
    private float dashTimer = 0f;

    void Update()
    {
        if(!isDashing && groundCheck.IsGrounded())
        {
            dashTimer += dashCooldown;
        }
        if (dashTimer >= dashCooldown && input.RetrieveDashInput())
        {
            if (dashSound != null) dashSound.Play();
            isDashing = true;
            dashTimer = 0f;
            dashDirection = Vector3.right;
            if(!controller.IsFacingRight())
            {
                dashDirection = -dashDirection;
            }
            gameObject.layer = LayerMask.NameToLayer(Constants.INVINCIBLE_LAYER);
            dashSpeed = movement.dashSpeed;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
        dashTimer += Time.deltaTime;
    }

    void FixedUpdate()
    {
        if(isDashing)
        {
            ExecuteDash();
        }
    }

    void ExecuteDash()
    {
        dashSpeed -= dashSpeed * movement.dashDecay * Time.fixedDeltaTime;
        rb.velocity = dashSpeed * dashDirection;
        if(dashSpeed < movement.dashCutoff)
        {
            isDashing = false;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            gameObject.layer = LayerMask.NameToLayer(Constants.PLAYER_LAYER);
        }
    }
}
