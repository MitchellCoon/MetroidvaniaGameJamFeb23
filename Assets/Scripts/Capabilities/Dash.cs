using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dash : MonoBehaviour
{
    [SerializeField] InputController input = null;
    [SerializeField] MovementOverride movement;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] PlayerMovementController controller;
    
    private bool isDashing = false;
    private Vector3 dashDirection;
    private float dashSpeed;



    void Update()
    {
        if (input.RetrieveDashInput())
        {
            isDashing = true;
            dashDirection = new Vector3(1,0,0);
            if(!controller.IsFacingRight())
            {
                dashDirection = -dashDirection;
            }
            gameObject.layer = LayerMask.NameToLayer("Invincible");
            dashSpeed = movement.dashSpeed;
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        }
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
            gameObject.layer = LayerMask.NameToLayer("Player");
        }
    }
}
