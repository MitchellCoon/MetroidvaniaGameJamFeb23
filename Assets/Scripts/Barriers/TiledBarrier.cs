using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Tilemaps;

using CyberneticStudios.SOFramework;
using System;

[RequireComponent(typeof(TilemapCollider2D))]
public class TiledBarrier : MonoBehaviour
{
    [SerializeField] BoolCondition canPlayerPassThrough;

    new TilemapCollider2D collider;

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
        collider = GetComponent<TilemapCollider2D>();
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