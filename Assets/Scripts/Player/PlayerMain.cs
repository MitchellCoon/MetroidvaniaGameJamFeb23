using UnityEngine;
using Cinemachine;
using UnityEngine.Assertions;

public class PlayerMain : MonoBehaviour
{
    CinemachineVirtualCamera vCam;
    Rigidbody2D body;

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
    }

    void OnEnable()
    {
        SetCameraTargetAsPlayer();
    }
}
