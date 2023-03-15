using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

[ExecuteInEditMode]
public class GuidComponent : MonoBehaviour
{
    [Tooltip("The GUID be auto-generated when this component is attached to a game object in a scene.")]
    [SerializeField] string uniqueIdentifier = "";

    static Dictionary<string, GuidComponent> globalLookup = new Dictionary<string, GuidComponent>();

    public string GetUniqueIdentifier()
    {
        return uniqueIdentifier;
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Application.IsPlaying(gameObject)) return;
        if (string.IsNullOrEmpty(gameObject.scene.path)) return;

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty property = serializedObject.FindProperty("uniqueIdentifier");

        if (string.IsNullOrEmpty(property.stringValue) || !IsUnique(property.stringValue))
        {
            property.stringValue = System.Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }

        globalLookup[property.stringValue] = this;
    }
#endif

    private bool IsUnique(string candidate)
    {
        if (!globalLookup.ContainsKey(candidate)) return true;
        if (globalLookup[candidate] == this) return true;
        if (globalLookup[candidate] == null)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        if (globalLookup[candidate].GetUniqueIdentifier() != candidate)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        return false;
    }
}
