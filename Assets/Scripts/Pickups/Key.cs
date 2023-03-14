using UnityEngine;

using CyberneticStudios.SOFramework;

public class Key : BasePickup
{
    [Space]
    [Space]
    [SerializeField] BoolVariable hasKey;
    [SerializeField] Sound pickupSound;

    void Start()
    {
        if (hasKey.value) Hide();
    }

    protected override void HandlePickup(Collider2D other)
    {
        if (hasKey.value) return;
        if (pickupSound != null) pickupSound.Play();
        hasKey.value = true;
        Hide();
    }

    void Hide()
    {
        DisableSprite();
        DisableCollider();
    }
}
