using UnityEngine;
using UnityEngine.Assertions;

public enum Track
{
    Dangerman,
    Megacreep,
    Monsterdance,
    ExitMusic,
    RollCredits,
}

public class MusicTrack : BaseAudio
{
    [SerializeField] Track _track;

    MusicPlayer musicPlayer;

    public Track track => _track;

    void Awake()
    {
        musicPlayer = GetComponentInParent<MusicPlayer>();
        Assert.IsNotNull(musicPlayer, "MusicTrack needs to be a descendant of MusicPlayer!");
        musicPlayer.RegisterTrack(this);
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

    public static string GetName(Track value)
    {
        return System.Enum.GetName(typeof(Track), value);
    }
}
