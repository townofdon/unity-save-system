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

    Coroutine loading;

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

    void OnEnable()
    {
        GlobalEvents.OnPlayerDeath += OnPlayerDeath;
        GlobalEvents.OnCheckpointReached += OnCheckpointReached;
        GlobalEvents.OnStartGame += OnStartGame;
    }

    void OnDisable()
    {
        GlobalEvents.OnPlayerDeath -= OnPlayerDeath;
        GlobalEvents.OnCheckpointReached -= OnCheckpointReached;
        GlobalEvents.OnStartGame -= OnStartGame;
    }

    void OnPlayerDeath()
    {
        if (loading != null) StopCoroutine(loading);
        loading = StartCoroutine(CRespawnPlayer());
    }

    void OnStartGame()
    {
        if (loading != null) StopCoroutine(loading);
        State.game.Clear();
        loading = StartCoroutine(CLoadLevel(isNewGame: true));
    }

    void OnCheckpointReached()
    {
        var prevTimeScale = Time.timeScale;
        dataPersistor.SaveGame();
        Time.timeScale = prevTimeScale;
    }

    IEnumerator CRespawnPlayer()
    {
        yield return new WaitForSeconds(respawnTime);
        yield return CLoadLevel();
    }

    IEnumerator CLoadLevel(bool isNewGame = false)
    {
        var prevTimeScale = Time.timeScale;
        Time.timeScale = 0;
        dataPersistor.LoadGame();
        if (isNewGame) dataPersistor.LoadMetadata();
        var sceneIndex = State.game.GetSceneIndex();
        if (sceneIndex <= 0) sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex <= 0) sceneIndex = 1;
        yield return SceneManager.LoadSceneAsync(sceneIndex);
        if (isNewGame) dataPersistor.SaveGame();
        dataPersistor.NotifyLoaded();
        Time.timeScale = prevTimeScale;
        loading = null;
    }
}
