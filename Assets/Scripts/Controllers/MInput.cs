using System;
using UnityEngine;
using UnityEngine.InputSystem;

// this is just a conenience class to make it easier to work with the new input system

public enum GamepadCode
{
    Start,
    Select,
    ButtonSouth,
    ButtonNorth,
    ButtonWest,
    ButtonEast,
    TriggerLeft,
    TriggerRight,
    BumperLeft,
    BumperRight,
    StickLeft,
    StickRight,
}

public enum ControlScheme
{
    Keyboard,
    Gamepad,
}

// M IS FOR METROIDVANIA :{
public static class MInput
{
    static ControlScheme _controlScheme;

    public static ControlScheme controlScheme => _controlScheme;

    public static bool UsingKeyboard(bool value)
    {
        if (value) _controlScheme = ControlScheme.Keyboard;
        return value;
    }

    public static Vector2 UsingKeyboard(Vector2 value)
    {
        if (value != Vector2.zero) _controlScheme = ControlScheme.Keyboard;
        return value;
    }

    public static bool UsingGamepad(bool value)
    {
        if (value) _controlScheme = ControlScheme.Gamepad;
        return value;
    }

    public static Vector2 UsingGamepad(Vector2 value)
    {
        if (value != Vector2.zero) _controlScheme = ControlScheme.Gamepad;
        return value;
    }

    public static float GetAxisRaw(string axis)
    {
        if (Keyboard.current == null) throw new UnityException("Cannot find Keyboard. Try restarting Unity.");
        switch (axis)
        {
            case "Horizontal":
                return Mathf.Clamp(GetGamepadMove().x + GetKeyboardMove().x, -1, 1);
            case "Vertical":
                return Mathf.Clamp(GetGamepadMove().y + GetKeyboardMove().y, -1, 1);
            default:
                throw new UnityException("axis must be \"Horizontal\" or \"Vertical\"");
        }
    }

    public static float GetAxis(string axis)
    {
        return GetAxisRaw(axis);
    }

    public static bool GetKeyDown(KeyCode code)
    {
        return UsingKeyboard(LookupKeyControl(code)?.wasPressedThisFrame ?? false);
    }

    public static bool GetKeyUp(KeyCode code)
    {
        return UsingKeyboard(LookupKeyControl(code)?.wasReleasedThisFrame ?? false);
    }

    public static bool GetKey(KeyCode code)
    {
        return UsingKeyboard(LookupKeyControl(code)?.isPressed ?? false);
    }

    public static bool GetPadDown(GamepadCode code)
    {
        if (Gamepad.current == null) return false;
        return UsingGamepad(LookupButtonControl(code).wasPressedThisFrame);
    }

    public static bool GetPadUp(GamepadCode code)
    {
        if (Gamepad.current == null) return false;
        return UsingGamepad(LookupButtonControl(code).wasReleasedThisFrame);
    }

    public static bool GetPad(GamepadCode code)
    {
        if (Gamepad.current == null) return false;
        return UsingGamepad(LookupButtonControl(code).isPressed);
    }

    static Vector2 GetKeyboardMove()
    {
        float keyboardLeft = Keyboard.current.aKey.isPressed ? -1 : 0;
        float keyboardRight = Keyboard.current.dKey.isPressed ? 1 : 0;
        float keyboardUp = Keyboard.current.wKey.isPressed ? 1 : 0;
        float keyboardDown = Keyboard.current.sKey.isPressed ? -1 : 0;
        return UsingKeyboard(new Vector2(
            keyboardLeft + keyboardRight,
            keyboardUp + keyboardDown
        ));
    }

    static Vector2 GetGamepadMove()
    {
        if (Gamepad.current == null) return Vector2.zero;
        return UsingGamepad(
            Gamepad.current.leftStick.ReadValue()
            + Gamepad.current.dpad.ReadValue()
        );
    }

    static Vector2 GetGamepadLook()
    {
        if (Gamepad.current == null) return Vector2.zero;
        return UsingGamepad(Gamepad.current.rightStick.ReadValue());
    }

    static UnityEngine.InputSystem.Controls.ButtonControl LookupButtonControl(GamepadCode code)
    {
        switch (code)
        {
            case GamepadCode.Start: return Gamepad.current.startButton;
            case GamepadCode.Select: return Gamepad.current.selectButton;
            case GamepadCode.ButtonSouth: return Gamepad.current.buttonSouth;
            case GamepadCode.ButtonNorth: return Gamepad.current.buttonNorth;
            case GamepadCode.ButtonWest: return Gamepad.current.buttonWest;
            case GamepadCode.ButtonEast: return Gamepad.current.buttonEast;
            case GamepadCode.TriggerLeft: return Gamepad.current.leftTrigger;
            case GamepadCode.TriggerRight: return Gamepad.current.rightTrigger;
            case GamepadCode.BumperLeft: return Gamepad.current.leftShoulder;
            case GamepadCode.BumperRight: return Gamepad.current.rightShoulder;
            case GamepadCode.StickLeft: return Gamepad.current.leftStickButton;
            case GamepadCode.StickRight: return Gamepad.current.rightStickButton;
            default:
                throw new UnityException($"Unsupported GamepadCode: {System.Enum.GetName(typeof(GamepadCode), code)}");
        }
    }

