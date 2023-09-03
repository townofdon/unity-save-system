using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
{
    [SerializeField] List<TKey> _keys = new List<TKey> { };
    [SerializeField] List<TValue> _values = new List<TValue> { };

    public void OnBeforeSerialize()
    {
        _keys.Clear();
        _values.Clear();

        foreach (KeyValuePair<TKey, TValue> pair in this)
        {
            _keys.Add(pair.Key);
            _values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();
        if (_keys.Count != _values.Count)
        {
            Debug.LogWarning($"[SerializedDictionary] Keys count did match values count ({_keys.Count},{_values.Count})");
        }
        for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
        {
            Add(_keys[i], _values[i]);
        }
    }
}
