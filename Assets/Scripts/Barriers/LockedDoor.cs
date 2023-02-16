using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;

using CyberneticStudios.SOFramework;

/// <summary>
/// A LockedDoor requires a Key to open it. The key can exist in any scene.
/// There are two states that are tracked:
/// (1) does the player have the needed key? and
/// (2) has the door been opened?
/// Both of these states are handled via BoolVariables.
/// </summary>
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class LockedDoor : MonoBehaviour
{
    [SerializeField] BoolVariable hasKey;
    [SerializeField] BoolVariable hasOpenedDoor;
    [SerializeField] float openAnimationDuration = 0f;

    [Space]
    [Space]

    [Tooltip("optional")][SerializeField] SpriteRenderer hiddenGateSprite;

    SpriteRenderer[] sprites;
    Collider2D[] colliders;

    void Awake()
    {
        Assert.IsNotNull(hasKey);
        Assert.IsNotNull(hasOpenedDoor);
        sprites = GetComponentsInChildren<SpriteRenderer>();
        colliders = GetComponentsInChildren<Collider2D>();
    }

    void Start()
    {
        if (hiddenGateSprite != null) hiddenGateSprite.enabled = true;
        if (hasOpenedDoor.value) HideDoor();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (!hasKey.value) return;
        hasOpenedDoor.value = true;
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {
        // we can add animations, FX, etc. here.
        yield return new WaitForSeconds(openAnimationDuration);
        HideDoor();
    }

    void HideDoor()
    {
        foreach (var sprite in sprites) if (sprite != null) sprite.enabled = false;
        if (hiddenGateSprite != null) hiddenGateSprite.enabled = true;
        foreach (var collider in colliders) if (collider != null) collider.enabled = false;
    }
}
