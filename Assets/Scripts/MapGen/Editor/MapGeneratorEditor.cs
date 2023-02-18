using UnityEngine;
using UnityEditor;

using MapGen;

[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (MapGenerator)target;

        GUILayout.BeginVertical();

        GUILayout.Space(40);

        GUILayout.Label("Press the button below to regenerate the minimap");

        if (GUILayout.Button("Generate Minimap", GUILayout.Height(40)))
        {
            script.Generate();
        }

        if (GUILayout.Button("Remove Test Sprites", GUILayout.Height(40)))
        {
            script.DiscardTestSprites();
        }

        GUILayout.EndVertical();
    }
}
