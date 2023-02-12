using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions;

public class PlayerMain : MonoBehaviour
{
    CinemachineVirtualCamera vCam;

    void OnEnable()
    {
        SetCameraTargetAsPlayer();
    }

    void SetCameraTargetAsPlayer()
    {
        if (vCam == null) vCam = FindObjectOfType<CinemachineVirtualCamera>();
        Assert.IsNotNull(vCam, "Unable to find CinemachineVirtualCamera in current scene");
        vCam.LookAt = transform;
        vCam.Follow = transform;
    }
}
