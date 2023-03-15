using UnityEngine;

public class Sound : BaseAudio
{
    void Awake()
    {
        base.Init();
    }

    void OnEnable()
    {
        base.RegisterListeners();
    }

    void OnDisable()
    {
        base.UnregisterListeners();
    }
}
