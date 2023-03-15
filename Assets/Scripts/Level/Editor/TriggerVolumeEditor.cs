using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(TriggerVolume))]
public class TriggerVolumeEditor : Editor
{
    public override void OnInspectorGUI()
    {

        GUIStyle warningStyle = new GUIStyle();
        warningStyle.normal.textColor = new Color(1, 0.66f, 0);

        var script = (TriggerVolume)target;

        if (!script.HasBoolRef)
        {
            GUILayout.Space(20);
            ErrorLabel("BoolVariable is required - add it below");
        }

        base.OnInspectorGUI();

        GUILayout.BeginVertical();

        GUILayout.Space(40);

        if (GUILayout.Button("Resize Debug Sprite to Collider Bounds", GUILayout.Height(40)))
        {
            script.ResizeSpriteToColliderBounds();
        }

        GUILayout.EndVertical();
    }

    void ErrorLabel(string text)
    {
        GUIStyle errorStyle = new GUIStyle();
        errorStyle.normal.textColor = Color.red;
        GUILayout.Label("!! " + text, errorStyle);
        GUILayout.Space(20);
    }
}
