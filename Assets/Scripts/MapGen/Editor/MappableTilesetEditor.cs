using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(MappableTileset))]
public class MappableTilesetEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (MappableTileset)target;

        GUILayout.BeginVertical();


        if (script.IsMapBorder)
        {
            GUILayout.Label("NOTE - Map Border tilemaps hidden during runtime.");
        }

        GUILayout.Space(10);

        GUILayout.EndVertical();
    }
}
