using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] float respawnTime = 3f;
    [SerializeField] DataPersistor dataPersistor;

    Coroutine respawning;

    // not a singleton
    private static GameManager instance;

    void Awake()
    {
        if (instance != null)
        {
            Debug.LogError($"[GameManager] more than one instance was encountered");
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }

        instance = this;

        State.game.Clear();
        Assert.IsNotNull(dataPersistor);
    }

    void Start()
    {
        dataPersistor.NewGame(SaveSlot.A);
    }

    void OnEnable()
    {
        GlobalEvents.OnPlayerDeath += OnPlayerDeath;
        GlobalEvents.OnCheckpointReached += OnCheckpointReached;
    }

    void OnDisable()
    {
        GlobalEvents.OnPlayerDeath -= OnPlayerDeath;
        GlobalEvents.OnCheckpointReached -= OnCheckpointReached;
    }

    void OnPlayerDeath()
    {
        if (respawning != null) StopCoroutine(respawning);
        respawning = StartCoroutine(CRespawnPlayer());
    }

    void OnCheckpointReached()
    {
        var prevTimeScale = Time.timeScale;
        Time.timeScale = 0;
        dataPersistor.SaveGame();
        Time.timeScale = prevTimeScale;
    }

    IEnumerator CRespawnPlayer()
    {
        yield return new WaitForSeconds(respawnTime);
        var sceneIndex = State.game.GetSceneIndex();
        Debug.Log("[GameManager] before scene load");
        yield return SceneManager.LoadSceneAsync(sceneIndex == -1 ? SceneManager.GetActiveScene().buildIndex : sceneIndex);
        Debug.Log("[GameManager] after scene load");
        dataPersistor.LoadGame();
        respawning = null;
    }
}
