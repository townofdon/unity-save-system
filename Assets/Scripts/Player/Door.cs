using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    public enum DoorChannel
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
    }

    [SerializeField] public string targetScene;
    [SerializeField] public DoorChannel doorChannel;
    [SerializeField] public Transform spawnPoint;

    float timeElapsed;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (timeElapsed < 1f) return;
        if (!other.CompareTag("Player")) return;
        if (!other.TryGetComponent<Player>(out var player)) return;
        player.gameObject.SetActive(false);
        player.gameObject.tag = "Untagged";
        StartCoroutine(CWarpToLevel());
    }

    void Update()
    {
        timeElapsed += Time.deltaTime;
    }

    IEnumerator CWarpToLevel()
    {
        string outgoingScene = SceneManager.GetActiveScene().name;
        gameObject.tag = "Untagged";
        gameObject.transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
        yield return SceneManager.LoadSceneAsync(targetScene);
        var door = FindMatchingDoor(outgoingScene);
        if (door == null)
        {
            Debug.LogError("COULD NOT FIND MATCHING DOOR.");
            yield break;
        }
        var player = GameObject.FindWithTag("Player");
        var body = player.GetComponent<Rigidbody2D>();
        player.transform.position = door.spawnPoint.position;
        body.velocity = Vector2.zero;
        Destroy(gameObject);
        yield return null;
    }

    Door FindMatchingDoor(string outgoingScene)
    {
        var doors = GameObject.FindGameObjectsWithTag("Door");
        foreach (var door in doors)
        {
            if (door.TryGetComponent<Door>(out var other))
            {
                if (other.doorChannel == doorChannel && other.targetScene == outgoingScene) return other;
            }
        }
        return null;
    }
}
