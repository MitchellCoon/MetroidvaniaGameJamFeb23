using UnityEngine;

using CyberneticStudios.SOFramework;

public class Key : BasePickup
{
    [SerializeField] BoolVariable hasKey;

    bool isActive = false;

    void Start()
    {
        if (hasKey.value) Hide();
    }

    protected override void HandlePickup(Collider2D other)
    {
        if (hasKey.value) return;
        hasKey.value = true;
        Hide();
    }

    void Hide()
    {
        DisabledSprite();
        DisableCollider();
    }
}
