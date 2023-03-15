using UnityEngine;
using UnityEditor;

using CyberneticStudios.SOFramework;

[CustomEditor(typeof(ObjectEnablerDisabler))]
public class ObjectEnablerDisablerEditor : Editor
{
    public override void OnInspectorGUI()
    {


        GUIStyle warningStyle = new GUIStyle();
        warningStyle.normal.textColor = new Color(1, 0.66f, 0);

        var script = (ObjectEnablerDisabler)target;

        if (!script.HasChildren || !script.HasCondition)
        {
            GUILayout.Space(20);
        }

        if (!script.HasChildren)
        {
            ErrorLabel("This GameObject requires one or more children");
        }

        if (!script.HasCondition)
        {
            ErrorLabel("Condition is required - add it below");
        }

        base.OnInspectorGUI();


        GUILayout.BeginVertical();

        GUILayout.Space(40);

        GUILayout.Label("INSTRUCTIONS");
        GUILayout.Label("- Place children inside this GameObject");
        GUILayout.Label("- Add the BoolVariable you want to use as on/off state");
        GUILayout.Label("- Check `Invert` to evaluate BoolVar as its opposite value");
        GUILayout.Space(20);
        GUILayout.Label("*Note* - only top-level children are controlled", warningStyle);

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
