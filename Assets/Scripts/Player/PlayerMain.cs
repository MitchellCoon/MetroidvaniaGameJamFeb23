using UnityEngine;

using MapGen;

public class PlayerMain : MonoBehaviour
{
    Rigidbody2D body;

    PlayerCombat playerCombat;

    public void SetKinematic()
    {
        body.isKinematic = true;
    }

    public void SetDynamic()
    {
        body.isKinematic = false;
    }

    void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerCombat = GetComponent<PlayerCombat>();
    }

    void Update()
    {
        GlobalMapState.playerPosition = transform.position;
    }

    void OnEnable()
    {
        GlobalMapState.isPlayerActive = true;
        GlobalEvent.OnRoomLoaded += OnRoomLoaded;
    }

    void OnDisable()
    {
        GlobalMapState.isPlayerActive = false;
        GlobalEvent.OnRoomLoaded -= OnRoomLoaded;
    }

    void OnRoomLoaded(Vector2 obj)
    {
        GlobalEvent.Invoke.OnPlayerEnteredRoom(this);
    }
}
