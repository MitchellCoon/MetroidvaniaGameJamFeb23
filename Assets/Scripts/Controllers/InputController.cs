using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputController : ScriptableObject
{
    public abstract float RetrieveMoveInput();

    public abstract bool RetrieveJumpInput();

    public abstract bool RetrieveJumpButtonHeld();

    public abstract bool RetrieveJumpButtonReleased();

    public abstract bool RetrieveDashInput();

}