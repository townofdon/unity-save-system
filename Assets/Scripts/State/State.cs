using System;
using UnityEngine;

public static class State
{
    static GameState _gameState;

    public static GameState game => GetOrLoadGameState();

    private static GameState GetOrLoadGameState()
    {
        if (_gameState != null) return _gameState;
        _gameState = Resources.Load<ScriptableObject>("GameState") as GameState;
        return _gameState;
    }
}