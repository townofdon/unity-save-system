using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class SaveableEntity : MonoBehaviour
{
    [ReadOnly][SerializeField] string _uuid = "";

    [HideInInspector][SerializeField] int _instanceId;

    [SerializeField] ISaveable[] saveables;

    public string uuid => _uuid;

    public void OnSave(ref GameData gameData)
    {
        foreach (ISaveable saveable in GetComponents<ISaveable>())
        {
            saveable.OnSave(ref gameData);
        }
    }

    public void OnLoad(GameData gameData)
    {
        foreach (ISaveable saveable in GetComponents<ISaveable>())
        {
            saveable.OnLoad(gameData);
        }
    }

#if UNITY_EDITOR
    static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

    // void Update()
    // {
    //     if (Application.IsPlaying(gameObject)) return;
    //     if (string.IsNullOrEmpty(gameObject.scene.path)) return;

    //     SerializedObject serializedObject = new SerializedObject(this);
    //     SerializedProperty p_uuid = serializedObject.FindProperty("_uuid");

    //     if (string.IsNullOrEmpty(p_uuid.stringValue) || !IsUnique(p_uuid.stringValue))
    //     {
    //         p_uuid.stringValue = Guid.NewGuid().ToString();
    //         serializedObject.ApplyModifiedProperties();
    //     }

    //     globalLookup[p_uuid.stringValue] = this;
    // }

    [ContextMenu("Regenerate UUID")]
    void Regen()
    {
        SerializedObject serializedObject = new SerializedObject(this);
        SerializedProperty p_uuid = serializedObject.FindProperty("_uuid");

        p_uuid.stringValue = Guid.NewGuid().ToString();
        serializedObject.ApplyModifiedProperties();

        globalLookup[p_uuid.stringValue] = this;
    }

    // bool IsUnique(string candidate)
    // {
    //     if (!globalLookup.ContainsKey(candidate)) return true;

    //     if (globalLookup[candidate] == this) return true;

    //     if (globalLookup[candidate] == null)
    //     {
    //         globalLookup.Remove(candidate);
    //         return true;
    //     }

    //     if (globalLookup[candidate].uuid.ToString() != candidate)
    //     {
    //         globalLookup.Remove(candidate);
    //         return true;
    //     }

    //     return false;
    // }
#endif

}
