using UnityEngine;

[CreateAssetMenu(fileName = "PlayerInputController", menuName = "InputController/PlayerController")]

public class PlayerInputController : InputController
{
    public override float RetrieveMoveInput()
    {
        return MInput.GetAxisRaw("Horizontal");
    }

    public override bool RetrieveJumpInput()
    {
        return MInput.GetKeyDown(KeyCode.Space) || MInput.GetPadDown(GamepadCode.ButtonSouth);
    }

    public override bool RetrieveJumpButtonReleased()
    {
        return MInput.GetKeyUp(KeyCode.Space) || MInput.GetPadUp(GamepadCode.ButtonSouth);
    }

    public override bool RetrieveDashInput()
    {
        return MInput.GetKeyDown(KeyCode.LeftShift) || MInput.GetPadDown(GamepadCode.BumperLeft);;
    }
}
