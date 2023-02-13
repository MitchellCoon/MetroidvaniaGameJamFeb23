using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;

/// <summary>
/// A Gate requires a Switch to open it.
/// When a switch is turned on, the corresponding BoolVariable emits an OnChanged event.
/// This Gate can either respond to this event if it exists in the scene,
/// or will simply be hidden when the Start method fires.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Gate : MonoBehaviour
{
    [SerializeField] BoolVariable didHitSwitch;
    [SerializeField] float openAnimationDuration = 0f;

    [Space]
    [Space]

    [Tooltip("optional")][SerializeField] SpriteRenderer hiddenGateSprite;

    Collider2D[] colliders;
    SpriteRenderer[] sprites;

    void OnEnable()
    {
        didHitSwitch.OnChanged += OnActivateSwitch;
    }

    void OnDisable()
    {
        didHitSwitch.OnChanged -= OnActivateSwitch;
    }

    void Awake()
    {
        Assert.IsNotNull(didHitSwitch);
        sprites = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    void Start()
    {
        if (hiddenGateSprite != null) hiddenGateSprite.enabled = false;
        if (didHitSwitch.value) HideGate();
    }

    void OnActivateSwitch(bool isActivated)
    {
        if (!isActivated) return;
        StartCoroutine(OpenGate());
    }

    IEnumerator OpenGate()
    {
        // we can add animations, FX, etc. here.
        yield return new WaitForSeconds(openAnimationDuration);
        HideGate();
    }

    void HideGate()
    {
        foreach (var sprite in sprites) if (sprite != null) sprite.enabled = false;
        if (hiddenGateSprite != null) hiddenGateSprite.enabled = true;
        foreach (var collider in colliders) if (collider != null) collider.enabled = false;
    }
}
