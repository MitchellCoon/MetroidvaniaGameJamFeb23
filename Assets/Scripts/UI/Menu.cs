using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Menu : MonoBehaviour
{
    Button[] buttons;

    void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    void OnEnable()
    {
        FocusOnFirstButton();
    }

    void Start()
    {
        FocusOnFirstButton();
    }

    void FocusOnFirstButton()
    {
        if (buttons.Length > 0) buttons[0].Select();
    }
}
