using UnityEngine;
using Cinemachine;

[RequireComponent(typeof(CinemachineVirtualCamera))]
public class VCamFollowManager : MonoBehaviour
{
    CinemachineVirtualCamera vCam;

    void Awake()
    {
        vCam = GetComponent<CinemachineVirtualCamera>();
    }

    void OnEnable()
    {
        GlobalEvent.OnPlayerSpawn += OnPlayerSpawn;
        GlobalEvent.OnEnemyPossessed += OnEnemyPossessed;
        GlobalEvent.OnPlayerEnteredRoom += OnPlayerEnteredRoom;
    }

    void OnDisable()
    {
        GlobalEvent.OnPlayerSpawn -= OnPlayerSpawn;
        GlobalEvent.OnEnemyPossessed -= OnEnemyPossessed;
        GlobalEvent.OnPlayerEnteredRoom -= OnPlayerEnteredRoom;
    }

    void OnPlayerSpawn(PlayerMain player)
    {
        LookAt(player.transform);
    }

    void OnEnemyPossessed(PlayerMain player)
    {
        LookAt(player.transform);
    }

    void OnPlayerEnteredRoom(PlayerMain player)
    {
        LookAt(player.transform);
    }

    void LookAt(Transform transform)
    {
        vCam.LookAt = transform;
        vCam.Follow = transform;
    }
}
