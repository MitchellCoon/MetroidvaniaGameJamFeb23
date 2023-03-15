using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;

public class Menu : MonoBehaviour
{
    Button[] buttons;

    protected void Init()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    protected void FocusOnFirstButton()
    {
        Assert.IsNotNull(buttons, "You forgot to call Base.Init() for a Menu subclass");
        if (buttons.Length > 0) buttons[0].Select();
    }
}
