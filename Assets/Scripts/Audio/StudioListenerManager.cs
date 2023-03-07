using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(StudioListener))]
public class StudioListenerManager : MonoBehaviour
{
    StudioListener listener;

    void Awake()
    {
        listener = GetComponent<StudioListener>();
    }

    void OnEnable()
    {
        GlobalEvent.OnPlayerSpawn += OnPlayerSpawn;
        GlobalEvent.OnEnemyPossessed += OnEnemyPossessed;
    }

    void OnDisable()
    {
        GlobalEvent.OnPlayerSpawn -= OnPlayerSpawn;
        GlobalEvent.OnEnemyPossessed -= OnEnemyPossessed;
    }

    void OnPlayerSpawn(PlayerMain player)
    {
        listener.SetAttenuationObject(player.gameObject);
    }

    void OnEnemyPossessed(PlayerMain player)
    {
        listener.SetAttenuationObject(player.gameObject);
    }
}
