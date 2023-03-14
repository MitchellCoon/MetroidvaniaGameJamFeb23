using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

using CyberneticStudios.SOFramework;
using System;

[RequireComponent(typeof(Collider2D))]
public class TiledBarrier : MonoBehaviour
{
    [SerializeField] BoolCondition canPlayerPassThrough;

    new Collider2D collider;

    void OnEnable()
    {
        canPlayerPassThrough.OnChanged += OnCanPlayerPassThroughChanged;
    }

    void OnDisable()
    {
        canPlayerPassThrough.OnChanged -= OnCanPlayerPassThroughChanged;
    }

    void Awake()
    {
        collider = GetComponent<Collider2D>();
        Assert.IsNotNull(canPlayerPassThrough);
        Assert.IsTrue(canPlayerPassThrough.hasRef);
    }

    void Start()
    {
        if (canPlayerPassThrough.value) DisableCollider();
    }

    void OnCanPlayerPassThroughChanged(bool incoming)
    {
        if (canPlayerPassThrough.value)
        {
            DisableCollider();
        }
        else
        {
            EnableCollider();
        }
    }

    void EnableCollider()
    {
        collider.enabled = true;
    }

    void DisableCollider()
    {
        collider.enabled = false;
    }

}
