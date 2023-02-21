using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;

public class Switch : MonoBehaviour, IInteractable
{
    [SerializeField] BoolVariable didHitSwitch;
    [Space]
    [Space]
    [SerializeField] SpriteRenderer onSprite;
    [SerializeField] SpriteRenderer offSprite;

    void Awake()
    {
        Assert.IsNotNull(didHitSwitch);
        Assert.IsNotNull(onSprite);
        Assert.IsNotNull(offSprite);
        Deactivate();
    }

    void Start()
    {
        if (didHitSwitch.value)
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
        didHitSwitch.value = true;
        Activate();
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
