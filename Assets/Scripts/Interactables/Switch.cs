using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using CyberneticStudios.SOFramework;
using System.Collections;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] BoolVariable didHitSwitch;
    [Space]
    [Space]
    [SerializeField] SpriteRenderer onSprite;
    [SerializeField] SpriteRenderer offSprite;
    [Space]
    [Space]
    [SerializeField] Sound switchSound;
    [Space]
    [Space]
    [SerializeField][Tooltip("Set to 0 to disable")] float resetTime = 0;
    [SerializeField] UnityEvent OnUse;
    [SerializeField] UnityEvent OnDeactivate;

    Coroutine deactivating;

    void OnValidate()
    {
        if (resetTime < 0) resetTime = 0;
    }

    void Awake()
    {
        Assert.IsNotNull(onSprite);
        Assert.IsNotNull(offSprite);
        Deactivate();
    }

    void Start()
    {
        if (didHitSwitch != null && didHitSwitch.value)
        {
            Activate();
        }
        else
        {
            Deactivate();
        }
    }

    // This gets called by Combat/Hitbox
    public void Use()
    {
        if (deactivating != null) return;
        if (didHitSwitch != null) didHitSwitch.value = true;
        Activate();
        OnUse.Invoke();
        if (switchSound != null) switchSound.Play();
        if (resetTime > 0) StartResetTimer();
    }

    void Activate()
    {
        onSprite.enabled = true;
        offSprite.enabled = false;
    }

    void Deactivate()
    {
        onSprite.enabled = false;
        offSprite.enabled = true;
        OnDeactivate.Invoke();
    }

    void StartResetTimer()
    {
        deactivating = StartCoroutine(CDeactivate());
    }

    IEnumerator CDeactivate()
    {
        yield return new WaitForSeconds(resetTime);
        Deactivate();
        deactivating = null;
    }
}
