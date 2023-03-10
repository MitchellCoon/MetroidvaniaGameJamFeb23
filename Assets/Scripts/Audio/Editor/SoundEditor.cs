using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(Sound))]
public class SoundEditor : Editor
{
    public override void OnInspectorGUI()
    {
        GUIStyle stoppedStyle = new GUIStyle();
        stoppedStyle.normal.textColor = Color.red;
        stoppedStyle.padding = new RectOffset(6, 6, 0, 0);

        GUIStyle playingStyle = new GUIStyle();
        playingStyle.normal.textColor = Color.green;
        playingStyle.padding = new RectOffset(6, 6, 0, 0);

        GUIStyle pausedStyle = new GUIStyle();
        pausedStyle.normal.textColor = new Color(1, 0.66f, 0);
        pausedStyle.padding = new RectOffset(6, 6, 0, 0);

        base.OnInspectorGUI();

        var script = (Sound)target;

        GUILayout.BeginVertical();

        GUILayout.Space(40);

        if (script.isPlaying)
        {
            GUILayout.Label("Playing", playingStyle);
        }
        else
        {
            GUILayout.Label("Stopped", stoppedStyle);
        }

        if (script.isPaused)
        {
            GUILayout.Label("Paused", pausedStyle);
        }

        if (GUILayout.Button("Play", GUILayout.Height(40)))
        {
            PerformAction(script, () => { script.Play(); });
        }

        if (GUILayout.Button("Stop", GUILayout.Height(40)))
        {
            PerformAction(script, () => { script.Stop(); });
        }

        GUILayout.EndVertical();
    }

    void PerformAction(Sound script, System.Action action)
    {
        if (Application.isPlaying)
        {
            script.Init();
            action?.Invoke();
        }
        else
        {
            Debug.LogWarning("Must be in play mode");
        }
    }
}