    static UnityEngine.InputSystem.Controls.ButtonControl LookupKeyControl(KeyCode code)
    {
        if (Keyboard.current == null) return null;
        switch (code)
        {
            case KeyCode.Mouse0: return Mouse.current != null ? Mouse.current.leftButton : null;
            case KeyCode.Mouse1: return Mouse.current != null ? Mouse.current.rightButton : null;
            case KeyCode.Escape: return Keyboard.current.escapeKey;
            case KeyCode.Tab: return Keyboard.current.tabKey;
            case KeyCode.Space: return Keyboard.current.spaceKey;
            case KeyCode.Return: return Keyboard.current.enterKey;
            case KeyCode.KeypadEnter: return Keyboard.current.numpadEnterKey;
            case KeyCode.LeftShift: return Keyboard.current.leftShiftKey;
            case KeyCode.RightShift: return Keyboard.current.rightShiftKey;
            case KeyCode.LeftAlt: return Keyboard.current.leftAltKey;
            case KeyCode.RightAlt: return Keyboard.current.rightAltKey;
            case KeyCode.Alpha0: return Keyboard.current.digit0Key;
            case KeyCode.Alpha1: return Keyboard.current.digit1Key;
            case KeyCode.Alpha2: return Keyboard.current.digit2Key;
            case KeyCode.Alpha3: return Keyboard.current.digit3Key;
            case KeyCode.Alpha4: return Keyboard.current.digit4Key;
            case KeyCode.Alpha5: return Keyboard.current.digit5Key;
            case KeyCode.Alpha6: return Keyboard.current.digit6Key;
            case KeyCode.Alpha7: return Keyboard.current.digit7Key;
            case KeyCode.Alpha8: return Keyboard.current.digit8Key;
            case KeyCode.Alpha9: return Keyboard.current.digit9Key;
            case KeyCode.Keypad0: return Keyboard.current.numpad0Key;
            case KeyCode.Keypad1: return Keyboard.current.numpad1Key;
            case KeyCode.Keypad2: return Keyboard.current.numpad2Key;
            case KeyCode.Keypad3: return Keyboard.current.numpad3Key;
            case KeyCode.Keypad4: return Keyboard.current.numpad4Key;
            case KeyCode.Keypad5: return Keyboard.current.numpad5Key;
            case KeyCode.Keypad6: return Keyboard.current.numpad6Key;
            case KeyCode.Keypad7: return Keyboard.current.numpad7Key;
            case KeyCode.Keypad8: return Keyboard.current.numpad8Key;
            case KeyCode.Keypad9: return Keyboard.current.numpad9Key;
            case KeyCode.A: return Keyboard.current.aKey;
            case KeyCode.B: return Keyboard.current.bKey;
            case KeyCode.C: return Keyboard.current.cKey;
            case KeyCode.D: return Keyboard.current.dKey;
            case KeyCode.E: return Keyboard.current.eKey;
            case KeyCode.F: return Keyboard.current.fKey;
            case KeyCode.G: return Keyboard.current.gKey;
            case KeyCode.H: return Keyboard.current.hKey;
            case KeyCode.I: return Keyboard.current.iKey;
            case KeyCode.J: return Keyboard.current.jKey;
            case KeyCode.K: return Keyboard.current.kKey;
            case KeyCode.L: return Keyboard.current.lKey;
            case KeyCode.M: return Keyboard.current.mKey;
            case KeyCode.N: return Keyboard.current.nKey;
            case KeyCode.O: return Keyboard.current.oKey;
            case KeyCode.P: return Keyboard.current.pKey;
            case KeyCode.Q: return Keyboard.current.qKey;
            case KeyCode.R: return Keyboard.current.rKey;
            case KeyCode.S: return Keyboard.current.sKey;
            case KeyCode.T: return Keyboard.current.tKey;
            case KeyCode.U: return Keyboard.current.uKey;
            case KeyCode.V: return Keyboard.current.vKey;
            case KeyCode.W: return Keyboard.current.wKey;
            case KeyCode.X: return Keyboard.current.xKey;
            case KeyCode.Y: return Keyboard.current.yKey;
            case KeyCode.Z: return Keyboard.current.zKey;
            default:
                throw new UnityException($"Unsupported KeyCode {System.Enum.GetName(typeof(KeyCode), code)}");
        }
    }
}
