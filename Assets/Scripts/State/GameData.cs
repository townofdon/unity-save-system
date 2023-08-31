
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GameData
{
    public int playerLives;
    public int money;
    public Vector2 playerSpawnPosition;
    public Dictionary<string, bool> enemiesKilled;
    public Dictionary<string, bool> collectiblesObtained;

    public static GameData defaultValues => new GameData
    {
        playerLives = 3,
        money = 0,
        playerSpawnPosition = Vector2.zero,
        enemiesKilled = new Dictionary<string, bool>(),
        collectiblesObtained = new Dictionary<string, bool>(),
    };
}
