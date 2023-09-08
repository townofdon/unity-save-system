using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] float respawnTime = 3f;

    Coroutine respawning;

    // not a singleton bc this is not global nor should it be
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
