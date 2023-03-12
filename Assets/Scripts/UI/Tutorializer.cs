using UnityEngine;

public class Tutorializer : MonoBehaviour
{
    [SerializeField] GameObject keyboardInstructions;
    [SerializeField] GameObject gamepadInstructions;

    private void Update()
    {
        if (MInput.controlScheme == ControlScheme.Keyboard)
        {
            keyboardInstructions.SetActive(true);
            gamepadInstructions.SetActive(false);
        }
        else
        {
            keyboardInstructions.SetActive(false);
            gamepadInstructions.SetActive(true);
        }
    }
}
