using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class MonoSaveable : MonoBehaviour
{
    [ReadOnly][SerializeField] string _uuid = "";
    [SerializeField] string _scenePath;

    protected string uuid => _uuid;

    public abstract void OnGameSave(ref GameState gameState);

    public abstract void OnGameLoad(GameState gameState);

    [ContextMenu("Regenerate UUID")]
    public void RegenerateUUID()
    {
#if UNITY_EDITOR
        if (Application.isPlaying) return;
        Regen();
#endif
    }

#if UNITY_EDITOR

    static Dictionary<string, MonoSaveable> _globalLookup = new Dictionary<string, MonoSaveable>();
    int _instanceId;
    string _prevUuid;

    void OnValidate()
    {
        if (Application.isPlaying) return;

        _globalLookup ??= new Dictionary<string, MonoSaveable>();

        if (NeedsUUID() || DidSceneChange() || DidGameObjectChange() || !IsUnique(_uuid))
        {
            Regen();
        }

        // word to the wise - do NOT set serialized properties inside of OnValidate or you may trigger a stack overflow! with zero error logs which is quite annoying

        _prevUuid = _uuid;
        _instanceId = GetInstanceID();
        _globalLookup[_uuid] = this;
    }

    void Reset()
    {
        if (string.IsNullOrEmpty(_prevUuid)) return;
        _uuid = _prevUuid;
    }

    string GetScenePath()
    {
        if (gameObject.scene == null) return "";
        if (string.IsNullOrEmpty(gameObject.scene.path)) return "";
        return gameObject.scene.path;
    }

    bool NeedsUUID()
    {
        return string.IsNullOrEmpty(_uuid);
    }

    bool DidSceneChange()
    {
        if (Application.isPlaying) return false;
        if (string.IsNullOrEmpty(GetScenePath())) return false;
        if (string.IsNullOrEmpty(_scenePath)) return false;
        return _scenePath != GetScenePath();
    }

    bool DidGameObjectChange()
    {
        if (Application.isPlaying) return false;
        if (_instanceId == default) return false;
        if (GetInstanceID() == default) return false;
        return _instanceId != GetInstanceID();
    }

    bool IsUnique(string candidate)
    {
        if (_globalLookup == null) return false;
        if (!_globalLookup.ContainsKey(candidate)) return true;
        if (_globalLookup[candidate] == this) return true;
        if (_globalLookup[candidate] == null)
        {
            _globalLookup.Remove(candidate);
            return true;
        }
        if (_globalLookup[candidate].uuid.ToString() != candidate)
        {
            _globalLookup.Remove(candidate);
            return true;
        }

        return false;
    }

    void Regen()
    {
        SerializedObject serializedObject = new(this);
        SerializedProperty p_uuid = serializedObject.FindProperty("_uuid");
        SerializedProperty p_scenePath = serializedObject.FindProperty("_scenePath");

        p_uuid.stringValue = Guid.NewGuid().ToString();
        p_scenePath.stringValue = GetScenePath();
        serializedObject.ApplyModifiedProperties();
    }

#endif

}
