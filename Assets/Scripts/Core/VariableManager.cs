using System.Collections.Generic;
using UnityEngine;

using CyberneticStudios.SOFramework;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Variables are awesome as global state containers for very specific things, but they need to be initialized
/// when the game starts. This class handles that.
/// To refresh the variables list:
/// - Open the MainGameSystems prefab -> Go to VariableManager
/// - Click the Ellipses on the VariableManager component
/// - Select "Refresh Variables List"
/// </summary>
public class VariableManager : MonoBehaviour
{
    [Tooltip("DO NOT POPULATE BY-HAND! Use VariableManager -> Context Menu -> Refresh Variables List")]
    [SerializeField] List<BaseVariable> variables = new List<BaseVariable>();

    void Awake()
    {
        for (int i = 0; i < variables.Count; i++)
        {
            variables[i].ResetVariable();
        }
    }

#if UNITY_EDITOR

    // This method grabs all of the Variable ScriptableObjects that exist anywhere in the Assets directory,
    // and populates `variables` above with the found results.
    [ContextMenu("Refresh Variables List")]
    public void RefreshVariablesList()
    {
        Debug.Log("*** FINDING VARIABLES ***");
        string[] guids;
        variables.Clear();
        // see: https://docs.unity.cn/560/Documentation/ScriptReference/AssetDatabase.FindAssets.html
        guids = AssetDatabase.FindAssets("t:BaseVariable");
        foreach (string guid in guids)
        {
            Debug.Log("Adding BaseVariable: " + AssetDatabase.GUIDToAssetPath(guid));
            PopulateVariableObjFromGuid(guid);
        }
        MarkDirty();
    }

    void PopulateVariableObjFromGuid(string guid)
    {
        // see: https://docs.unity3d.com/ScriptReference/AssetDatabase.GUIDToAssetPath.html
        string path = AssetDatabase.GUIDToAssetPath(guid);
        // see: https://docs.unity3d.com/ScriptReference/AssetDatabase.LoadAssetAtPath.html
        var variable = (BaseVariable)AssetDatabase.LoadAssetAtPath(path, typeof(BaseVariable));
        variables.Add(variable);
    }

    // This fixes a bug where changing prefab values from a Script would not
    // cause those changes to be saved.
    void MarkDirty()
    {
        // see: https://forum.unity.com/threads/can-prefabs-be-set-to-dirty-in-an-editor-script.598321/#post-3999400
        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }
#endif
}
