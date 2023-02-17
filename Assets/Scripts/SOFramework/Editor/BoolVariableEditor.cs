using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(BoolVariable))]
public class BoolVariableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (BoolVariable)target;

        GUILayout.BeginVertical();

        GUILayout.Space(40);

        GUILayout.Label("Press the button below to send OnChanged events to all listeners");

        if (GUILayout.Button("Invoke Callbacks", GUILayout.Height(40)))
        {
            script.InvokeCallbacks();
        }

        GUILayout.EndVertical();
    }
}
