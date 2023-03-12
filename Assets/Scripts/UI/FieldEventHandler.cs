using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FieldEventHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, ISubmitHandler
{
    [SerializeField] bool debug;
    public event Action OnSelected;
    public event Action OnDeselected;
    public event Action OnSubmitted;

    Selectable selectable;
    bool _isSelected = false;

    public bool isSelected => _isSelected;
    public bool interactable => selectable != null && selectable.interactable && selectable.enabled && isActiveAndEnabled;

    public void OnSelect(BaseEventData eventData)
    {
        if (debug) Debug.Log($"{gameObject.name} selected");
        _isSelected = true;
        OnSelected?.Invoke();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (debug) Debug.Log($"{gameObject.name} de-selected");
        _isSelected = false;
        OnDeselected?.Invoke();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (!interactable) return;
        if (debug) Debug.Log($"{gameObject.name} submitted");
        OnSubmitted?.Invoke();
    }

    void Awake()
    {
        selectable = GetComponent<Selectable>();
    }
}
