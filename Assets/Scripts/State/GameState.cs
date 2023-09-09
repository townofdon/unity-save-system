using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameState", menuName = "GameState", order = 0)]
public class GameState : ScriptableObject
{
    // TODO: REMOVE SERIALIZE FIELD
    [SerializeField] GameData data = GameData.defaultValues;
    [SerializeField] SaveMetadata metadata;

    DateTime timeStarted = DateTime.Now;

    #region SAVE_ACTIONS

    public void Clear()
    {
        data = GameData.defaultValues;
        metadata = new SaveMetadata();
        timeStarted = DateTime.Now;
    }

    public void SetData(GameData gameData)
    {
        data = gameData;
    }

    public GameData GetData()
    {
        return data;
    }

    public SaveMetadata GetMetadata()
    {
        return metadata;
    }

    public void OnSave(int sceneIndex, string sceneName = "")
    {
        metadata.timeLastUpdatedBinary = GetCurrentTime().ToBinary();
        metadata.timeSpentPlayingSeconds += GetTimeSpentPlayingSeconds();
        if (sceneIndex != -1) data.sceneIndex = sceneIndex;
        if (!string.IsNullOrWhiteSpace(sceneName)) metadata.sceneName = sceneName;
        timeStarted = DateTime.Now;
    }

    DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }

    double GetTimeSpentPlayingSeconds()
    {
        var currentTime = GetCurrentTime();
        var elapsed = currentTime - timeStarted;
        return elapsed.TotalSeconds;
    }

    #endregion

    #region GETTERS

    public int GetSceneIndex() => data.sceneIndex;
    public Vector2 GetPlayerSpawnPosition() => data.playerSpawnPosition;
    public int GetPlayerLives(int defaultValue = 3) => WithDefaultValue(data.playerLives, defaultValue);
    public int GetMoney(int defaultValue = 0) => WithDefaultValue(data.money, defaultValue);

    public bool GetIsEnemyKilled(string uuid) => WithLookup(data.enemiesKilled, uuid);
    public bool GetIsCollectibleObtained(string uuid) => WithLookup(data.collectiblesObtained, uuid);
    public bool GetIsCheckpointReached(string uuid) => WithLookup(data.checkpointsReached, uuid);

    #endregion

    #region SETTERS

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

    public void AddCheckpointReached(string uuid)
    {
        data.checkpointsReached[uuid] = true;
        var scene = SceneManager.GetActiveScene();
        OnSave(scene.buildIndex, scene.name);
    }

    #endregion

    #region UTILS

    T WithDefaultValue<T>(T value, T defaultValue = default)
    {
        if (value.Equals(default)) return defaultValue;
        return value;
    }

    bool WithLookup(Dictionary<string, bool> dict, string uuid)
    {
        if (!dict.ContainsKey(uuid)) return false;
        return dict[uuid];
    }

    #endregion UTILS
}
