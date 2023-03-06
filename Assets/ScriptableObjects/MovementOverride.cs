using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Movement Override", menuName = "MovementOverride")]

public class MovementOverride : ScriptableObject
{
    public float crouchSpeed = .36f;            // Amount of maxSpeed applied to crouching movement. 1 = 100%
    [Range(0, .3f)] public float movementSmoothing = .05f;	// How much to smooth out the movement
    [Range(0f, 100f)] public float maxSpeed = 10f;
    [Range(0f, 100f)] public float maxAcceleration = 100f;
    [Range(0f, 100f)] public float maxAirAcceleration = 100f;
    public float defaultGravityScale = 10f;

    public float jumpForce = 800f;
    public float jumpHeight = 30f;
    [Range(0, 5)] public int maxAirJumps = 0;
    public float downwardMovementMultiplier = 10f;
    public float upwardMovementMultiplier = 7f;
    public float upwardMovementShortJumpMultiplier = 15f;
    public float terminalVelocity = 3f; // multiple of gravity constant, 9.8 m/s

    public float dashSpeed = 40f;       // initial dash speed
    public float dashDecay = 5f;        // how fast the dash decelerates
    public float dashCutoff = 5f; // the speed at which the dash will and and normal movement resumes
}
