using UnityEngine;
using UnityEngine.Assertions;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ButtonText : MonoBehaviour, ISerializationCallbackReceiver
{
    [SerializeField] Color activeColor = Color.black;
    [SerializeField] Color inactiveColor = Color.white;

    FieldEventHandler eventHandler;
    TextMeshProUGUI textNode;

    void Awake()
    {
        eventHandler = GetComponentInParent<FieldEventHandler>();
        textNode = GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(eventHandler, "ButtonText must be a child of a Button component");
    }

    void Start()
    {
        if (eventHandler == null) return;
        SetColorState();
    }

    void OnEnable()
    {
        if (eventHandler == null) return;
        eventHandler.OnSelected += OnSelected;
        eventHandler.OnDeselected += OnDeselected;
    }

    void OnDisable()
    {
        if (eventHandler == null) return;
        eventHandler.OnSelected -= OnSelected;
        eventHandler.OnDeselected -= OnDeselected;
    }

    void OnSelected()
    {
        SetColorState();
    }
    void OnDeselected()
    {
        SetColorState();
    }

    void SetColorState()
    {
        if (eventHandler == null) return;
        if (eventHandler.isSelected)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    void Activate()
    {
        textNode.color = activeColor;
    }

    void Deactivate()
    {
        textNode.color = inactiveColor;
    }

    public void OnBeforeSerialize() { }

    public void OnAfterDeserialize()
    {
        SetColorState();
    }
}
