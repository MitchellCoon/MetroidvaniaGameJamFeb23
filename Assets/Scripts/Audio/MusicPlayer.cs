using System.Collections.Generic;
using UnityEngine;

using CyberneticStudios.SOFramework;
using UnityEngine.Assertions;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField][Range(0, 5)] float fadeInDuration = 3f;
    [SerializeField][Range(0, 5)] float fadeoutDuration = 3f;
    [Space]
    [Space]
    [SerializeField] BoolVariable hasDisabledSecurity;
    [SerializeField] BoolVariable hasStartedBossFight;
    [SerializeField] BoolVariable hasDefeatedBoss;

    const Track TRACK_MAIN_MENU = Track.Dangerman;
    const Track TRACK_GAME_START = Track.Dangerman;
    const Track TRACK_SECURITY_DISABLED = Track.Megacreep;
    const Track TRACK_BOSS_FIGHT = Track.Monsterdance;
    const Track TRACK_ESCAPE = Track.Dangerman;
    const Track TRACK_WIN_SCREEN = Track.Dangerman;

    List<MusicTrack> musicTracks = new List<MusicTrack>();

    MusicTrack currentTrack;

    bool hasFirstRoomLoaded = false;

    public void RegisterTrack(MusicTrack incoming)
    {
        if (musicTracks.Contains(incoming)) return;
        musicTracks.Add(incoming);
    }

    public void Init()
    {
        hasFirstRoomLoaded = false;
        hasDisabledSecurity.value = false;
        hasStartedBossFight.value = false;
        hasDefeatedBoss.value = false;
    }

    void Awake()
    {
        Assert.IsNotNull(hasDisabledSecurity, "hasDisabledSecurity {BoolVariable} required in MusicPlayer");
        Assert.IsNotNull(hasStartedBossFight, "hasStartedBossFight {BoolVariable} required in MusicPlayer");
        Assert.IsNotNull(hasDefeatedBoss, "hasDefeatedBoss {BoolVariable} required in MusicPlayer");
    }

    void OnEnable()
    {
        GlobalEvent.OnRoomLoaded += OnRoomLoaded;
        hasDisabledSecurity.OnChanged += OnHasDisabledSecurityChanged;
        hasStartedBossFight.OnChanged += OnHasEnteredBossArenaChanged;
        hasDefeatedBoss.OnChanged += OnHasDefeatedBossChanged;
        GlobalEvent.OnWinGame += OnWinGame;
    }

    void OnDisable()
    {
        GlobalEvent.OnRoomLoaded -= OnRoomLoaded;
        hasDisabledSecurity.OnChanged -= OnHasDisabledSecurityChanged;
        hasStartedBossFight.OnChanged -= OnHasEnteredBossArenaChanged;
        hasDefeatedBoss.OnChanged -= OnHasDefeatedBossChanged;
        GlobalEvent.OnWinGame -= OnWinGame;
    }

    void OnRoomLoaded(Vector2 obj)
    {
        if (hasFirstRoomLoaded) return;
        StopCurrentTrack(exclude: TRACK_GAME_START);
        PlayTrack(TRACK_GAME_START);
        hasFirstRoomLoaded = true;
    }

    void OnHasDisabledSecurityChanged(bool value)
    {
        if (!value) return;
        FadeOutCurrentTrack(exclude: TRACK_SECURITY_DISABLED);
        FadeInTrack(TRACK_SECURITY_DISABLED);
    }

    void OnHasEnteredBossArenaChanged(bool value)
    {
        if (!value) return;
        StopCurrentTrack(exclude: TRACK_BOSS_FIGHT);
        PlayTrack(TRACK_BOSS_FIGHT);
    }

    void OnHasDefeatedBossChanged(bool value)
    {
        if (!value) return;
        StopCurrentTrack(exclude: TRACK_ESCAPE);
        PlayTrack(TRACK_ESCAPE);
    }

    void OnWinGame()
    {
        StopCurrentTrack(exclude: TRACK_WIN_SCREEN);
        PlayTrack(TRACK_WIN_SCREEN);
    }

    void PlayTrack(Track track)
    {
        MusicTrack musicTrack = FindMusicTrack(track);
        musicTrack.Play();
        currentTrack = musicTrack;
    }

    void FadeInTrack(Track track)
    {
        MusicTrack musicTrack = FindMusicTrack(track);
        musicTrack.FadeIn(fadeInDuration);
        currentTrack = musicTrack;
    }

    void StopCurrentTrack(Track exclude)
    {
        if (currentTrack == null) return;
        if (currentTrack.track == exclude) return;
        if (!currentTrack.isPlaying) return;
        currentTrack.Stop();
    }

    void FadeOutCurrentTrack(Track exclude)
    {
        if (currentTrack == null) return;
        if (currentTrack.track == exclude) return;
        if (!currentTrack.isPlaying) return;
        currentTrack.FadeOut(fadeoutDuration);
    }

    MusicTrack FindMusicTrack(Track track)
    {
        for (int i = 0; i < musicTracks.Count; i++)
        {
            if (musicTracks[i].track == track) return musicTracks[i];
        }
        throw new UnityException($"Music track \"{MusicTrack.GetName(track)}\" not found");
    }
}
