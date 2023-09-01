using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState", order = 0)]
public class GameState : ScriptableObject
{
    [SerializeField] GameData data = GameData.defaultValues;

    bool isLoaded;

    // getters

    public Vector2 GetPlayerSpawnPosition(Vector2 defaultValue) => WithDefaultValue(data.playerSpawnPosition, defaultValue);
    public int GetPlayerLives(int defaultValue) => WithDefaultValue(data.playerLives, defaultValue);
    public int GetMoney(int defaultValue) => WithDefaultValue(data.money, defaultValue);
    public bool GetIsEnemyKilled(string uuid) => WithLookup(data.enemiesKilled, uuid);
    public bool GetIsCollectibleObtained(string uuid) => WithLookup(data.collectiblesObtained, uuid);

    // setters

    public void Clear()
    {
        isLoaded = false;
        data = GameData.defaultValues;
    }

    public void SetLoaded()
    {
        isLoaded = true;
    }

    public void SetPlayerSpawnPosition(Vector3 position)
    {
        data.playerSpawnPosition = position;
    }

    public void GainMoney(int value)
    {
        data.money += value;
    }

    public void GainPlayerLife()
    {
        data.playerLives++;
    }

    public void LosePlayerLife()
    {
        data.playerLives--;
    }

    public void AddEnemyKilled(string uuid)
    {
        data.enemiesKilled[uuid] = true;
    }

    public void AddCollectibleObtained(string uuid)
    {
        data.collectiblesObtained[uuid] = true;
    }

    // util

    T WithDefaultValue<T>(T value, T defaultValue)
    {
        if (!isLoaded) return defaultValue;
        return value;
    }

    bool WithLookup(Dictionary<string, bool> dict, string uuid)
    {
        if (!isLoaded) return false;
        if (!dict.ContainsKey(uuid)) return false;
        return dict[uuid];
    }
}
