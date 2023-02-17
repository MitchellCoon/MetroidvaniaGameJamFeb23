using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;
using System.Collections.Generic;

/// <summary>
/// A Gate requires a Switch to open it.
/// When a switch is turned on, the corresponding BoolVariable emits an OnChanged event.
/// This Gate can either respond to this event if it exists in the scene,
/// or will simply be hidden when the Start method fires.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Gate : MonoBehaviour
{
    [SerializeField] BoolCondition[] openConditions = new BoolCondition[0];
    [SerializeField] float openAnimationDuration = 0f;

    [Space]
    [Space]

    [Tooltip("optional")][SerializeField] SpriteRenderer hiddenGateSprite;

    Collider2D[] colliders;
    SpriteRenderer[] sprites;

    Dictionary<Collider2D, bool> collidersInitiallyEnabledMap = new Dictionary<Collider2D, bool>();

    Coroutine closing;
    Coroutine opening;

    void OnEnable()
    {
        for (int i = 0; i < openConditions.Length; i++)
        {
            if (openConditions[i] == null) continue;
            openConditions[i].OnChanged += OnBoolVariableChange;
        }
    }

    void OnDisable()
    {
        for (int i = 0; i < openConditions.Length; i++)
        {
            if (openConditions[i] == null) continue;
            openConditions[i].OnChanged -= OnBoolVariableChange;
        }
    }

    void Awake()
    {
        Assert.IsTrue(openConditions.Length > 0);
        sprites = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();
        foreach (var collider in colliders) collidersInitiallyEnabledMap[collider] = collider.enabled;
    }

    void Start()
    {
        if (hiddenGateSprite != null) hiddenGateSprite.enabled = false;
        if (AllConditionsMet()) HideGate();
    }

    void OnBoolVariableChange(bool isActivated)
    {
        HandleConditionsChange();
    }

    void HandleConditionsChange()
    {
        if (AllConditionsMet())
        {
            if (closing != null) StopCoroutine(closing);
            if (opening == null) opening = StartCoroutine(OpenGate());
        }
        else
        {
            if (opening != null) StopCoroutine(opening);
            if (closing == null) closing = StartCoroutine(CloseGate());
        }
    }

    bool AllConditionsMet()
    {
        for (int i = 0; i < openConditions.Length; i++)
        {
            if (!openConditions[i].value) return false;
        }
        return true;
    }

    IEnumerator OpenGate()
    {
        // we can add animations, FX, etc. here.
        yield return new WaitForSeconds(openAnimationDuration);
        HideGate();
        opening = null;
    }

    IEnumerator CloseGate()
    {
        ShowGate();
        // we can add animations, FX, etc. here.
        yield return new WaitForSeconds(openAnimationDuration);
        closing = null;
    }

    void HideGate()
    {
        foreach (var sprite in sprites) if (sprite != null) sprite.enabled = false;
        if (hiddenGateSprite != null) hiddenGateSprite.enabled = true;
        foreach (var collider in colliders) if (collider != null) collider.enabled = false;
    }

    void ShowGate()
    {
        foreach (var sprite in sprites) if (sprite != null) sprite.enabled = true;
        if (hiddenGateSprite != null) hiddenGateSprite.enabled = false;
        foreach (var collider in colliders) if (collider != null && collidersInitiallyEnabledMap[collider]) collider.enabled = true;
    }
}
