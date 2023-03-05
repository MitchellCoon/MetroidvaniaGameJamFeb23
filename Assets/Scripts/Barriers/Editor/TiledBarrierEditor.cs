using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(TiledBarrier))]
public class TiledBarrierEditor : Editor
{
    public override void OnInspectorGUI()
    {

        GUIStyle warningStyle = new GUIStyle();
        warningStyle.normal.textColor = new Color(1, 0.66f, 0);
        warningStyle.padding = new RectOffset(6, 6, 0, 0);

        var script = (TiledBarrier)target;

        base.OnInspectorGUI();

        GUILayout.BeginVertical();
        GUILayout.Space(20);
        GUILayout.Label("NOTE", warningStyle);
        GUILayout.Label("Don't forget to add EnemyOnlyBarriers to", warningStyle);
        GUILayout.Label("prevent enemies passing this barrier when", warningStyle);
        GUILayout.Label("not being possessed.", warningStyle);
        GUILayout.Space(20);
        GUILayout.EndVertical();
    }
}
