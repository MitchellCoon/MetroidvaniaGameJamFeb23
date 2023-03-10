using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D), typeof(Rigidbody2D))]
public abstract class BasePickup : MonoBehaviour
{
    [SerializeField] bool initDefaultProperties = true;
    [SerializeField] SpriteRenderer mainSprite;

    new CircleCollider2D collider;
    Rigidbody2D body;

    // This method will get called in Awake, so no need to implement Awake in child classes.
    protected virtual void Init() { }

    protected void Awake()
    {
        collider = GetComponent<CircleCollider2D>();
        body = GetComponent<Rigidbody2D>();
        if (initDefaultProperties)
            InitDefaultProperties();
        Init();
    }

    void InitDefaultProperties()
    {
        if (!initDefaultProperties) return;
        // these defaults are mainly CYA to prevent dumb bugs
        collider.isTrigger = true;
        body.isKinematic = true;
        body.gravityScale = 0f;
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        HandlePickup(other);
    }

    protected abstract void HandlePickup(Collider2D other);

    protected void DisableSprite()
    {
        if (mainSprite != null) mainSprite.enabled = false;
    }

    protected void DisableCollider()
    {
        collider.enabled = false;
    }
}
