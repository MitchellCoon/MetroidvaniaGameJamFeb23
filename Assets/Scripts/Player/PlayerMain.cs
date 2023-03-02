using UnityEngine;
using UnityEngine.Assertions;
using Cinemachine;

using MapGen;

public class PlayerMain : MonoBehaviour
{
    CinemachineVirtualCamera vCam;
    Rigidbody2D body;

    PlayerCombat playerCombat;

    public void SetCameraTargetAsPlayer()
    {
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
        GlobalMapState.playerPosition = transform.position;
    }

    void OnEnable()
    {
        SetCameraTargetAsPlayer();
        GlobalMapState.isPlayerActive = true;
    }

    void OnDisable()
    {
        GlobalMapState.isPlayerActive = false;
    }
}
