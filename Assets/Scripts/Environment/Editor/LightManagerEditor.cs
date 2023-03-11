using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(LightManager))]
public class LightManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (LightManager)target;

        base.OnInspectorGUI();

        GUILayout.BeginVertical();
        GUILayout.Space(20);
        GUILayout.Label($"GlobalLight={GlobalLight.Value}");
        GUILayout.Space(20);
        GUILayout.EndVertical();
    }
}
