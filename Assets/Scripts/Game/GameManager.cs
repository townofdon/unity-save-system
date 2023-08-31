using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] float respawnTime = 3f;

    Coroutine respawning;

    void Awake()
    {
        State.game.Clear();
    }

    void Start()
    {
        State.game.SetLoaded();
    }

    void OnEnable()
    {
        GlobalEvents.OnPlayerDeath += OnPlayerDeath;
    }

    void OnDisable()
    {
        GlobalEvents.OnPlayerDeath -= OnPlayerDeath;
    }

    void OnPlayerDeath()
    {
        if (respawning != null) StopCoroutine(respawning);
        respawning = StartCoroutine(CRespawnPlayer());
    }

    void RespawnPlayer()
    {
        // Instantiate(playerPrefab, transform.position, Quaternion.identity);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    IEnumerator CRespawnPlayer()
    {
        yield return new WaitForSeconds(respawnTime);
        RespawnPlayer();
    }
}
