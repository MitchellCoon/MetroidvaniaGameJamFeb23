using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(HiddenArea))]
public class HiddenAreaEditor : Editor
{
    public override void OnInspectorGUI()
    {

        GUIStyle warningStyle = new GUIStyle();
        warningStyle.normal.textColor = new Color(1, 0.66f, 0);
        warningStyle.padding = new RectOffset(6, 6, 0, 0);

        var script = (HiddenArea)target;

        base.OnInspectorGUI();

        GUILayout.BeginVertical();
        GUILayout.Space(20);
        GUILayout.Label("INSTRUCTIONS");
        GUILayout.Label("Place 1 or more SpriteRenderers / Tilemaps on");
        GUILayout.Label("this object or one of its children, along");
        GUILayout.Label("with a RigidBody2D and a collider set to trigger.");
        GUILayout.Space(20);
        GUILayout.Label("NOTE", warningStyle);
        GUILayout.Label("If using a tilemap, set the composite collider", warningStyle);
        GUILayout.Label("geometry type to \"Polygon\".", warningStyle);
        GUILayout.Space(20);
        GUILayout.EndVertical();
    }
}
