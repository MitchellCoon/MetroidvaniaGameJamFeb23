using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(ObjectEnablerDisabler))]
public class ObjectEnablerDisablerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var script = (ObjectEnablerDisabler)target;

        GUILayout.BeginVertical();

        GUILayout.Space(40);

        GUILayout.Label("INSTRUCTIONS");
        GUILayout.Label("- Place children inside this GameObject");
        GUILayout.Label("- Note - only top-level children are controlled");
        GUILayout.Label("- Add the BoolVariable you want to use as on/off state");
        GUILayout.Label("That's it!");

        GUILayout.EndVertical();
    }
}
