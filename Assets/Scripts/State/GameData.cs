
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameData
{
    public int sceneIndex;
    public Vector2 playerSpawnPosition;
    public int playerLives;
    public int money;
    public SerializableDictionary<string, bool> enemiesKilled;
    public SerializableDictionary<string, bool> collectiblesObtained;
    public SerializableDictionary<string, bool> checkpointsReached;

    public static GameData defaultValues => new GameData
    {
        sceneIndex = -1,
        playerLives = 3,
        money = 0,
        playerSpawnPosition = Vector2.zero,
        enemiesKilled = new SerializableDictionary<string, bool>(),
        collectiblesObtained = new SerializableDictionary<string, bool>(),
        checkpointsReached = new SerializableDictionary<string, bool>(),
    };
}
