using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(VariableManager))]
public class VariableManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (VariableManager)target;

        GUILayout.BeginVertical();

        GUILayout.Space(40);

        GUILayout.Label("Press the button below to repopulate the list\nof variables in your Assets directory.");

        if (GUILayout.Button("Refresh Variables List", GUILayout.Height(40)))
        {
            script.RefreshVariablesList();
        }

        GUILayout.EndVertical();
    }
}
