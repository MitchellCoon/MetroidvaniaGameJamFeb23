using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

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
    enum DoorState
    {
        Init,
        Locked,
        Unlocked,
        Open,
    }

    [SerializeField] BoolVariable hasKey;
    [SerializeField] BoolVariable hasOpenedDoor;
    [SerializeField] bool keepDoorOpen = false;
    [SerializeField] float openAnimationDuration = 0f;
    [SerializeField] float closeDoorWaitTime = 1f;
    [Space]
    [Space]
    [SerializeField] SpriteRenderer lockedDoorSprite;
    [SerializeField] SpriteRenderer unlockedDoorSprite;
    [FormerlySerializedAs("hiddenGateSprite")]
    [SerializeField] SpriteRenderer openDoorSprite;
    [SerializeField] SpriteRenderer lockedSignSprite;
    [SerializeField] SpriteRenderer unlockedSignSprite;
    [Space]
    [Space]
    [SerializeField] Sound openDoorSound;
    [SerializeField] Sound closeDoorSound;
    [SerializeField] Sound lockedDoorErrorSound;

    Collider2D[] colliders;

    DoorState doorState;
    Coroutine openingDoor;
    Coroutine closingDoor;

    void Awake()
    {
        colliders = GetComponentsInChildren<Collider2D>();
    }

    void OnEnable()
    {
        if (hasKey != null) hasKey.OnChanged += OnHasKeyChanged;
    }

    void OnDisable()
    {
        if (hasKey != null) hasKey.OnChanged -= OnHasKeyChanged;
    }

    void Start()
    {
        LockDoor();
        if (hasKey != null && hasKey.value) UnlockDoor();
        if (hasOpenedDoor != null && keepDoorOpen && hasOpenedDoor.value) OpenDoor();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasKey == null) return;
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        if (!hasKey.value)
        {
            IndicateLocked();
            return;
        }
        hasOpenedDoor.value = true;
        StopCoroutineAndReset(ref openingDoor);
        StopCoroutineAndReset(ref closingDoor);
        openingDoor = StartCoroutine(COpenDoor());
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (hasKey == null) return;
        if (!other.CompareTag(Constants.PLAYER_TAG)) return;
        StopCoroutineAndReset(ref openingDoor);
        StopCoroutineAndReset(ref closingDoor);
        closingDoor = StartCoroutine(CCloseDoor());
    }

    void OnHasKeyChanged(bool incoming)
    {
        StopCoroutineAndReset(ref closingDoor);
        if (incoming)
        {
            UnlockDoor();
        }
        else
        {
            LockDoor();
        }
    }

    IEnumerator COpenDoor()
    {
        //
        // we can add animations, FX, etc. here.
        //
        yield return new WaitForSeconds(openAnimationDuration);
        OpenDoor();
        openingDoor = null;
    }

    IEnumerator CCloseDoor()
    {
        yield return new WaitForSeconds(closeDoorWaitTime);
        // manually trigger hasKey change event to close the door
        OnHasKeyChanged(hasKey.value);
        closingDoor = null;
    }

    void StopCoroutineAndReset(ref Coroutine action)
    {
        if (action == null) return;
        StopCoroutine(action);
        action = null;
    }

    void LockDoor()
    {
        if (doorState == DoorState.Locked) return;
        doorState = DoorState.Locked;
        if (lockedSignSprite != null) lockedSignSprite.enabled = true;
        if (lockedDoorSprite != null) lockedDoorSprite.enabled = true;
        if (unlockedSignSprite != null) unlockedSignSprite.enabled = false;
        if (unlockedDoorSprite != null) unlockedDoorSprite.enabled = false;
        if (openDoorSprite != null) openDoorSprite.enabled = false;
        foreach (var collider in colliders) if (collider != null) collider.enabled = true;
    }

    void IndicateLocked()
    {
        if (lockedDoorErrorSound != null) lockedDoorErrorSound.Play();
    }

    void UnlockDoor()
    {
        if (doorState == DoorState.Unlocked) return;
        doorState = DoorState.Unlocked;
        if (closeDoorSound != null) closeDoorSound.Play();
        if (lockedSignSprite != null) lockedSignSprite.enabled = false;
        if (lockedDoorSprite != null) lockedDoorSprite.enabled = false;
        if (unlockedSignSprite != null) unlockedSignSprite.enabled = true;
        if (unlockedDoorSprite != null) unlockedDoorSprite.enabled = true;
        if (openDoorSprite != null) openDoorSprite.enabled = false;
        foreach (var collider in colliders) if (collider != null && collider.isTrigger) collider.enabled = true;
    }

    void OpenDoor()
    {
        if (doorState == DoorState.Open) return;
        doorState = DoorState.Open;
        if (openDoorSound != null) openDoorSound.Play();
        if (lockedSignSprite != null) lockedSignSprite.enabled = false;
        if (lockedDoorSprite != null) lockedDoorSprite.enabled = false;
        if (unlockedSignSprite != null) unlockedSignSprite.enabled = true;
        if (unlockedDoorSprite != null) unlockedDoorSprite.enabled = false;
        if (openDoorSprite != null) openDoorSprite.enabled = true;
        foreach (var collider in colliders) if (collider != null) collider.enabled = false;
    }
}
