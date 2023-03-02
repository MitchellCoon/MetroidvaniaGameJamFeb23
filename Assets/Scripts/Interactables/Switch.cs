using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

using CyberneticStudios.SOFramework;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] BoolVariable didHitSwitch;
    [Space]
    [Space]
    [SerializeField] SpriteRenderer onSprite;
    [SerializeField] SpriteRenderer offSprite;
    [Space]
    [Space]
    [SerializeField] UnityEvent OnUse;

    void Awake()
    {
        Assert.IsNotNull(didHitSwitch);
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
        if (didHitSwitch != null) didHitSwitch.value = true;
        Activate();
        OnUse.Invoke();
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
    }
}
