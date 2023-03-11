using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(FieldEventHandler))]
public class ButtonNoise : MonoBehaviour
{
    [SerializeField] Sound focusSound;
    [SerializeField] Sound clickSound;

    FieldEventHandler eventHandler;

    void Awake()
    {
        eventHandler = GetComponent<FieldEventHandler>();
    }

    void OnEnable()
    {
        eventHandler.OnSubmitted += OnButtonClick;
        eventHandler.OnSelected += OnButtonFocus;
    }

    void OnDisable()
    {
        eventHandler.OnSubmitted -= OnButtonClick;
        eventHandler.OnSelected -= OnButtonFocus;
    }

    void OnButtonClick()
    {
        if (clickSound != null) clickSound.Play();
    }

    void OnButtonFocus()
    {
        if (focusSound != null) focusSound.Play();
    }
}
