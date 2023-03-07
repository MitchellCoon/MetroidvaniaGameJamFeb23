using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    List<MusicTrack> musicTracks = new List<MusicTrack>();

    public void RegisterTrack(MusicTrack incoming)
    {
        if (musicTracks.Contains(incoming)) return;
        musicTracks.Add(incoming);
    }

    void PlayTrack(Track track)
    {
        MusicTrack musicTrack = FindMusicTrack(track);
        if (musicTrack == null) throw new UnityException($"Music track \"{MusicTrack.GetName(track)}\" not found");
    }

    MusicTrack FindMusicTrack(Track track)
    {
        for (int i = 0; i < musicTracks.Count; i++)
        {
            if (musicTracks[i].track == track) return musicTracks[i];
        }
        return null;
    }
}
