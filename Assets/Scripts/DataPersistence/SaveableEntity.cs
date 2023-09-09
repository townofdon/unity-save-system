using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// TODO: REMOVE

[ExecuteInEditMode]
public class SaveableEntity : MonoBehaviour
{
    //     [ReadOnly][SerializeField] string _uuid = "";
    //     [HideInInspector][SerializeField] string _scenePath;

    //     [HideInInspector][SerializeField] MonoSaveable[] saveables;

    //     public string uuid => _uuid;

    //     public void OnSave(ref GameData gameData)
    //     {
    //         // if (saveables == null) return;
    //         // for (int i = 0; i < saveables.Length; i++)
    //         // {
    //         //     saveables[i].OnSave(ref gameData);
    //         // }
    //     }

    //     public void OnLoad(GameData gameData)
    //     {
    //         // if (saveables == null) return;
    //         // for (int i = 0; i < saveables.Length; i++)
    //         // {
    //         //     saveables[i].OnLoad(gameData);
    //         // }
    //     }

    //     [ContextMenu("Regenerate UUID")]
    //     public void RegenerateUUID()
    //     {
    // #if UNITY_EDITOR
    //         if (Application.isPlaying) return;
    //         Regen();
    // #endif
    //     }

    // #if UNITY_EDITOR

    //     int _instanceId;
    //     string _prevUuid;

    //     void OnValidate()
    //     {
    //         if (Application.IsPlaying(gameObject)) return;

    //         saveables = GetComponents<MonoSaveable>();

    //         if (DidSceneChange() || DidGameObjectChange())
    //         {
    //             Regen();
    //         }

    //         _instanceId = GetInstanceID();
    //         _prevUuid = _uuid;
    //         _scenePath = gameObject.scene.path;
    //     }

    //     void Reset()
    //     {
    //         if (string.IsNullOrEmpty(_prevUuid)) return;
    //         _uuid = _prevUuid;
    //     }

    //     bool DidSceneChange()
    //     {
    //         if (Application.IsPlaying(gameObject)) return false;
    //         if (string.IsNullOrEmpty(_scenePath)) return false;
    //         return gameObject.scene.path != _scenePath;
    //     }

    //     bool DidGameObjectChange()
    //     {
    //         if (Application.IsPlaying(gameObject)) return false;
    //         if (_instanceId == default) return false;
    //         return _instanceId != GetInstanceID();
    //     }

    //     void Regen()
    //     {
    //         SerializedObject serializedObject = new SerializedObject(this);
    //         SerializedProperty p_uuid = serializedObject.FindProperty("_uuid");

    //         p_uuid.stringValue = Guid.NewGuid().ToString();
    //         serializedObject.ApplyModifiedProperties();
    //     }

    //     // static Dictionary<string, SaveableEntity> globalLookup = new Dictionary<string, SaveableEntity>();

    //     // void Update()
    //     // {
    //     //     if (Application.IsPlaying(gameObject)) return;
    //     //     if (string.IsNullOrEmpty(gameObject.scene.path)) return;

    //     //     SerializedObject serializedObject = new SerializedObject(this);
    //     //     SerializedProperty p_uuid = serializedObject.FindProperty("_uuid");

    //     //     if (string.IsNullOrEmpty(p_uuid.stringValue) || !IsUnique(p_uuid.stringValue))
    //     //     {
    //     //         p_uuid.stringValue = Guid.NewGuid().ToString();
    //     //         serializedObject.ApplyModifiedProperties();
    //     //     }

    //     //     globalLookup[p_uuid.stringValue] = this;
    //     // }

    //     // bool IsUnique(string candidate)
    //     // {
    //     //     if (!globalLookup.ContainsKey(candidate)) return true;

    //     //     if (globalLookup[candidate] == this) return true;

    //     //     if (globalLookup[candidate] == null)
    //     //     {
    //     //         globalLookup.Remove(candidate);
    //     //         return true;
    //     //     }

    //     //     if (globalLookup[candidate].uuid.ToString() != candidate)
    //     //     {
    //     //         globalLookup.Remove(candidate);
    //     //         return true;
    //     //     }

    //     //     return false;
    //     // }
    // #endif

}
