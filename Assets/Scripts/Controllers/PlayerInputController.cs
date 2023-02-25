using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputController", menuName = "InputController/PlayerController")]

public class PlayerInputController : InputController
{
    public override float RetrieveMoveInput()
    {
        return Input.GetAxisRaw("Horizontal");
    }

    public override bool RetrieveJumpInput()
    {
        return Input.GetButtonDown("Jump");
    }

    public override bool RetrieveJumpButtonReleased()
    {
        return Input.GetButtonUp("Jump");
    }

    public override bool RetrieveDashInput()
    {
        return Input.GetButtonDown("Dash");
    }
}
