using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SaveIdentifier : MonoBehaviour
{
    [ReadOnly][SerializeField] string _uuid = "";

    public string uuid => _uuid;

    // public object CaptureState()
    // {
    //     Dictionary<string, object> state = new Dictionary<string, object>();
    //     foreach (ISaveable saveable in GetComponents<ISaveable>())
    //     {
    //         state[saveable.GetType().ToString()] = saveable.CaptureState();
    //     }
    //     return state;
    // }

    // public void RestoreState(object state)
    // {
    //     Dictionary<string, object> stateDict = (Dictionary<string, object>)state;
    //     foreach (ISaveable saveable in GetComponents<ISaveable>())
    //     {
    //         string typeString = saveable.GetType().ToString();
    //         if (stateDict.ContainsKey(typeString))
    //         {
    //             saveable.RestoreState(stateDict[typeString]);
    //         }
    //     }
    // }

#if UNITY_EDITOR
    static Dictionary<string, SaveIdentifier> globalLookup = new Dictionary<string, SaveIdentifier>();

    void Update()
    {
        if (Application.IsPlaying(gameObject)) return;
        if (string.IsNullOrEmpty(gameObject.scene.path)) return;

        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty p_uuid = serializedObject.FindProperty("_uuid");

        if (string.IsNullOrEmpty(p_uuid.stringValue) || !IsUnique(p_uuid.stringValue))
        {
            p_uuid.stringValue = Guid.NewGuid().ToString();
            serializedObject.ApplyModifiedProperties();
        }

        globalLookup[p_uuid.stringValue] = this;
    }

    [ContextMenu("Regenerate UUID")]
    void Regen()
    {
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty p_uuid = serializedObject.FindProperty("_uuid");

        p_uuid.stringValue = Guid.NewGuid().ToString();
        serializedObject.ApplyModifiedProperties();

        globalLookup[p_uuid.stringValue] = this;
    }

    bool IsUnique(string candidate)
    {
        if (!globalLookup.ContainsKey(candidate)) return true;

        if (globalLookup[candidate] == this) return true;

        if (globalLookup[candidate] == null)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        if (globalLookup[candidate].uuid.ToString() != candidate)
        {
            globalLookup.Remove(candidate);
            return true;
        }

        return false;
    }
#endif

}
