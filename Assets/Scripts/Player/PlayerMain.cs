using UnityEngine;
using UnityEngine.Assertions;
using Cinemachine;

using MapGen;

public class PlayerMain : MonoBehaviour
{
    CinemachineVirtualCamera vCam;
    Rigidbody2D body;

    PlayerCombat playerCombat;

    bool IsPlayer => playerCombat != null && !playerCombat.IsEnemy;

    public void SetCameraTargetAsPlayer()
    {
        if (!IsPlayer) return;
        if (vCam == null) vCam = FindObjectOfType<CinemachineVirtualCamera>();
        Assert.IsNotNull(vCam, "Unable to find CinemachineVirtualCamera in current scene");
        vCam.LookAt = transform;
        vCam.Follow = transform;
    }

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
        if (IsPlayer) GlobalMapState.playerPosition = transform.position;
    }

    void OnEnable()
    {
        if (IsPlayer) SetCameraTargetAsPlayer();
        if (IsPlayer) GlobalMapState.isPlayerActive = true;
    }

    void OnDisable()
    {
        if (IsPlayer) GlobalMapState.isPlayerActive = false;
    }
}
