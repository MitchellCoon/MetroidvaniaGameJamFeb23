using UnityEngine;

using CyberneticStudios.SOFramework;

public class Key : BasePickup
{
    [Space]
    [Space]
    [SerializeField] BoolVariable hasKey;

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
        DisableSprite();
        DisableCollider();
    }
}
